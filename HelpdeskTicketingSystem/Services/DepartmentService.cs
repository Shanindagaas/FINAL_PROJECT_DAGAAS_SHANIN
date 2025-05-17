using HelpdeskTicketingSystem.Data;
using HelpdeskTicketingSystem.Enums;
using HelpdeskTicketingSystem.Interfaces;
using HelpdeskTicketingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpdeskTicketingSystem.Services
{
    public class DepartmentService : IDepartmentServices
    {
        private readonly AppDbContext _context;

        public DepartmentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Department>> getAllDepartmentsAsync()
        {
            return await _context.Departments.ToListAsync();
        }

        public async Task<Department> getDepartmentByIDAsync(int id)
        {
            return await _context.Departments.FindAsync(id);
        }

        public async Task<bool> isSupervisorAsync(int userID, int departmentID)
        {
            var user = await _context.Users.FindAsync(userID);
            if (user == null)
            {
                return false;
            }

            return user.DepartmentId == departmentID &&
                (user.Role == UserRole.Supervisor.ToString() ||
                user.Role == UserRole.Admin.ToString());
        }

        Task<IEnumerable<Department>> IDepartmentServices.GetAllDepartmentsAsync()
        {
            throw new NotImplementedException();
        }

        Task<Department> IDepartmentServices.GetDepartmentByIDAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
