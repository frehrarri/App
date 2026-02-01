using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Voyage.Data;
using Voyage.Data.TableModels;

namespace Voyage.Utilities
{
    public class RoleSeeder
    {
        private _AppDbContext _db;
        private const int COMPANYID_SEED = 0;
               
        public RoleSeeder(_AppDbContext db)
        {
            _db = db;
        }

        public async Task CreateGlobalRoles()
        {
            if (!await _db.Companies.AnyAsync(c => c.CompanyId == COMPANYID_SEED))
            {
                _db.Companies.Add(new Company
                {
                    CompanyId = 0,
                    Name = "SYSTEM",
                    CreatedBy = "SYSTEM",
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true,
                    IsLatest = true
                });
            }

            await _db.SaveChangesAsync();

            foreach (var role in Enum.GetValues<Constants.DefaultRoles>())
            {
                var roleName = role.ToString();
                var roleId = Convert.ToInt32(role);

                var exists = await _db.CompanyRoles.AnyAsync(r => r.CompanyId == COMPANYID_SEED && r.RoleName == roleName);
                if (!exists)
                {
                    var newRole = new CompanyRole
                    {
                        RoleKey = Guid.NewGuid(),
                        CompanyId = COMPANYID_SEED,
                        RoleId = Convert.ToInt32(role),
                        RoleName = roleName,
                        RoleDescription = $"Global {roleName} role",
                        CreatedBy = "SYSTEM",
                        CreatedDate = DateTime.UtcNow,
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
