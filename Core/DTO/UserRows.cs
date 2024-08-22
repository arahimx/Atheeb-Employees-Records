namespace Core.DTO
{
    public class UserRows
    {
        public int Id { get; set; }
        public string? CreatedById { get; set; }
        public string? TraineeName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? Status { get; set; }
        public bool? isEmergency { get; set; }
        public DateTime? ResponseDate { get; set; }
    }
}
