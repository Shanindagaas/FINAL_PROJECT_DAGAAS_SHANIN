using System.Net.NetworkInformation;
using HelpdeskTicketingSystem.Interfaces;

namespace HelpdeskTicketingSystem.API.Middleware
{
    public class RoleAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public RoleAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        
        public async Task InvokeAsync(HttpContext context, IAuthService authService)
        {
            if (!context.Request.Path.Value.StartsWith("/api") ||
                context.Request.Path.Value.StartsWith("/api/auth/login"))
            {
                await _next(context);
                return;
            }

            if (!context.User.Identity.IsAuthenticated)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new { message = "Authentication required" });
                return;
            }

            var userIdClaim = context.User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new { message = "Invalid token" });
                return;
            }

            var roleClaim = context.User.FindFirst("role")?.Value;
            if (string.IsNullOrEmpty(roleClaim))
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsJsonAsync(new { message = "Role information missing" });
                return;
            }

            var departmentIdClaim = context.User.FindFirst("departmentId")?.Value;
            if (string.IsNullOrEmpty(departmentIdClaim) || !int.TryParse(departmentIdClaim, out int departmentId))
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsJsonAsync(new { message = "Department information missing" });
                return;
            }

            var user = await authService.getUserByIdAsync(userId);
            if (user == null || user.Role != roleClaim || user.DepartmentID != departmentId)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsJsonAsync(new { message = "Invalid user role or department" });
                return;
            }

            await _next(context);
        }
    }

    public static class RoleAuthorizationMiddlewareExtensions
    {
        public static IApplicationBuilder UseRoleAuthorization(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RoleAuthorizationMiddleware>();
        }
    }
}
