using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HelpdeskTicketingSystem.DTOs;
using HelpdeskTicketingSystem.Interfaces;

namespace HelpdeskApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RemarksController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly IRemarkService _remarkService;

        public RemarksController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet("ticket/{ticketId}")]
        public async Task<IActionResult> GetRemarksByTicketId(int ticketId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("sub")?.Value);
                var userRole = User.FindFirst("role")?.Value;
                var departmentId = int.Parse(User.FindFirst("departmentId")?.Value);

                var hasAccess = await _ticketService.UserHasAccessToTicketAsync(ticketId, userId, userRole, departmentId);
                if (!hasAccess)
                {
                    return Forbid("You don't have access to this ticket");
                }

                var remarks = await _remarkService.GetRemarksByTicketIDAsync(ticketId, userId);
                return Ok(remarks);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddRemark([FromBody] RemarkDTO remarkDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("sub")?.Value);
                var userRole = User.FindFirst("role")?.Value;
                var departmentId = int.Parse(User.FindFirst("departmentId")?.Value);

                remarkDto.UserID = userId;

                var remark = await _remarkService.AddRemarkAsync(remarkDto, userId, userRole, departmentId);
                return CreatedAtAction(nameof(GetRemarksByTicketId), new { ticketId = remarkDto.TicketID }, remark);
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRemarkById(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("sub")?.Value);
                var userRole = User.FindFirst("role")?.Value;
                var departmentId = int.Parse(User.FindFirst("departmentId")?.Value);

                var remark = await _remarkService.GetRemarkByIdAsync(id, userId, userRole, departmentId);
                if (remark == null)
                {
                    return NotFound(new { message = "Remark not found or you don't have access to it" });
                }

                return Ok(remark);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRemark(int id)
        {
            try
            {
                // Get user ID from claims
                var userId = int.Parse(User.FindFirst("sub")?.Value);
                var userRole = User.FindFirst("role")?.Value;
                var departmentId = int.Parse(User.FindFirst("departmentId")?.Value);

                // Only allow users to delete their own remarks, or supervisors/admins
                var result = await _remarkService.DeleteRemarkAsync(id, userId, userRole, departmentId);
                if (!result)
                {
                    return NotFound(new { message = "Remark not found or you don't have permission to delete it" });
                }

                return NoContent();
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
    }
}