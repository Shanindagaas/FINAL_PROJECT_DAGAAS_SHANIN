using HelpdeskTicketingSystem.Data;
using HelpdeskTicketingSystem.DTOs;
using HelpdeskTicketingSystem.Enums;
using HelpdeskTicketingSystem.Interfaces;
using HelpdeskTicketingSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace HelpdeskTicketingSystem.Services
{
    public class RemarkService : IRemarkService
    {
        private readonly AppDbContext _context;
        
        public RemarkService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RemarkDTO> addRemarkAsync(RemarkCreateDTO remarkDTO, int userID)
        {
            var user = await _context.Users.FindAsync(userID);
            if (user == null) { throw new UnauthorizedAccessException("User not found"); }

            var ticket = await _context.Tickets.FindAsync(remarkDTO.TicketID);
            if (ticket == null) { throw new InvalidOperationException("Ticket not found"); }

            bool canAccess = ticket.DepartmentId == user.DepartmentId;

            if (!canAccess && ticket.Severity == SeverityLevel.Critical.ToString())
            {
                canAccess = user.Role == UserRole.Supervisor.ToString() ||
                            user.Role == UserRole.Admin.ToString() ||
                            ticket.AssignedToId == userID;
            }

            if (!canAccess) { throw new UnauthorizedAccessException("You don't have access to this ticket!"); }

            if (ticket.Severity == SeverityLevel.Critical.ToString() &&
                user.Role == UserRole.JuniorOfficer.ToString() &&
                ticket.AssignedToId == userID) { throw new UnauthorizedAccessException("Junior Officers can't handle critical tickets unless assigned to"); }

            var remark = new Remark
            {
                TicketId = remarkDTO.TicketID,
                UserId = userID,
                Content = remarkDTO.Content,
                CreatedAt = DateTime.UtcNow,
            };

            _context.Remarks.Add(remark);
            await _context.SaveChangesAsync();

            return new RemarkDTO
            {
                Id = remark.Id,
                TicketID = remark.TicketId,
                UserID = remark.UserId,
                Content = remark.Content,
                CreatedAt = remark.CreatedAt
            };
        }

        public Task<RemarkDTO> AddRemarkAsync(RemarkCreateDTO remarkDTO, int userID)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<RemarkDTO>> GetRemarksByTicketIdAsync(int ticketId, int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found");
            }

            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null)
            {
                throw new InvalidOperationException("Ticket not found");
            }

            // Check if user can access this ticket
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

            var remarks = await _context.Remarks
                .Include(r => r.User)
                .Where(r => r.TicketId == ticketId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return remarks.Select(r => new RemarkDTO
            {
                Id = r.Id,
                TicketID = r.TicketId,
                UserID = r.UserId,
                UserName = r.User?.Name ?? "Unknown",
                Content = r.Content,
                CreatedAt = r.CreatedAt
            });
        }

        public Task<IEnumerable<RemarkDTO>> GetRemarksByTicketIDAsync(int ticketID, int userID)
        {
            throw new NotImplementedException();
        }

        Task<RemarkDTO> IRemarkService.AddRemarkAsync(RemarkDTO remarkDTO, int userID, string? userRole, int departmentID)
        {
            throw new NotImplementedException();
        }

        Task<bool> IRemarkService.DeleteRemarkAsync(int ticketID, int userID, string? userRole, int departmentID)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<RemarkDTO>> IRemarkService.GetRemarkByIdAsync(int ticketID, int userID, string? userRole, int departmentID)
        {
            throw new NotImplementedException();
        }
    }
 }
