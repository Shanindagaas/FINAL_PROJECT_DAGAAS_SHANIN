using HelpdeskTicketingSystem.Models;

namespace HelpdeskTicketingSystem.Interfaces
{
    public interface IDepartmentServices
    {
        Task<IEnumerable<Department>> GetAllDepartmentsAsync();
        Task<Department> GetDepartmentByIDAsync(int id);
        Task<bool> isSupervisorAsync(int userID, int departmentID);
    }
}
