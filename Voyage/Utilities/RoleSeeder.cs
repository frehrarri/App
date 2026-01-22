using Microsoft.AspNetCore.Identity;

namespace Voyage.Utilities
{
    public static class RoleSeeder
    {
        //public static async Task SeedRolesAsync(IServiceProvider services)
        //{
        //    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();

        //    foreach (var role in Enum.GetValues<Constants.DefaultRoles>())
        //    {
        //        if (!await roleManager.RoleExistsAsync(role.ToString()))
        //        {
        //            await roleManager.CreateAsync(new AppRole() 
        //            {
        //                Name = role.ToString(), 
        //                //CompanyId = HttpContext.Session.GetInt32("CompanyId") 
        //            });
        //        }
        //    }
        //}
    }
}
