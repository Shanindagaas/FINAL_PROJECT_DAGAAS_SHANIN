using HelpdeskTicketingSystem.DTOs;

namespace HelpdeskTicketingSystem.Interfaces
{
    public interface ITicketService
    {
        Task<IEnumerable<TicketDTO>> getTicketsForUserAsync(int userID);
        Task<TicketDTO> getTicketByIDAsync(int ticketID, int userID);
        Task<TicketDTO> CreateTicketAsync(TicketCreateDTO ticketDTO, int createdByID);
        Task<TicketDTO> UpdateTicketAsync(TicketUpdateDTO ticketDTO, int userID);
        Task<bool> DeleteTicketAsync(int ticketID, int userID);
        Task<TicketDTO> AssignTicketAsync(int ticketID, int assignToUserID, int requestedByUserID);
        Task<TicketDTO> ChangeTicketStatusAsync(int ticketID, string newStatus, int userID);
        Task<TicketDTO> ChangeTicketDepartmentAsync(int ticketID, int newDepartmentID, int requestedByID);
    }
}
