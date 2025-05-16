namespace HelpdeskTicketingSystem.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set;} = string.Empty;
    }
}
