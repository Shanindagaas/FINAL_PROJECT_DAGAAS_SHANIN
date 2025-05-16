namespace HelpdeskTicketingSystem.DTOs
{
    public class TicketUpdateDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? Severity { get; set; }
        public int? AssignedToId { get; set; }
        public int? DepartmentID {  get; set; }
    }
}
