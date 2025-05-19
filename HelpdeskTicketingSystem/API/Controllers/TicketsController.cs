using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HelpdeskTicketingSystem.DTOs;
using HelpdeskTicketingSystem.Enums;
using HelpdeskTicketingSystem.Interfaces;

namespace HelpdeskApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTickets()
        {
            try
            {
                var userId = int.Parse(User.FindFirst("sub")?.Value);
                var userRole = User.FindFirst("role")?.Value;
                var departmentId = int.Parse(User.FindFirst("departmentId")?.Value);

                var tickets = await _ticketService.getTicketsForUserAsync(userId, userRole, departmentId);
                return Ok(tickets);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicket(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("sub")?.Value);
                var userRole = User.FindFirst("role")?.Value;
                var departmentId = int.Parse(User.FindFirst("departmentId")?.Value);

                var ticket = await _ticketService.getTicketByIDAsync(id, userId, userRole, departmentId);
                if (ticket == null)
                {
                    return NotFound(new { message = "Ticket not found or you don't have access to it" });
                }

                return Ok(ticket);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] TicketCreateDTO ticketDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("sub")?.Value);
                var userDepartmentId = int.Parse(User.FindFirst("departmentId")?.Value);

                // Set user ID as creator
                ticketDto.CreatedBy = userId;

                // If department not specified, use user's department
                if (ticketDto.DepartmentId <= 0)
                {
                    ticketDto.DepartmentId = userDepartmentId;
                }

                if (string.IsNullOrEmpty(ticketDto.Status))
                {
                    ticketDto.Status = TicketStatus.Open.ToString();
                }

                var ticket = await _ticketService.CreateTicketAsync(ticketDto, userId);
                return CreatedAtAction(nameof(GetTicket), new { id = ticket.Id }, ticket);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTicket(int id, [FromBody] TicketUpdateDTO ticketDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("sub")?.Value);
                var userRole = User.FindFirst("role")?.Value;
                var departmentId = int.Parse(User.FindFirst("departmentId")?.Value);

                var ticket = await _ticketService.UpdateTicketAsync(id, ticketDto, userId, userRole, departmentId);
                if (ticket == null)
                {
                    return NotFound(new { message = "Ticket not found" });
                }

                return Ok(ticket);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/assign")]
        [Authorize(Roles = "Admin,Supervisor")]
        public async Task<IActionResult> AssignTicket(int id, [FromBody] int assignToUserId)
        {
            try
            {
                // Get user ID from claims
                var userId = int.Parse(User.FindFirst("sub")?.Value);
                var userRole = User.FindFirst("role")?.Value;
                var departmentId = int.Parse(User.FindFirst("departmentId")?.Value);

                var ticket = await _ticketService.AssignTicketAsync(id, assignToUserId, userId, userRole, departmentId);
                if (ticket == null)
                {
                    return NotFound(new { message = "Ticket not found" });
                }

                return Ok(ticket);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateTicketStatus(int id, [FromBody] TicketStatus status)
        {
            try
            {
                // Get user ID from claims
                var userId = int.Parse(User.FindFirst("sub")?.Value);
                var userRole = User.FindFirst("role")?.Value;
                var departmentId = int.Parse(User.FindFirst("departmentId")?.Value);

                var ticket = await _ticketService.UpdateTicketStatusAsync(id, status, userId, userRole, departmentId);
                if (ticket == null)
                {
                    return NotFound(new { message = "Ticket not found" });
                }

                return Ok(ticket);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/department")]
        [Authorize(Roles = "Admin,Supervisor")]
        public async Task<IActionResult> ChangeDepartment(int id, [FromBody] int newDepartmentId)
        {
            try
            {
                // Get user ID from claims
                var userId = int.Parse(User.FindFirst("sub")?.Value);
                var userRole = User.FindFirst("role")?.Value;
                var departmentId = int.Parse(User.FindFirst("departmentId")?.Value);

                var ticket = await _ticketService.ChangeTicketDepartmentAsync(id, newDepartmentId, userId, userRole, departmentId);
                if (ticket == null)
                {
                    return NotFound(new { message = "Ticket not found" });
                }

                return Ok(ticket);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("dashboard")]
        [Authorize(Roles = "Admin,Supervisor")]
        public async Task<IActionResult> GetDashboardData()
        {
            try
            {
                var userId = int.Parse(User.FindFirst("sub")?.Value);
                var userRole = User.FindFirst("role")?.Value;
                var departmentId = int.Parse(User.FindFirst("departmentId")?.Value);

                var dashboardData = await _ticketService.getDashboardDataAsync(userId, userRole, departmentId);
                return Ok(dashboardData);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}