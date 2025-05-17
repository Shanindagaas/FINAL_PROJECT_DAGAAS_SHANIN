using HelpdeskTicketingSystem.DTOs;
using HelpdeskTicketingSystem.Enums;

namespace HelpdeskTicketingSystem.Interfaces
{
    public interface ITicketService
    {
        Task<IEnumerable<TicketDTO>> getTicketsForUserAsync(int userID, string? userRole, int departmentID);
        Task<TicketDTO> getTicketByIDAsync(int ticketID, int userID, string? userRole, int departmentID);
        Task<TicketDTO> CreateTicketAsync(TicketCreateDTO ticketDTO, int createdByID);
        Task<TicketDTO> UpdateTicketAsync(int id, TicketUpdateDTO ticketDTO, int userID, string? userRole, int departmentID);
        Task<bool> DeleteTicketAsync(int ticketID, int userID);
        Task<TicketDTO> AssignTicketAsync(int ticketID, int assignToUserID, int requestedByUserID, string? userRole, int departmentID);
        Task<TicketDTO> ChangeTicketStatusAsync(int ticketID, string newStatus, int userID);
        Task<TicketDTO> UpdateTicketStatusAsync(int id, TicketStatus newStatus, int userID, string? userRole, int departmentID);
        Task<TicketDTO> ChangeTicketDepartmentAsync(int ticketID, int newDepartmentID, int requestedByID, string? userRole, int departmentID);

        Task<TicketDTO> getDashboardDataAsync(int userID, string? userRole, int departmentID);

        Task<bool> UserHasAccessToTicketAsync(int ticketID, int userID, string? userRole, int departmentID);
    }
}
