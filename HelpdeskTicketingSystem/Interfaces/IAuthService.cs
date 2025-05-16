using HelpdeskTicketingSystem.DTOs;

namespace HelpdeskTicketingSystem.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> LoginAsync(LoginDTO loginDTO);
        Task<AuthResponseDTO> RegisterAsync(RegisterUserDTO registerDTO);
        Task<UserDTO> getCurrentUserAsync(int userID);
    }
}
