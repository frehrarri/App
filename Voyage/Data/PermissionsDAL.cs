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

                        IsEnabled = p.RolePermissions.Any(tp =>
                            tp.RoleKey == Guid.Parse(roleKey)
                            && tp.CompanyId == companyId)
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

        //public async Task<PermissionsDTO> GetPermissions()
        //{
        //    PermissionsDTO permissionsDTO = new PermissionsDTO();

        //    permissionsDTO.Permissions = await _db.Permissions.Select(p => new PermissionDTO()
        //    {
        //        PermissionKey = p.PermissionKey.ToString(),
        //        PermissionName = p.PermissionName,
        //    }).ToListAsync();

        //    return permissionsDTO;
        //}

        //public async Task<bool> HasPermissionAsync(string userKey, string permissionName)
        //{
        //    return await _db.Permissions
        //            .Where(p => p.PermissionName == permissionName)
        //            .AnyAsync(p =>
        //                p.UserPermissions.Any(up => up.AppUser.Id == userKey && !up.InheritIsDenied)

        //                || p.TeamPermissions.Any(tp => tp.Team.TeamUserRoles
        //                                    .Any(tur => tur.User.Id == userKey))

        //                || p.DepartmentPermissions.Any(dp => dp.Department.DepartmentUserRoles
        //                                          .Any(dur => dur.User.Id == userKey))

        //                || p.RolePermissions.Any(rp => rp.CompanyRole.IndividualUserRoles
        //                                    .Any(iur => iur.User.Id == userKey))
        //            );
        //}

        //public async Task<PermissionsDTO> GetAllPermissions(int companyId, string userKey)
        //{
        //    PermissionsDTO permissionsDTO = new PermissionsDTO();

        //permissionsDTO.Permissions = 
        //    await _db.Permissions
        //     .Select(p => new PermissionDTO
        //     {
        //        PermissionKey = p.PermissionKey.ToString(),
        //        PermissionName = p.PermissionName,

        //        IsEnabled = p.UserPermissions.Any(up => up.AppUser.Id == userKey)

        //            || p.TeamPermissions.Any(tp => tp.Team.TeamUserRoles.Any(tur => tur.User.Id == userKey))

        //            || p.DepartmentPermissions.Any(dp => dp.Department.DepartmentUserRoles.Any(dur => dur.User.Id == userKey))

        //            || p.RolePermissions.Any(rp => rp.CompanyRole.IndividualUserRoles.Any(iur => iur.User.Id == userKey)),

        //                // Determine first matching scope
        //         InheritsFrom = p.UserPermissions.Any(up => up.AppUser.Id == userKey) ? "User" :

        //            p.TeamPermissions.Any(tp => tp.Team.TeamUserRoles.Any(tur => tur.User.Id == userKey)) ? "Team" :

        //                p.DepartmentPermissions.Any(dp => dp.Department.DepartmentUserRoles.Any(dur => dur.User.Id == userKey)) ? "Department" :

        //                    p.RolePermissions.Any(rp => rp.CompanyRole.IndividualUserRoles.Any(iur => iur.User.Id == userKey)) ? "Role" : ""

        //     })
        //     .ToListAsync();

        //    return permissionsDTO;
        //}

        //public async Task GetDepartmentPermissions(string deptKey)
        //{
        //    await _db.Permissions
        //        .Select(p => new PermissionDTO
        //        {
        //            PermissionKey = p.PermissionKey.ToString(),
        //            PermissionName = p.PermissionName,

        //            IsEnabled = p.DepartmentPermissions
        //                .Any(tp => tp.DepartmentKey == Guid.Parse(deptKey))
        //        })
        //        .OrderBy(p => p.PermissionName)
        //        .ToListAsync();
        //}



        //public async Task GetUserPermissions(string userKey)
        //{
        //    await _db.Permissions
        //        .Select(p => new PermissionDTO
        //        {
        //            PermissionKey = p.PermissionKey.ToString(),
        //            PermissionName = p.PermissionName,

        //            IsEnabled = p.UserPermissions
        //                .Any(tp => tp.Id == userKey)
        //        })
        //        .OrderBy(p => p.PermissionName)
        //        .ToListAsync();
        //}
    }
}
