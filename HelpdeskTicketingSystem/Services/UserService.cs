using HelpdeskTicketingSystem.Data;
using HelpdeskTicketingSystem.DTOs;
using HelpdeskTicketingSystem.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelpdeskTicketingSystem.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        
        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserDTO>> getUsersInDepartmentAsync(int departmentID)
        {
            var users = await _context.Users
                .Include(u => u.Department)
                .Where(u => u.DepartmentId == departmentID)
                .ToListAsync();

            return users.Select(u => new UserDTO
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role,
                DepartmentID = u.DepartmentId,
                DepartmentName = u.Department?.Name ?? "Unknown"
            });
        }

        public async Task<UserDTO> getUserByIDAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return null;
            }

            return new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                DepartmentID = user.DepartmentId,
                DepartmentName = user.Department?.Name ?? "Unknown"
            };
        }

        public async Task<bool> isUserInDepartmentAsync(int userID, int departmentID)
        {
            var user = await _context.Users.FindAsync(userID);
            return user != null && user.DepartmentId == departmentID;
        }

        Task<IEnumerable<UserDTO>> IUserService.GetUsersInDepartmentAsync(int departmentID)
        {
            throw new NotImplementedException();
        }

        Task<UserDTO> IUserService.GetUsersByIDAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
