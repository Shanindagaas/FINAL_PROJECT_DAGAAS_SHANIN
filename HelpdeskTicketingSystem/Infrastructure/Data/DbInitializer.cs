using HelpdeskTicketingSystem.Data;
using HelpdeskTicketingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace HelpdeskApp.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(AppDbContext context)
        {
            // Make sure database is created
            await context.Database.EnsureCreatedAsync();

            // Seed departments if they don't exist
            if (!await context.Departments.AnyAsync())
            {
                await SeedDepartmentsAsync(context);
            }

            // Seed users if they don't exist
            if (!await context.Users.AnyAsync())
            {
                await SeedUsersAsync(context);
            }

            // Seed sample tickets if they don't exist
            if (!await context.Tickets.AnyAsync())
            {
                await SeedTicketsAsync(context);
            }
        }

        private static async Task SeedDepartmentsAsync(AppDbContext context)
        {
            var departments = new List<Department>
            {
                new Department { Id = 1, Name = "IT" },
                new Department { Id = 2, Name = "HR" },
                new Department { Id = 3, Name = "Finance" },
                new Department { Id = 4, Name = "Customer Support" }
            };

            await context.Departments.AddRangeAsync(departments);
            await context.SaveChangesAsync();
        }

        private static async Task SeedUsersAsync(AppDbContext context)
        {
            var users = new List<User>
            {
                new User
                {
                    Id = 1,
                    Name = "Admin User",
                    Email = "admin@helpdesk.com",
                    PasswordHash = HashPassword("admin123"),
                    Role = "Admin",
                    DepartmentId = 1
                },
                new User
                {
                    Id = 2,
                    Name = "IT Supervisor",
                    Email = "it.supervisor@helpdesk.com",
                    PasswordHash = HashPassword("supervisor123"),
                    Role = "Supervisor",
                    DepartmentId = 1
                },
                new User
                {
                    Id = 3,
                    Name = "HR Supervisor",
                    Email = "hr.supervisor@helpdesk.com",
                    PasswordHash = HashPassword("supervisor123"),
                    Role = "Supervisor",
                    DepartmentId = 2
                },
                new User
                {
                    Id = 4,
                    Name = "IT Officer",
                    Email = "it.officer@helpdesk.com",
                    PasswordHash = HashPassword("officer123"),
                    Role = "Officer",
                    DepartmentId = 1
                },
                new User
                {
                    Id = 5,
                    Name = "IT Junior Officer",
                    Email = "it.junior@helpdesk.com",
                    PasswordHash = HashPassword("junior123"),
                    Role = "Junior Officer",
                    DepartmentId = 1
                },
                new User
                {
                    Id = 6,
                    Name = "HR Officer",
                    Email = "hr.officer@helpdesk.com",
                    PasswordHash = HashPassword("officer123"),
                    Role = "Officer",
                    DepartmentId = 2
                },
                new User
                {
                    Id = 7,
                    Name = "Finance Officer",
                    Email = "finance.officer@helpdesk.com",
                    PasswordHash = HashPassword("officer123"),
                    Role = "Officer",
                    DepartmentId = 3
                }
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
        }

        private static async Task SeedTicketsAsync(AppDbContext context)
        {
            var tickets = new List<Ticket>
            {
                new Ticket
                {
                    Id = 1,
                    Title = "Network Outage in HR Department",
                    Description = "The entire HR department is experiencing network connectivity issues.",
                    CreatedById = 3, // HR Supervisor
                    AssignedToId = 4, // IT Officer
                    DepartmentId = 1, // IT Department
                    Severity = "High",
                    Status = "In Progress",
                    CreatedAt = DateTime.Now.AddDays(-2)
                },
                new Ticket
                {
                    Id = 2,
                    Title = "Email not working",
                    Description = "Unable to send or receive emails since this morning.",
                    CreatedById = 6, // HR Officer
                    AssignedToId = 5, // IT Junior Officer
                    DepartmentId = 1, // IT Department
                    Severity = "Medium",
                    Status = "Open",
                    CreatedAt = DateTime.Now.AddDays(-1)
                },
                new Ticket
                {
                    Id = 3,
                    Title = "Payroll System Down",
                    Description = "The payroll system is not accessible. This is critical as we need to process payments today.",
                    CreatedById = 7, // Finance Officer
                    AssignedToId = 2, // IT Supervisor
                    DepartmentId = 1, // IT Department
                    Severity = "Critical",
                    Status = "Open",
                    CreatedAt = DateTime.Now.AddHours(-5)
                },
                new Ticket
                {
                    Id = 4,
                    Title = "New Employee Onboarding",
                    Description = "Need to set up new employee accounts and equipment for start date next Monday.",
                    CreatedById = 3, // HR Supervisor
                    AssignedToId = null, // Not assigned yet
                    DepartmentId = 2, // HR Department
                    Severity = "Low",
                    Status = "Open",
                    CreatedAt = DateTime.Now.AddDays(-3)
                }
            };

            await context.Tickets.AddRangeAsync(tickets);
            await context.SaveChangesAsync();

            // Add some remarks
            var remarks = new List<Remark>
            {
                new Remark
                {
                    Id = 1,
                    TicketId = 1,
                    UserId = 4, // IT Officer
                    Content = "Investigating the issue. Initial diagnosis suggests a switch failure.",
                    CreatedAt = DateTime.Now.AddDays(-2).AddHours(1)
                },
                new Remark
                {
                    Id = 2,
                    TicketId = 1,
                    UserId = 4, // IT Officer
                    Content = "Replaced faulty switch. Testing connectivity now.",
                    CreatedAt = DateTime.Now.AddDays(-2).AddHours(3)
                },
                new Remark
                {
                    Id = 3,
                    TicketId = 3,
                    UserId = 2, // IT Supervisor
                    Content = "This is being given highest priority. Team is working on it now.",
                    CreatedAt = DateTime.Now.AddHours(-4)
                }
            };

            await context.Remarks.AddRangeAsync(remarks);
            await context.SaveChangesAsync();
        }

        private static string HashPassword(string password)
        {
            // Simple password hashing for demo purposes
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}