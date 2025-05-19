namespace HelpdeskTicketingSystem.DTOs
{
    public class TicketCreateDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int CreatedBy { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Severity {  get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public int? AssignedToId { get; set; }
    }
}
