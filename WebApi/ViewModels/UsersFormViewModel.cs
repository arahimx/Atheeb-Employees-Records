using System.ComponentModel.DataAnnotations;
namespace RestApi.ViewModels
{
    public class UsersFormViewModel
    {
        public int? Id { get; set; }

        //User Personal Information
        #region PersonalInfo
        [DataType(DataType.Text, ErrorMessage = Errors.OnlyEnglishLetters), Required(ErrorMessage = Errors.RequiredField)]
        public string FullName { get; set; }

        [DataType(DataType.Text, ErrorMessage = Errors.OnlyNumbers), Required(ErrorMessage = Errors.RequiredField)]
        public int Age { get; set; }

        [DataType(DataType.Text, ErrorMessage = Errors.OnlyEnglishLetters), Required(ErrorMessage = Errors.RequiredField)]
        public string Address { get; set; }

        [DataType(DataType.EmailAddress, ErrorMessage = Errors.Invalid), Required(ErrorMessage = Errors.RequiredField)]
        public string Email { get; set; }

        [DataType(DataType.Upload, ErrorMessage = Errors.OnlyEnglishLetters), Required(ErrorMessage = Errors.RequiredField)]
        public IFormFile Picture { get; set; }

        [Required(ErrorMessage = Errors.RequiredField)]
        public List<string>? Skills { get; set; }

        [DataType(DataType.Text, ErrorMessage = Errors.OnlyEnglishLetters)]
        public string? CertificateName { get; set; }

        [DataType(DataType.Upload, ErrorMessage = Errors.OnlyEnglishLetters)]
        public IFormFile? CertificateAttachment { get; set; }
        #endregion

        //Job Information
        #region JobInfo
        [DataType(DataType.Text, ErrorMessage = Errors.OnlyEnglishLetters), Required(ErrorMessage = Errors.RequiredField), MaxLength(200, ErrorMessage =Errors.MaxLength)]
        public string Title { get; set; }

        [DataType(DataType.Text, ErrorMessage = Errors.OnlyNumbers), Required(ErrorMessage = Errors.RequiredField)]
        public int YearsOfExperience { get; set; }

        [Display(Name = "DepartmentName"), DataType(DataType.Text, ErrorMessage = Errors.OnlyEnglishLetters), Required(ErrorMessage = Errors.RequiredField)]
        public string DepartmentName { get; set; }

        [DataType(DataType.Date, ErrorMessage = Errors.Invalid), Required(ErrorMessage = Errors.RequiredField)]
        public DateTime HiringDate { get; set; }

        [DataType(DataType.Text, ErrorMessage = Errors.OnlyNumbers), Required(ErrorMessage = Errors.RequiredField)]
        public decimal MonthlySalary { get; set; }

        [DataType(DataType.Text, ErrorMessage = Errors.OnlyNumbers), MinLength(0, ErrorMessage = Errors.MaxMinLength),MaxLength(5, ErrorMessage =Errors.MaxMinLength)]
        public int? EvaluationRate { get; set; }

        [DataType(DataType.Text, ErrorMessage = Errors.OnlyNumbers)]
        public decimal? Bounce { get; set; }
        #endregion

        //Previous Employment Information
        #region Prevoise Employement
        [DataType(DataType.Text, ErrorMessage = Errors.OnlyEnglishLetters)]
        public string? PreviousEmployerName { get; set; }

        [DataType(DataType.Text, ErrorMessage = Errors.OnlyEnglishLetters)]
        public string? PreviousEmployerCountry { get; set; }

        [DataType(DataType.Date, ErrorMessage = Errors.Invalid)]
        public DateTime? PreviousJoiningDate { get; set; }
        
        [DataType(DataType.Date, ErrorMessage = Errors.Invalid)]
        public DateTime? PreviousLeaveDate { get; set; }
        #endregion
    }
}