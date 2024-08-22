using AutoMapper;
using Core.IRepository;
using Microsoft.AspNetCore.Mvc;
using RestApi.Interface;
using RestApi.ViewModels;
namespace RestApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EmployeesRecordsController : ControllerBase
    {
        private readonly IUnitOfWork<UsersSkills>? _userSkills;
        private readonly IUnitOfWork<Departments>? _departments;
        private readonly IUnitOfWork<UserEmploymentRecords>? _previousEmployer;
        private readonly IUnitOfWork<UserInfo>? _context;
        private readonly IUnitOfWork<Skills>? _skills;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public EmployeesRecordsController(IUnitOfWork<UserEmploymentRecords>? previousEmployer, IUnitOfWork<UsersSkills>? userSkills, IUnitOfWork<Departments>? departments, IUnitOfWork<UserInfo> context, IMapper mapper, IUnitOfWork<Skills>? skills, IFileService fileService)
        {
            _userSkills = userSkills;
            _departments = departments;
            _context = context;
            _mapper = mapper;
            _skills = skills;
            _previousEmployer = previousEmployer;
            _fileService = fileService;
        }

        [HttpGet]
        public ActionResult<UsersListViewModel> SearchUser([FromBody] string? values)
        {
            try
            {
                var user = _context!.Entity.GetAll().FirstOrDefault(x => x.FullName == values || x.Title == values || x.Email == values);
                if (user == null)
                {
                    return NotFound(new { isSuccesfull = true, Message = $"Users not exist." });
                }
                var viewModel = _mapper.Map<UsersListViewModel>(user);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { isSuccesfull = false, Message = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult<UsersListViewModel> GetUser(int id)
        {
            try
            {
                var user = _context!.Entity.GetById(id);
                if (user == null)
                {
                    return NotFound(new { isSuccesfull = true, Message = $"Users not exist." });
                }
                var viewModel = _mapper.Map<UsersListViewModel>(user);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { isSuccesfull = false, Message = ex.Message });
            }
        }

        [HttpGet()]
        public ActionResult<UsersListViewModel> GetAllUsers()
        {
            try
            {
                var user = _context!.Entity.GetAll();
                if (user == null)
                {
                    return NotFound(new { isSuccesfull = true, Message = $"Users not exist." });
                }
                var viewModel = _mapper.Map<IEnumerable<UsersListViewModel>>(user);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { isSuccesfull = false, Message = ex.Message });
            }
        }


        [HttpPost]
        public async Task<ActionResult<UsersFormViewModel>> AddUser([FromForm] UsersFormViewModel user)
        {
            try
            {
                // Validate input
                if (user == null || !ModelState.IsValid)
                {
                    return BadRequest(new { isSuccess = false, Message = "Invalid user data provided." });
                }

                // Check if the user already exists
                var existingUser = _context.Entity.GetAll().FirstOrDefault(x => x.Email == user.Email);
                if (existingUser != null)
                {
                    return BadRequest(new { isSuccess = false, Message = $"User with email {user.Email} already exists." });
                }

                // Handle previous employer records if provided
                int? previousEmployerId = null;
                if (!string.IsNullOrEmpty(user.PreviousEmployerName) &&
                    !string.IsNullOrEmpty(user.PreviousEmployerCountry) &&
                    user.PreviousJoiningDate != null &&
                    user.PreviousLeaveDate != null)
                {
                    var prevEmployer = new UserEmploymentRecords
                    {
                        Country = user.PreviousEmployerCountry,
                        JoiningDate = user.PreviousJoiningDate.Value,
                        LeaveDate = user.PreviousLeaveDate.Value,
                        Name = user.PreviousEmployerName
                    };
                    _previousEmployer.Entity.Insert(prevEmployer);
                    _previousEmployer.Save(); // Use async Save method
                    previousEmployerId = prevEmployer.Id;
                }

                // Get or create skills and department
                var skillIds = await GetOrCreateSkillsAsync(user.Skills);
                var departmentId = await GetOrCreateDepartmentAsync(user.DepartmentName);

                // Upload files
                string pictureName = null;
                string certificateName = null;

                if (user.Picture != null)
                {
                    pictureName = await _fileService.UploadFileAsync(user.Picture);
                }

                if (user.CertificateAttachment != null)
                {
                    certificateName = await _fileService.UploadFileAsync(user.CertificateAttachment);
                }
                // Create the new user
                var userEntity = new UserInfo
                {
                    FullName = user.FullName,
                    Address = user.Address,
                    Age = user.Age,
                    Email = user.Email,
                    Title = user.Title,
                    Picture = pictureName,
                    YearsOfExperience = user.YearsOfExperience,
                    CertificateName = user.CertificateName,
                    CertificateAttachment = certificateName,
                    MonthlySalary = user.MonthlySalary,
                    EvaluationRate = user.EvaluationRate,
                    Bounce = CalculateBounce(user.MonthlySalary,user.EvaluationRate,user.DepartmentName),
                    DepartmentId = departmentId,
                    HiringDate = user.HiringDate,
                    PreviousEmployersId = previousEmployerId
                };

                _context.Entity.Insert(userEntity);
                _context.Save(); // Use async Save method

                // Associate the skills with the user
                foreach (var skillId in skillIds.Values)
                {
                    var userSkill = new UsersSkills { UserId = userEntity.Id, SkillId = skillId };
                    _userSkills.Entity.Insert(userSkill);
                }
                _userSkills.Save(); // Use async Save method

                return CreatedAtAction(nameof(GetUser), new { id = userEntity.Id }, new { isSuccess = true, Message = $"User {user.FullName} added successfully." });
            }
            catch (Exception ex)
            {
                // Handle exceptions and return a descriptive error message
                return StatusCode(500, new { isSuccess = false, Message = "An error occurred while processing the request.", Details = ex.Message });
            }
        }
        [HttpPut]
        public async Task<ActionResult<UsersFormViewModel>> UpdateUser(int id, [FromForm] UsersFormViewModel user)
        {
            try
            {
                // Validate input
                if (user == null || !ModelState.IsValid)
                {
                    return BadRequest(new { isSuccess = false, Message = "Invalid user data provided." });
                }

                // Check if the user exists
                var existingUser = _context.Entity.GetAll().FirstOrDefault(x => x.Id == id);
                if (existingUser == null)
                {
                    return NotFound(new { isSuccess = false, Message = $"User with ID {id} does not exist." });
                }

                // Handle previous employer records if provided
                if (!string.IsNullOrEmpty(user.PreviousEmployerName) &&
                    !string.IsNullOrEmpty(user.PreviousEmployerCountry) &&
                    user.PreviousJoiningDate != null &&
                    user.PreviousLeaveDate != null)
                {
                    var prevEmployer = new UserEmploymentRecords
                    {
                        Country = user.PreviousEmployerCountry,
                        JoiningDate = user.PreviousJoiningDate.Value,
                        LeaveDate = user.PreviousLeaveDate.Value,
                        Name = user.PreviousEmployerName
                    };
                    _previousEmployer.Entity.Insert(prevEmployer);
                    _previousEmployer.Save(); // Use async Save method
                    existingUser.PreviousEmployersId = prevEmployer.Id;
                }

                // Get or create skills and department
                var skillIds = await GetOrCreateSkillsAsync(user.Skills);
                var departmentId = await GetOrCreateDepartmentAsync(user.DepartmentName);

                // Update files if provided
                if (user.Picture != null)
                {
                    if (!string.IsNullOrEmpty(existingUser.Picture))
                    {
                        _fileService.DeleteFileAsync(existingUser.Picture); // Optionally delete the old picture
                    }
                    existingUser.Picture = await _fileService.UploadFileAsync(user.Picture);
                }

                if (user.CertificateAttachment != null)
                {
                    if (!string.IsNullOrEmpty(existingUser.CertificateAttachment))
                    {
                        _fileService.DeleteFileAsync(existingUser.CertificateAttachment); // Optionally delete the old certificate
                    }
                    existingUser.CertificateAttachment = await _fileService.UploadFileAsync(user.CertificateAttachment);
                }

                // Update the user details
                existingUser.FullName = user.FullName;
                existingUser.Address = user.Address;
                existingUser.Age = user.Age;
                existingUser.Email = user.Email;
                existingUser.Title = user.Title;
                existingUser.YearsOfExperience = user.YearsOfExperience;
                existingUser.CertificateName = user.CertificateName;
                existingUser.MonthlySalary = user.MonthlySalary;
                existingUser.EvaluationRate = user.EvaluationRate;
                existingUser.Bounce = CalculateBounce(user.MonthlySalary, user.EvaluationRate, user.DepartmentName);
                existingUser.DepartmentId = departmentId;
                existingUser.HiringDate = user.HiringDate;

                _context.Entity.Update(existingUser);
                _context.Save(); // Use async Save method

                // Update the user's skills
                var existingUserSkills = _userSkills.Entity.GetAll().Where(x => x.UserId == id).ToList();
                foreach (var userSkill in existingUserSkills)
                {
                    _userSkills.Entity.Delete(userSkill);
                }
                _userSkills.Save(); // Use async Save method

                foreach (var skillId in skillIds.Values)
                {
                    var newUserSkill = new UsersSkills { UserId = existingUser.Id, SkillId = skillId };
                    _userSkills.Entity.Insert(newUserSkill);
                }
                _userSkills.Save(); // Use async Save method

                return Ok(new { isSuccess = true, Message = $"User {user.FullName} updated successfully." });
            }
            catch (Exception ex)
            {
                // Handle exceptions and return a descriptive error message
                return StatusCode(500, new { isSuccess = false, Message = "An error occurred while processing the request.", Details = ex.Message });
            }
        }


        // DELETE: api/user/5
        [HttpDelete]
        public async Task<IActionResult> DeleteUser(int id)
        {
            // Retrieve the user by ID
            var user = _context.Entity.GetById(id);
            if (user == null)
            {
                return NotFound(new { isSuccess = false, Message = "User not found." });
            }

            try
            {
                // Optionally delete the old certificate if it exists
                if (!string.IsNullOrEmpty(user.CertificateAttachment))
                {
                    await _fileService.DeleteFileAsync(user.CertificateAttachment);
                }

                // Optionally delete the old picture if it exists
                if (!string.IsNullOrEmpty(user.Picture))
                {
                    await _fileService.DeleteFileAsync(user.Picture);
                }

                // Delete the user record from the database
                _context.Entity.Delete(id);
                _context.Save(); // Ensure changes are saved to the database

                return Ok(new { isSuccess = true, Message = "User deleted successfully." });
            }
            catch (Exception ex)
            {
                // Handle exceptions and return a descriptive error message
                return StatusCode(500, new { isSuccess = false, Message = "An error occurred while deleting the user.", Details = ex.Message });
            }
        }



        public async Task<Dictionary<string, int>> GetOrCreateSkillsAsync(IEnumerable<string> skillTitles)
        {
            var skillIdMap = new Dictionary<string, int>();

            foreach (var skillTitle in skillTitles)
            {
                var existingSkill = _skills.Entity.GetAll().FirstOrDefault(x => x.Title == skillTitle);

                if (existingSkill == null)
                {
                    // Create a new skill if it does not exist
                    var newSkill = new Skills { Title = skillTitle };
                    _skills.Entity.Insert(newSkill);
                    _context.Save();

                    skillIdMap[skillTitle] = newSkill.Id;
                }
                else
                {
                    skillIdMap[skillTitle] = existingSkill.Id;
                }
            }

            return skillIdMap;
        }


        public async Task<int> GetOrCreateDepartmentAsync(string departmentName)
        {
            var existingDepartment = _departments.Entity.GetAll().FirstOrDefault(x => x.Title == departmentName);

            if (existingDepartment == null)
            {
                // Create a new department if it does not exist
                var newDepartment = new Departments { Title = departmentName };
                _departments.Entity.Insert(newDepartment);
                _context.Save();
                return newDepartment.Id;
            }
            return existingDepartment.Id;
        }

        public decimal CalculateBounce(decimal salaryInMonth, int? evaluationRate, string departmentName)
        {
            // Ensure the salary input is non-negative
            if (salaryInMonth < 0)
            {
                throw new ArgumentException("Salary must be non-negative.");
            }

            // Ensure the evaluation rate is non-negative, if provided
            if (evaluationRate < 0)
            {
                throw new ArgumentException("Evaluation rate must be non-negative.");
            }

            // If evaluation rate is null, default it to 0
            int effectiveEvaluationRate = evaluationRate ?? 0;

            // Convert salary from month to year
            decimal salaryInYear = salaryInMonth * 12;

            // Calculate the department ratio based on the department name
            decimal departmentRatio = CalculateDepartmentRatio(departmentName);

            // Calculate the two components of the bounce
            decimal fifteenPercentOfSalary = salaryInYear * 0.15m;
            decimal evaluationBasedBounce = salaryInYear * effectiveEvaluationRate * departmentRatio;

            // Return the maximum of the two components
            return Math.Max(fifteenPercentOfSalary, evaluationBasedBounce);
        }

        private decimal CalculateDepartmentRatio(string departmentName)
        {
            // Get the total number of users across all departments
            int totalUsers = _context.Entity.GetAll().Count();

            if (totalUsers == 0)
            {
                throw new InvalidOperationException("No users found in the system.");
            }

            // Get the number of users in the specified department
            int usersInDepartment = _context.Entity.GetAll().Count(u => u.Department.Title == departmentName);

            // Calculate the department ratio
            decimal departmentRatio = (decimal)usersInDepartment / totalUsers;

            return departmentRatio;
        }

    }
}