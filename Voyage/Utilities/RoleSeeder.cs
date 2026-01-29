using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Voyage.Data;
using Voyage.Data.TableModels;

namespace Voyage.Utilities
{
    public class RoleSeeder
    {
        private _AppDbContext _db; 
               
        public RoleSeeder(_AppDbContext db)
        {
            _db = db;
        }

        public async Task CreateGlobalRoles()
        {
            int index = 1;
            foreach (var role in Enum.GetValues<Constants.DefaultRoles>())
            {
                var roleName = role.ToString();
                var roleId = Convert.ToInt32(role);

                var exists = await _db.CompanyRoles.AnyAsync(r => r.CompanyId == null && r.RoleName == roleName);
                if (!exists)
                {
                    var newRole = new CompanyRole
                    {
                        RoleKey = Guid.NewGuid(),
                        RoleVersion = 0.0M,
                        CompanyId = null,
                        RoleId = index++,
                        RoleName = roleName,
                        RoleDescription = $"Global {roleName} role",
                        IsActive = true,
                        IsLatest = true,
                    };

                    await _db.CompanyRoles.AddAsync(newRole);
                }
            }

            await _db.SaveChangesAsync();
        }
    }
}
