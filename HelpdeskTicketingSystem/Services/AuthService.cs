using HelpdeskApp.Infrastructure.Helpers;
using HelpdeskTicketingSystem.Data;
using HelpdeskTicketingSystem.DTOs;
using HelpdeskTicketingSystem.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace HelpdeskApp.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly JwtTokenGenerator _jwtTokenGenerator;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _jwtTokenGenerator = new JwtTokenGenerator(configuration);
        }

        public async Task<object> AuthenticateAsync(LoginDTO loginDto)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.ToLower() == loginDto.Email.ToLower());

            if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                return null;
            }

            // Generate JWT token
            var token = _jwtTokenGenerator.GenerateToken(user);

            return new
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                DepartmentId = user.DepartmentId,
                Token = token
            };
        }

        public Task<UserDTO> getCurrentUserAsync(int userID)
        {
            throw new NotImplementedException();
        }

        public async Task<object> GetUserByIdAsync(int userId)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return null;
            }

            return new
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                DepartmentId = user.DepartmentId,
                Department = user.Department?.Name
            };
        }

        public Task<UserDTO> getUserByIdAsync(int userID)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<object>> GetUsersByDepartmentAsync(int departmentId)
        {
            var users = await _context.Users
                .AsNoTracking()
                .Where(u => u.DepartmentId == departmentId)
                .Select(u => new
                {
                    UserId = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role
                })
                .ToListAsync();

            return users;
        }

        public Task<AuthResponseDTO> LoginAsync(LoginDTO loginDTO)
        {
            throw new NotImplementedException();
        }

        public Task<AuthResponseDTO> RegisterAsync(RegisterUserDTO registerDTO)
        {
            throw new NotImplementedException();
        }

        Task<AuthResponseDTO> IAuthService.AuthenticateAsync(LoginDTO loginDTO)
        {
            throw new NotImplementedException();
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            // Simple password verification for demo purposes
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var hashedPassword = Convert.ToBase64String(hashedBytes);

            return hashedPassword == storedHash;
        }
    }
}