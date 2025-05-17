namespace HelpdeskTicketingSystem.DTOs
{
    public class TicketCreateDTO
    {
        public string Title { get; set; } = string.Empty;
        public int CreatedBy { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Severity {  get; set; } = string.Empty;
        public int DepartmentID { get; set; }
    }
}
