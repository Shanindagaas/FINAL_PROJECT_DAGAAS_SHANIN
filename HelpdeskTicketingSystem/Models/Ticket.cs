namespace HelpdeskTicketingSystem.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CreatedById { get; set; }
        public int? AssignedToId { get; set; }
        public int DepartmentId { get; set; }
        public string Severity { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual User CreatedBy { get; set; } = null!;
        public virtual User? AssignedTo {  get; set; }
        public virtual Department Department { get; set; } = null!;
        public virtual ICollection<Remark> Remarks { get; set; } = new List<Remark>();
    }
}
