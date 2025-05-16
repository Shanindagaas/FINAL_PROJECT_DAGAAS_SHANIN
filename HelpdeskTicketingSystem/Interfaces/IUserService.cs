using HelpdeskTicketingSystem.DTOs;

namespace HelpdeskTicketingSystem.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetUsersInDepartmentAsync(int departmentID);
        Task<UserDTO> GetUsersByIDAsync(int id);
        Task<bool> isUserInDepartmentAsync(int userID, int departmentID);
    }
}
