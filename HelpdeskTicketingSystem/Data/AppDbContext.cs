using HelpdeskTicketingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpdeskTicketingSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Remark> Remarks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Department)
                .WithMany(d => d.Users)
                .HasForeignKey(u => u.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.CreatedBy)
                .WithMany(u => u.CreatedTickets)
                .HasForeignKey(t => t.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.AssignedTo)
                .WithMany(u => u.AssignedTickets)
                .HasForeignKey(t => t.AssignedToId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Department)
                .WithMany(d => d.Tickets)
                .HasForeignKey(t => t.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Remark>()
                .HasOne(r => r.User)
                .WithMany(u => u.Remarks)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>().HasData(
                new Department { Id = 1, Name = "IT"},
                new Department { Id = 2, Name = "HR"},
                new Department { Id = 3, Name = "Finance"},
                new Department { Id = 4, Name = "Marketing"}
            );

            //DEFAULT PASSWORD FOR USERS
            string defaultPasswordHash = BCrypt.Net.BCrypt.HashPassword("P@ssw0rd1234");
            modelBuilder.Entity<User>().HasData(
                            new User { Id = 1, Name = "Admin User", Email = "admin@example.com", PasswordHash = defaultPasswordHash, Role = "Admin", DepartmentId = 1 },
                            new User { Id = 2, Name = "IT Supervisor", Email = "itsupervisor@example.com", PasswordHash = defaultPasswordHash, Role = "Supervisor", DepartmentId = 1 },
                            new User { Id = 3, Name = "HR Supervisor", Email = "hrsupervisor@example.com", PasswordHash = defaultPasswordHash, Role = "Supervisor", DepartmentId = 2 },
                            new User { Id = 4, Name = "IT Officer", Email = "itofficer@example.com", PasswordHash = defaultPasswordHash, Role = "Officer", DepartmentId = 1 },
                            new User { Id = 5, Name = "IT Junior Officer", Email = "itjunior@example.com", PasswordHash = defaultPasswordHash, Role = "Junior Officer", DepartmentId = 1 },
                            new User { Id = 6, Name = "HR Officer", Email = "hrofficer@example.com", PasswordHash = defaultPasswordHash, Role = "Officer", DepartmentId = 2 }
                        );

            modelBuilder.Entity<Ticket>().HasData(
                new Ticket
                {
                    Id = 1,
                    Title = "Email Not Working",
                    Description = "Cannot access my work email since this morning",
                    CreatedById = 6,
                    DepartmentId = 1,
                    Severity = "High",
                    Status = "Open",
                    CreatedAt = DateTime.UtcNow
                },
                new Ticket
                {
                    Id = 2,
                    Title = "New Employee Setup",
                    Description = "Need to setup workstation for new hire starting Monday",
                    CreatedById = 3,
                    DepartmentId = 1,
                    Severity = "Medium",
                    Status = "Open",
                    CreatedAt = DateTime.UtcNow
                },
                new Ticket
                {
                    Id = 3,
                    Title = "Server Down",
                    Description = "Production server is not responding",
                    CreatedById = 4,
                    AssignedToId = 2,
                    DepartmentId = 1,
                    Severity = "Critical",
                    Status = "In Progress",
                    CreatedAt = DateTime.UtcNow
                }
            );

            modelBuilder.Entity<Remark>().HasData(
                new Remark
                {
                    Id = 1,
                    TicketId = 3,
                    UserId = 2,
                    Content = "Investigating the issue. Initial analysis shows potential network problem.",
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}
