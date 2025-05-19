namespace HelpdeskTicketingSystem.DTOs
{
    public class RemarkDTO
    {
        public int Id { get; set; }
        public int TicketID { get; set; }
        public int UserID { get; set; }
        public string UserRole { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
