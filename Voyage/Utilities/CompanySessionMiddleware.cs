using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Voyage.Data;
using Voyage.Data.TableModels;

namespace Voyage.Utilities
{
    public class CompanySessionMiddleware
    {
        private readonly RequestDelegate _next;

        public CompanySessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            UserManager<AppUser> userManager,
             _AppDbContext _db)
        {
            // Only for authenticated users
            if (context.User.Identity?.IsAuthenticated == true)
            {
                // Avoid resetting session every request
                if (!context.Session.Keys.Contains("CompanyId"))
                {
                    var user = await userManager.GetUserAsync(context.User);

                    if (user?.CompanyId != null)
                    {
                        var userRole = await _db.IndividualUserRoles
                            .Include(ur => ur.Role)
                            .Where(ur => ur.EmployeeId == user.EmployeeId
                                         && ur.CompanyId == user.CompanyId)
                            .FirstOrDefaultAsync();

                        if (userRole != null)
                            context.Session.SetInt32("RoleId", userRole.Role.RoleId);

                        context.Session.SetInt32("CompanyId", user.CompanyId);
                        context.Session.SetInt32("EmployeeId", user.EmployeeId);
                    }
                }
            }

            await _next(context);
        }
    }

}
