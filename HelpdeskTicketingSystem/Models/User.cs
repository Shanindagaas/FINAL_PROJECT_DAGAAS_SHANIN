namespace HelpdeskTicketingSystem.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        //Admin, Supervisor, Officer, Junior Officer
        public string Role { get; set; } = string.Empty;
        public int DepartmentId { get; set; }

        public virtual Department Department { get; set; } = null!;
        public virtual ICollection<Ticket> CreatedTickets { get; set; } = new List<Ticket>();
        public virtual ICollection<Ticket> AssignedTickets { get; set; } = new List<Ticket>();
        public virtual ICollection<Remark> Remarks { get; set; } = new List<Remark>();
    }
}
