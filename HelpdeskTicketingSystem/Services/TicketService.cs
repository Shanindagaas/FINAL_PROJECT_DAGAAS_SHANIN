using HelpdeskTicketingSystem.Data;
using HelpdeskTicketingSystem.DTOs;
using HelpdeskTicketingSystem.Enums;
using HelpdeskTicketingSystem.Interfaces;
using HelpdeskTicketingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HelpdeskApp.Infrastructure.Services
{
    public class TicketService : ITicketService
    {
        private readonly AppDbContext _context;
        private readonly IDepartmentServices _departmentService;
        private readonly IUserService _userService;

        public TicketService(
            AppDbContext context,
            IDepartmentServices departmentService,
            IUserService userService)
        {
            _context = context;
            _departmentService = departmentService;
            _userService = userService;
        }

        public async Task<IEnumerable<TicketDTO>> GetTicketsForUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found");
            }

            var query = _context.Tickets
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .Include(t => t.Department)
                .AsQueryable();

            //SHOW TICKETS FROM ASSIGNED DEPARTMENT (AND CRITICAL TICKETS FROM ANY DEP)
            query = query.Where(t =>
                t.DepartmentId == user.DepartmentId ||
                t.Severity == SeverityLevel.Critical.ToString());

            if (user.Role != UserRole.Admin.ToString() &&
                user.Role != UserRole.Supervisor.ToString())
            {
                query = query.Where(t =>
                    t.DepartmentId == user.DepartmentId ||
                    (t.Severity == SeverityLevel.Critical.ToString() && t.AssignedToId == userId));
            }

            var tickets = await query.ToListAsync();

            return tickets.Select(t => MapToTicketDto(t));
        }

        public async Task<TicketDTO> GetTicketByIdAsync(int ticketId, int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found");
            }

            var ticket = await _context.Tickets
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .Include(t => t.Department)
                .Include(t => t.Remarks)
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null)
            {
                return null;
            }

            bool canAccess = ticket.DepartmentId == user.DepartmentId;

            if (!canAccess && ticket.Severity == SeverityLevel.Critical.ToString())
            {
                canAccess = user.Role == UserRole.Supervisor.ToString() ||
                            user.Role == UserRole.Admin.ToString() ||
                            ticket.AssignedToId == userId;
            }

            if (!canAccess)
            {
                throw new UnauthorizedAccessException("You don't have access to this ticket");
            }
            return MapToTicketDto(ticket);
        }

        public async Task<TicketDTO> CreateTicketAsync(TicketCreateDTO ticketDto, int createdById)
        {
            var user = await _context.Users.FindAsync(createdById);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found");
            }

            var ticket = new Ticket
            {
                Title = ticketDto.Title,
                Description = ticketDto.Description,
                CreatedById = createdById,
                DepartmentId = ticketDto.DepartmentID,
                Severity = ticketDto.Severity,
                Status = TicketStatus.Open.ToString(),
                CreatedAt = DateTime.UtcNow
            };
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
            return await GetTicketByIdAsync(ticket.Id, createdById);
        }

        public async Task<TicketDTO> UpdateTicketAsync(TicketUpdateDTO ticketDto, int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found");
            }

            var ticket = await _context.Tickets
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .Include(t => t.Department)
                .FirstOrDefaultAsync(t => t.Id == ticketDto.Id);

            if (ticket == null)
            {
                return null;
            }

            bool canAccess = await CanUserAccessTicketAsync(ticket, userId);
            if (!canAccess)
            {
                throw new UnauthorizedAccessException("You don't have access to update this ticket");
            }

            if (ticket.Severity == SeverityLevel.Critical.ToString() &&
                user.Role == UserRole.JuniorOfficer.ToString() &&
                ticket.AssignedToId != userId)
            {
                throw new UnauthorizedAccessException("Junior Officers can't handle Critical tickets unless assigned");
            }

            if ((ticketDto.AssignedToId.HasValue && ticketDto.AssignedToId != ticket.AssignedToId) ||
                (ticketDto.DepartmentID.HasValue && ticketDto.DepartmentID != ticket.DepartmentId))
            {
                bool isSupervisor = user.Role == UserRole.Supervisor.ToString() ||
                                   user.Role == UserRole.Admin.ToString();

                if (!isSupervisor)
                {
                    throw new UnauthorizedAccessException("Only supervisors can reassign tickets or change departments");
                }

                if (ticketDto.DepartmentID.HasValue)
                {
                    var targetDept = await _context.Departments.FindAsync(ticketDto.DepartmentID.Value);
                    if (targetDept == null)
                    {
                        throw new InvalidOperationException("Target department does not exist");
                    }
                }

                if (ticketDto.AssignedToId.HasValue)
                {
                    var targetUser = await _context.Users.FindAsync(ticketDto.AssignedToId.Value);
                    if (targetUser == null)
                    {
                        throw new InvalidOperationException("Target user does not exist");
                    }

                    if (targetUser.DepartmentId != ticket.DepartmentId &&
                        !ticketDto.DepartmentID.HasValue)
                    {
                        throw new InvalidOperationException("Can't assign ticket to user in different department");
                    }
                }
            }

            if (!string.IsNullOrEmpty(ticketDto.Title))
            {
                ticket.Title = ticketDto.Title;
            }

            if (!string.IsNullOrEmpty(ticketDto.Description))
            {
                ticket.Description = ticketDto.Description;
            }

            if (!string.IsNullOrEmpty(ticketDto.Status))
            {
                ticket.Status = ticketDto.Status;
            }

            if (!string.IsNullOrEmpty(ticketDto.Severity))
            {
                ticket.Severity = ticketDto.Severity;
            }

            if (ticketDto.AssignedToId.HasValue)
            {
                ticket.AssignedToId = ticketDto.AssignedToId;
            }

            if (ticketDto.DepartmentID.HasValue)
            {
                ticket.DepartmentId = ticketDto.DepartmentID.Value;
            }

            await _context.SaveChangesAsync();

            return await GetTicketByIdAsync(ticket.Id, userId);
        }

        public async Task<bool> DeleteTicketAsync(int ticketId, int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found");
            }

            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null)
            {
                return false;
            }

            bool canDelete = user.Role == UserRole.Admin.ToString() ||
                             (user.Role == UserRole.Supervisor.ToString() && user.DepartmentId == ticket.DepartmentId) ||
                             ticket.CreatedById == userId;

            if (!canDelete)
            {
                throw new UnauthorizedAccessException("You don't have permission to delete this ticket");
            }

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<TicketDTO> AssignTicketAsync(int ticketId, int assignToUserId, int requestedByUserId)
        {
            var requestingUser = await _context.Users.FindAsync(requestedByUserId);
            if (requestingUser == null)
            {
                throw new UnauthorizedAccessException("User not found");
            }

            bool canAssign = requestingUser.Role == UserRole.Supervisor.ToString() ||
                             requestingUser.Role == UserRole.Admin.ToString();

            if (!canAssign)
            {
                throw new UnauthorizedAccessException("You don't have permission to assign tickets");
            }

            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null)
            {
                return null;
            }

            var targetUser = await _context.Users.FindAsync(assignToUserId);
            if (targetUser == null)
            {
                throw new InvalidOperationException("Target user does not exist");
            }

            if (targetUser.DepartmentId != ticket.DepartmentId)
            {
                throw new InvalidOperationException("Can't assign ticket to user in different department");
            }

            if (ticket.Severity == SeverityLevel.Critical.ToString() &&
                targetUser.Role == UserRole.JuniorOfficer.ToString())
            {
                var warningRemark = new Remark
                {
                    TicketId = ticketId,
                    UserId = requestedByUserId,
                    Content = "Warning: Assigned Critical ticket to Junior Officer",
                    CreatedAt = DateTime.UtcNow
                };

                _context.Remarks.Add(warningRemark);
            }
            ticket.AssignedToId = assignToUserId;
            await _context.SaveChangesAsync();

            return await GetTicketByIdAsync(ticketId, requestedByUserId);
        }

        public async Task<TicketDTO> ChangeTicketStatusAsync(int ticketId, string newStatus, int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found");
            }

            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null)
            {
                return null;
            }

            bool canAccess = await CanUserAccessTicketAsync(ticket, userId);
            if (!canAccess)
            {
                throw new UnauthorizedAccessException("You don't have access to update this ticket");
            }

            if (ticket.Severity == SeverityLevel.Critical.ToString() &&
                user.Role == UserRole.JuniorOfficer.ToString() &&
                ticket.AssignedToId != userId)
            {
                throw new UnauthorizedAccessException("Junior Officers can't handle Critical tickets unless assigned");
            }

            // Update the status
            ticket.Status = newStatus;
            await _context.SaveChangesAsync();

            return await GetTicketByIdAsync(ticketId, userId);
        }

        public async Task<TicketDTO> ChangeTicketDepartmentAsync(int ticketId, int newDepartmentId, int requestedByUserId)
        {
            var requestingUser = await _context.Users.FindAsync(requestedByUserId);
            if (requestingUser == null)
            {
                throw new UnauthorizedAccessException("User not found");
            }

            bool canChange = requestingUser.Role == UserRole.Supervisor.ToString() ||
                             requestingUser.Role == UserRole.Admin.ToString();

            if (!canChange)
            {
                throw new UnauthorizedAccessException("You don't have permission to change ticket departments");
            }

            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null)
            {
                return null;
            }

            var targetDept = await _context.Departments.FindAsync(newDepartmentId);
            if (targetDept == null)
            {
                throw new InvalidOperationException("Target department does not exist");
            }

            if (ticket.AssignedToId.HasValue)
            {
                ticket.AssignedToId = null;
            }

            ticket.DepartmentId = newDepartmentId;
            await _context.SaveChangesAsync();

            return await GetTicketByIdAsync(ticketId, requestedByUserId);
        }

        // Helper methods
        private async Task<bool> CanUserAccessTicketAsync(Ticket ticket, int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            if (user.Role == UserRole.Admin.ToString())
            {
                return true;
            }

            if (ticket.DepartmentId == user.DepartmentId)
            {
                return true;
            }

            if (ticket.Severity == SeverityLevel.Critical.ToString())
            {
                if (user.Role == UserRole.Supervisor.ToString())
                {
                    return true;
                }

                if (ticket.AssignedToId == userId)
                {
                    return true;
                }
            }

            return false;
        }

        private TicketDTO MapToTicketDto(Ticket ticket)
        {
            return new TicketDTO
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                CreatedById = ticket.CreatedById,
                CreatedByName = ticket.CreatedBy?.Name ?? "Unknown",
                AssignedToId = ticket.AssignedToId,
                AssignedToName = ticket.AssignedTo?.Name,
                DepartmentID= ticket.DepartmentId,
                DepartmentName = ticket.Department?.Name ?? "Unknown",
                Severity = ticket.Severity,
                Status = ticket.Status,
                CreatedAt = ticket.CreatedAt,
                Remarks = ticket.Remarks?.Select(r => new RemarkDTO
                {
                    Id = r.Id,
                    TicketID = r.TicketId,
                    UserID = r.UserId,
                    UserName = r.User?.Name ?? "Unknown",
                    Content = r.Content,
                    CreatedAt = r.CreatedAt
                }).ToList() ?? new List<RemarkDTO>()
            };
        }

        public Task<IEnumerable<TicketDTO>> getTicketsForUserAsync(int userID)
        {
            throw new NotImplementedException();
        }

        public Task<TicketDTO> getTicketByIDAsync(int ticketID, int userID)
        {
            throw new NotImplementedException();
        }

        Task<TicketDTO> ITicketService.AssignTicketAsync(int ticketID, int assignToUserID, int requestedByUserID)
        {
            throw new NotImplementedException();
        }

        Task<TicketDTO> ITicketService.ChangeTicketStatusAsync(int ticketID, string newStatus, int userID)
        {
            throw new NotImplementedException();
        }

        Task<TicketDTO> ITicketService.ChangeTicketDepartmentAsync(int ticketID, int newDepartmentID, int requestedByID)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<TicketDTO>> ITicketService.getTicketsForUserAsync(int userID, string? userRole, int departmentID)
        {
            throw new NotImplementedException();
        }

        Task<TicketDTO> ITicketService.getTicketByIDAsync(int ticketID, int userID, string? userRole, int departmentID)
        {
            throw new NotImplementedException();
        }

        Task<TicketDTO> ITicketService.UpdateTicketAsync(int id, TicketUpdateDTO ticketDTO, int userID, string? userRole, int departmentID)
        {
            throw new NotImplementedException();
        }

        Task<TicketDTO> ITicketService.AssignTicketAsync(int ticketID, int assignToUserID, int requestedByUserID, string? userRole, int departmentID)
        {
            throw new NotImplementedException();
        }

        Task<TicketDTO> ITicketService.UpdateTicketStatusAsync(int id, TicketStatus newStatus, int userID, string? userRole, int departmentID)
        {
            throw new NotImplementedException();
        }

        Task<TicketDTO> ITicketService.ChangeTicketDepartmentAsync(int ticketID, int newDepartmentID, int requestedByID, string? userRole, int departmentID)
        {
            throw new NotImplementedException();
        }

        Task<TicketDTO> ITicketService.getDashboardDataAsync(int userID, string? userRole, int departmentID)
        {
            throw new NotImplementedException();
        }

        Task<bool> ITicketService.UserHasAccessToTicketAsync(int ticketID, int userID, string? userRole, int departmentID)
        {
            throw new NotImplementedException();
        }
    }
}