using HelpdeskTicketingSystem.DTOs;

namespace HelpdeskTicketingSystem.Interfaces
{
    public interface IRemarkService
    {
        Task<RemarkDTO> AddRemarkAsync(RemarkCreateDTO remarkDTO, int userID);
        Task<IEnumerable<RemarkDTO>> GetRemarksByTicketIDAsync(int ticketID, int userID);
    }
}
