using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Voyage.Data.TableModels;
using Voyage.Models.DTO;
using Voyage.Utilities;

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

        #region Get Methods

        public async Task<PermissionsDTO> GetDeptPermissions(int companyId, string deptKey)
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

                        IsEnabled = p.DepartmentPermissions.Where(tp =>
                            tp.DepartmentKey == Guid.Parse(deptKey)
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

        public async Task<PermissionsDTO> GetTeamPermissions(int companyId, string teamKey)
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

                        IsEnabled = p.TeamPermissions.Where(tp =>
                            tp.TeamKey == Guid.Parse(teamKey)
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

        public async Task<PermissionsDTO> GetUserPermissions(int companyId, string userKey)
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

                        IsEnabled = p.UserPermissions.Where(tp =>
                            tp.Id == userKey
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


        #endregion




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

        public async Task SetPermissions(PermissionsDTO dto)
        {
            dynamic existing = null!;

            if (!string.IsNullOrEmpty(dto.DepartmentKey))
                existing = await _db.DepartmentPermissions.Where(dp => dp.CompanyId == dto.CompanyId && dto.DepartmentKey == dp.DepartmentKey.ToString()).ToListAsync();

            else if (!string.IsNullOrEmpty(dto.RoleKey))
                existing = await _db.RolePermissions.Where(rp => rp.CompanyId == dto.CompanyId && rp.RoleKey == Guid.Parse(dto.RoleKey)).ToListAsync();

            else if (!string.IsNullOrEmpty(dto.TeamKey))
                existing = await _db.TeamPermissions.Where(rp => rp.CompanyId == dto.CompanyId && rp.TeamKey == Guid.Parse(dto.TeamKey)).ToListAsync();

            else if (!string.IsNullOrEmpty(dto.UserKey))
                existing = await _db.UserPermissions.Where(rp => rp.CompanyId == dto.CompanyId && rp.Id == dto.UserKey).ToListAsync();

            //update
            if (existing != null)
            {
                foreach (var permission in existing)
                {
                    var selected = dto.Permissions.Find(p => p.PermissionKey == permission.PermissionKey.ToString());

                    if (selected != null)
                        permission.IsEnabled = selected.IsEnabled;
                }
            }
            //add
            else
            {
                var permissions = await _db.Permissions.ToListAsync();
                foreach (var p in permissions)
                {
                    if (!string.IsNullOrEmpty(dto.DepartmentKey))
                    {
                        await _db.DepartmentPermissions.AddAsync(new DepartmentPermissions
                        {
                            PermissionKey = p.PermissionKey,
                            DepartmentKey = Guid.Parse(dto.DepartmentKey),
                            CompanyId = dto.CompanyId,

                            //default to fully disabled permissions on creation
                            IsEnabled = false
                        });
                    }

                    else if (!string.IsNullOrEmpty(dto.RoleKey))
                    {
                        await _db.RolePermissions.AddAsync(new RolePermissions
                        {
                            PermissionKey = p.PermissionKey,
                            RoleKey = Guid.Parse(dto.RoleKey),
                            CompanyId = dto.CompanyId,

                            //default to fully disabled permissions on creation
                            IsEnabled = false
                        });
                    }

                    else if (!string.IsNullOrEmpty(dto.TeamKey))
                    {
                        await _db.TeamPermissions.AddAsync(new TeamPermissions
                        {
                            PermissionKey = p.PermissionKey,
                            TeamKey = Guid.Parse(dto.TeamKey),
                            CompanyId = dto.CompanyId,

                            //default to fully disabled permissions on creation
                            IsEnabled = false
                        });
                    }

                    else if (!string.IsNullOrEmpty(dto.UserKey))
                    {
                        await _db.UserPermissions.AddAsync(new UserPermissions
                        {
                            PermissionKey = p.PermissionKey,
                            Id = dto.UserKey,
                            CompanyId = dto.CompanyId,

                            //default to fully disabled permissions on creation
                            IsEnabled = false
                        });
                    }
                }
            }

            await _db.SaveChangesAsync();
        }

 
    }
}
