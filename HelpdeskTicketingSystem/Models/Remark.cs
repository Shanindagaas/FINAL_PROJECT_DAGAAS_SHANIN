namespace HelpdeskTicketingSystem.Models
{
    public class Remark
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual Ticket Ticket { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
