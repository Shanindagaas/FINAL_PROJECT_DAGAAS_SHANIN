namespace HelpdeskTicketingSystem.DTOs
{
    public class AuthResponseDTO
    {
        public bool Success { get; set; }
        public string Token { get; set; } = string.Empty;
        public UserDTO? User { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
