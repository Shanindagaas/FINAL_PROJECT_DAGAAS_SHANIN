namespace HelpdeskTicketingSystem.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<User> Users { get; set; } = new List<User>();
        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
