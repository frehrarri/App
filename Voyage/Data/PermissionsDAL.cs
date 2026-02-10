using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Voyage.Data.TableModels;
using Voyage.Models.DTO;
using Voyage.Utilities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Voyage.Data
{
    public class PermissionsDAL
    {
        private readonly _AppDbContext _db;
        private ILogger<PermissionsDAL> _logger;

        public PermissionsDAL(_AppDbContext db, ILogger<PermissionsDAL> logger) 
        { 
            _db = db;
            _logger = logger;
        }

        public async Task<PermissionsDTO> GetRolePermissions(int companyId, string roleKey)
        {
            try
            {
                PermissionsDTO permissionsDTO = new PermissionsDTO();

                permissionsDTO.Permissions = await _db.Permissions
                    .Where(p => p.RolePermissions.Any(rp => rp.CompanyId == companyId))
                    .Select(p => new PermissionDTO
                    {
                        PermissionKey = p.PermissionKey.ToString(),
                        PermissionName = p.PermissionName,

                        IsEnabled = p.RolePermissions.Where(tp => 
                            tp.RoleKey == Guid.Parse(roleKey) 
                            && tp.CompanyId == companyId)
                        .Select(rp => rp.IsEnabled)
                        .SingleOrDefault()
                    })
                    .OrderBy(p => p.PermissionName)
                    .ToListAsync();

                return permissionsDTO;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error: GetRolePermissions");
                return null!;
            }
        }


        public async Task SetDefaultRolePermissions(PermissionsDTO dto, IDbContextTransaction? tx = null)
        {
            bool inheritTx = false;

            if (tx != null)
                inheritTx = true;

            //transaction is not passed in
            if (!inheritTx)
                tx = await _db.Database.BeginTransactionAsync();

            try
            {
                List<Permission> permissions = await _db.Permissions.ToListAsync();

                foreach (var p in permissions)
                {
                    //do not add duplicates
                    var exists = await _db.RolePermissions.AnyAsync(rp => rp.PermissionKey == p.PermissionKey && rp.CompanyId == dto.CompanyId);
                    if (!exists)
                    {
                        await _db.RolePermissions.AddAsync(new RolePermissions
                        {
                            PermissionKey = p.PermissionKey,
                            RoleKey = Guid.Parse(dto.RoleKey),
                            CompanyId = dto.CompanyId,

                            //principal receives all permissions
                            //default to fully disabled permissions for any other role
                            IsEnabled = dto.RoleType == (int)Constants.DefaultRoles.Principal ? true : false
                        });
                    }
                    
                }

                await _db.SaveChangesAsync();

                if (!inheritTx)
                    await tx.CommitAsync();

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error: SetDefaultRolePermissions");

                if (!inheritTx)
                    await tx.RollbackAsync();

                return;
            }
        }

        public async Task SetRolePermissions(PermissionsDTO dto)
        {
            var existing = await _db.RolePermissions.Where(rp => rp.CompanyId == dto.CompanyId && rp.RoleKey == Guid.Parse(dto.RoleKey)).ToListAsync();

            if (existing.Any())
            {
                foreach (var permission in existing)
                {
                    var selected = dto.Permissions.Find(p => p.PermissionKey == permission.PermissionKey.ToString());
                    if (selected != null)
                        permission.IsEnabled = selected.IsEnabled;
                }
            }
            await _db.SaveChangesAsync();
        }


    }
}
