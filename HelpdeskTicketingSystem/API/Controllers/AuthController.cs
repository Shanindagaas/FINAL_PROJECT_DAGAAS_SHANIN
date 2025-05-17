using HelpdeskTicketingSystem.DTOs;
using HelpdeskTicketingSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpdeskTicketingSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                var result = await _authService.AuthenticateAsync(loginDTO);
                if (result == null)
                {
                    return Unauthorized(new { message = "Invalid email or password " });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> getCurrentUser()
        {
            var userID = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userID)) { return Unauthorized(new { message = "Invalid token" }); }

            try
            {
                var user = await _authService.getUserByIdAsync(int.Parse(userID));
                if (user == null)
                {
                    return NotFound(new { message = "User not found " });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
