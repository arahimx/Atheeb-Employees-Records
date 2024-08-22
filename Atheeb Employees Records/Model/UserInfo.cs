namespace Atheeb_Employees_Records.Model
{
    public class UserInfo
    {
        public int ID { get; set; }
        public string FullName { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string Picture { get; set; }
        public int YearsOfExperience { get; set; }
        public int DepartmentId { get; set; }
        public int? PreviousEmployersId { get; set; }
        public DateTime HiringDate { get; set; }
        public int SkillId { get; set; }
        public int? CertificatesId { get; set; }
        public decimal MonthlySalary { get; set; }
        public float? EvaluationRate { get; set; }
        public decimal? Bounce { get; set; }
        public UserEmploymentRecords? PreviousEmployers { get; set; }
        public Departments Department { get; set; }
        public UserCertificates? Certificates { get; set; }
        public Skills Skill { get; set; }
    }
}