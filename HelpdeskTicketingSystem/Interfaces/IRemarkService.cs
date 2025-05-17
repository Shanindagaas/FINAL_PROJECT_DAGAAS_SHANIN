using HelpdeskTicketingSystem.DTOs;

namespace HelpdeskTicketingSystem.Interfaces
{
    public interface IRemarkService
    {
        Task<RemarkDTO> AddRemarkAsync(RemarkDTO remarkDTO, int userID, string? userRole, int departmentID);
        Task<IEnumerable<RemarkDTO>> GetRemarksByTicketIDAsync(int ticketID, int userID);
        Task<IEnumerable<RemarkDTO>> GetRemarkByIdAsync(int ticketID, int userID, string? userRole, int departmentID);

        Task<bool> DeleteRemarkAsync(int ticketID, int userID, string? userRole, int departmentID);
    }
}
