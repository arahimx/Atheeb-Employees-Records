namespace Core.Entity
{
    public class UserInfo
    {
        public int Id { get; set; }
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
        public string? CertificateName { get; set; }
        public string? CertificateAttachment { get; set; }
        public decimal MonthlySalary { get; set; }
        public float? EvaluationRate { get; set; }
        public decimal? Bounce { get; set; }
        public UserEmploymentRecords? PreviousEmployers { get; set; }
        public Departments Department { get; set; }
    }
}