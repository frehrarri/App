using AngleSharp.Css;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Utilities.IO;
using System.ComponentModel.Design;
using System.Data;
using Voyage.Business;
using Voyage.Data.TableModels;
using Voyage.Models;
using Voyage.Models.DTO;
using Voyage.Utilities;
using static Voyage.Utilities.Constants;

namespace Voyage.Data
{
    public class HrDAL
    {
        private _AppDbContext _db;
        private ILogger<HrDAL> _logger;

        public HrDAL(_AppDbContext db, ILogger<HrDAL> logger)
        { 
            _db = db;
            _logger = logger;
        }

        public async Task<List<ManagePersonnelDTO>> GetPersonnel(int companyId)
        {
            try
            {
                return await _db.Users
                    .Where(u => u.CompanyId == companyId)
                    .Select(u => new ManagePersonnelDTO
                    {
                        //phone
                        EmployeeId = u.EmployeeId,
                        Id = u.Id,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Username = u.UserName!,
                        Email = u.Email!,
                        RoleId = u.IndividualUserRoles.Where(iur => iur.EmployeeId == u.EmployeeId).Select(iur => iur.RoleId).FirstOrDefault(),
                        Role = u.IndividualUserRoles.Where(iur => iur.EmployeeId == u.EmployeeId).Select(iur => iur.Role.RoleName).FirstOrDefault() ?? "",
                        IsUserActive = u.IsActiveUser
                    }).ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error: HrDAL : GetPersonnel");
                return null!;
            }
        }

        public async Task<List<ManageRolesDTO>> GetRoles(int? companyId)
        {
            try
            {
                return await _db.CompanyRoles
                  .Where(r => (r.CompanyId == companyId
                    || r.CompanyId == 0)
                    && r.IsLatest == true
                    && r.IsActive == true)
                  .Select(r => new ManageRolesDTO
                  {
                      Name = r.RoleName,
                      RoleId = r.RoleId,
                      CompanyId = r.CompanyId
                  }).ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error: HrDAL : GetRoles");
                return null!;
            }
        }

        public async Task<List<ManageDepartmentsDTO>> GetDepartments(int companyId)
        {
            try
            {
                return await _db.Departments
                    .Where(u => u.CompanyId == companyId)
                    .Select(u => new ManageDepartmentsDTO
                    {
                        Name = u.Name
                    }).ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error: HrDAL : GetDepartments");
                return null!;
            }
        }

        public async Task<List<TeamDTO>> GetTeams(int companyId)
        {
            try
            {
                return await _db.Teams
                    .Where(u => 
                        u.CompanyId == companyId 
                        && u.IsLatest == true)
                    .Select(u => new TeamDTO
                    {
                        TeamId = u.TeamId,
                        Name = u.Name
                    }).ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error: HrDAL : GetTeams");
                return null!;
            }
        }

        public async Task<List<AssignTeamDTO>> GetAssignTeam(int teamId, int companyId)
        {
            try
            {
                return await _db.TeamUserRoles
                    .Where(tur =>
                        tur.TeamId == teamId &&
                        tur.Team.CompanyId == companyId)
                    .Select(u => new AssignTeamDTO
                    {
                        //phone
                        EmployeeId = u.EmployeeId,
                        FirstName = u.User.FirstName,
                        LastName = u.User.LastName,
                        Username = u.User.UserName!,
                        Email = u.User.Email!,
                        RoleId = u.RoleId,
                        Role = u.Role.RoleName,
                        TeamName = u.Team.Name,
                        TeamId = u.TeamId
                    }).ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error: HrDAL : GetAssignTeam()");
                return null!;
            }
        }

        public async Task<List<ManagePermissionsDTO>> GetPermissions()
        {
            try
            {
                return await _db.Permissions
                    .Select(u => new ManagePermissionsDTO
                    {
                        Name = u.Name
                    }).ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error: HrDAL : GetPermissions");
                return null!;
            }
        }


        public async Task<bool> SavePersonnel(List<ManagePersonnelDTO> personnel, int companyId)
        {
            var datetime = DateTime.UtcNow;

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                foreach (var person in personnel)
                {
                    switch (person.DbSaveAction)
                    {         
                        case (int)SaveAction.Save:
                            var existingRole = await _db.IndividualUserRoles.FirstOrDefaultAsync(r => r.EmployeeId == person.EmployeeId && r.CompanyId == companyId);

                            if (existingRole != null)
                            {
                                existingRole.IsActive = person.IsUserActive;
                                existingRole.RoleId = person.RoleId;

                                var user = await _db.Users.FirstOrDefaultAsync(r => r.EmployeeId == person.EmployeeId && r.CompanyId == companyId);

                                if(user != null)
                                    user.IsActiveUser = person.IsUserActive;
                            }

                            break;

                        case (int)SaveAction.Remove:

                            var userToDelete = await _db.Users.FirstOrDefaultAsync(r => r.EmployeeId == person.EmployeeId
                                                                                        && r.CompanyId == companyId);
                            
                            if (userToDelete != null)
                                _db.Users.Remove(userToDelete);

                            break;
                    }
                }

                await _db.SaveChangesAsync();
                await tx.CommitAsync();
                return true;
            }
            catch (Exception e)
            {
                await tx.RollbackAsync();
                _logger.LogError(e, "Error: HrDAL.SaveRoles()");
                return false;
            }

        }

        public async Task<bool> SaveRoles(List<ManageRolesDTO> roles, int companyId)
        {
            var datetime = DateTime.UtcNow;

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                foreach (var role in roles)
                {
                    switch (role.DbChangeAction)
                    {
                        case (int)SaveAction.Save:
                            var existingRole = await _db.CompanyRoles.FirstOrDefaultAsync(r => r.RoleId == role.RoleId
                                                                                    && r.CompanyId == companyId);
                            // Update existing record
                            if (existingRole != null)
                            {
                                existingRole.RoleName = role.Name;
                                existingRole.ModifiedDate = datetime;
                                existingRole.ModifiedBy = role.CreatedBy;
                            }
                            // Add new role
                            else
                            {
                                int nextRoleId = await GetNextRoleId(companyId);

                                var newRole = new CompanyRole
                                {
                                    RoleId = nextRoleId,
                                    RoleName = role.Name,
                                    CompanyId = companyId,
                                    IsLatest = true,
                                    IsActive = true,
                                    CreatedDate = datetime,
                                    CreatedBy = role.CreatedBy
                                };

                                await _db.CompanyRoles.AddAsync(newRole);
                            }
                            break;

                        case (int)SaveAction.Remove:
                            var roleToDelete = await _db.CompanyRoles.FirstOrDefaultAsync(r => r.RoleId == role.RoleId
                                                                                        && r.CompanyId == companyId);

                            if (roleToDelete != null)
                            {
                                _db.CompanyRoles.Remove(roleToDelete);
                            }
                            break;
                    }
                }

                await _db.SaveChangesAsync();
                await tx.CommitAsync();
                return true;
            }
            catch (Exception e)
            {
                await tx.RollbackAsync();
                _logger.LogError(e, "Error: HrDAL.SaveRoles()");
                return false;
            }
        }


        public async Task SaveDepartments(List<DepartmentDTO> departments, int companyId)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Make old departments inactive because we will create new versions
                await _db.Departments
                    .Where(d => d.CompanyId == companyId
                             && d.IsLatest == true
                             && d.IsActive == true)
                    .ExecuteUpdateAsync(setters =>
                        setters.SetProperty(d => d.IsLatest, false));

                // Get the latest departments for comparison
                var latestDepartments = await _db.Departments
                    .AsNoTracking()
                    .Where(d => d.CompanyId == companyId
                             && d.IsLatest == true
                             && d.IsActive == true)
                    .ToListAsync();

                int nextDepartmentId = await GetNextDepartmentId(companyId);

                var deptsToSave = new List<Department>();

                if (departments != null && departments.Any())
                {
                    foreach (var deptDto in departments)
                    {
                        // Check if this department already existed
                        var existing = latestDepartments.FirstOrDefault(d => d.Name == deptDto.Name);

                        if (existing != null)
                        {
                            // Existing: create new version
                            deptsToSave.Add(new Department
                            {
                                DepartmentId = existing.DepartmentId,
                                Name = deptDto.Name,
                                CompanyId = companyId,
                                IsLatest = true,
                                IsActive = true,
                                CreatedDate = existing.CreatedDate,
                                ModifiedDate = DateTime.UtcNow
                            });
                        }
                        else
                        {
                            // New department
                            deptsToSave.Add(new Department
                            {
                                DepartmentId = nextDepartmentId,
                                Name = deptDto.Name,
                                CompanyId = companyId,
                                IsLatest = true,
                                IsActive = true,
                                CreatedDate = DateTime.UtcNow,
                                ModifiedDate = DateTime.UtcNow
                            });

                            nextDepartmentId++;
                        }
                    }
                }

                // Handle deletions: departments that existed before but are missing now
                var deptNamesToKeep = departments?.Select(d => d.Name).ToList() ?? new List<string>();
                var deptsToDelete = latestDepartments
                    .Where(d => !deptNamesToKeep.Contains(d.Name));

                foreach (var deptToDelete in deptsToDelete)
                {
                    deptsToSave.Add(new Department
                    {
                        DepartmentId = deptToDelete.DepartmentId,
                        Name = deptToDelete.Name,
                        CompanyId = companyId,
                        IsLatest = true,
                        IsActive = false, // mark as deleted
                        CreatedDate = deptToDelete.CreatedDate,
                        ModifiedDate = DateTime.UtcNow
                    });
                }

                if (deptsToSave.Any())
                {
                    await _db.Departments.AddRangeAsync(deptsToSave);
                    await _db.SaveChangesAsync();
                }

                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                _logger.LogError(e, "Error: HrDAL : SaveDepartments");
            }
        }

        private async Task<int> GetNextDepartmentId(int companyId)
        {
            var maxDeptId = await _db.Departments
                .Where(d => d.CompanyId == companyId)
                .MaxAsync(d => (int?)d.DepartmentId) ?? 0;

            return maxDeptId + 1;
        }

        private async Task<int> GetNextRoleId(int companyId)
        {
            var maxRolesId = await _db.CompanyRoles
                .Where(d => d.CompanyId == companyId)
                .MaxAsync(d => (int?)d.RoleId) ?? 0;

            return maxRolesId + 1;
        }

     

        public async Task SavePermissions(List<string> permissions)
        {
            try
            {
                List<Permissions> permissionsToSave = permissions.Select(p => new Permissions() { Name = p }).ToList();

                if (permissionsToSave.Any())
                {
                    await _db.Permissions.AddRangeAsync(permissionsToSave);
                }

                await _db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error: HrDAL : SavePermissions");
            }
        }

        //public async Task AssignTeam()
        //{
        //    try
        //    {

        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError(e, "Error: HrDAL : AssignTeam");
        //    }
        //}

        public async Task SaveTeams(List<TeamDTO> teams, int companyId)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Make old teams not latest because we will create new versions
                await _db.Teams
                    .Where(t => t.CompanyId == companyId
                             && t.IsLatest == true
                             && t.IsActive == true)
                    .ExecuteUpdateAsync(setters =>
                        setters.SetProperty(t => t.IsLatest, false));

                // Get latest teams for comparison (no tracking to avoid conflicts)
                var latestTeams = await _db.Teams
                    .AsNoTracking()
                    .Where(t => t.CompanyId == companyId
                             && t.IsLatest == true
                             && t.IsActive == true)
                    .ToListAsync();

                int nextTeamId = await GetNextTeamId(companyId);

                var teamsToSave = new List<Team>();

                if (teams != null && teams.Any())
                {
                    foreach (var teamDto in teams)
                    {
                        // Check if this team already existed
                        var existing = latestTeams.FirstOrDefault(t => t.Name == teamDto.Name);

                        if (existing != null)
                        {
                            // Existing team: create new version
                            teamsToSave.Add(new Team
                            {
                                TeamId = existing.TeamId,
                                Name = teamDto.Name,
                                CompanyId = companyId,
                                IsLatest = true,
                                IsActive = true,
                                CreatedDate = existing.CreatedDate,
                                ModifiedDate = DateTime.UtcNow
                            });
                        }
                        else
                        {
                            // New team
                            teamsToSave.Add(new Team
                            {
                                TeamId = nextTeamId,
                                Name = teamDto.Name,
                                CompanyId = companyId,
                                IsLatest = true,
                                IsActive = true,
                                CreatedDate = DateTime.UtcNow,
                                ModifiedDate = DateTime.UtcNow
                            });

                            nextTeamId++;
                        }
                    }
                }

                // Handle deletions: teams that existed before but are missing now
                var teamNamesToKeep = teams?.Select(t => t.Name).ToList() ?? new List<string>();
                var teamsToDelete = latestTeams
                    .Where(t => !teamNamesToKeep.Contains(t.Name));

                foreach (var teamToDelete in teamsToDelete)
                {
                    teamsToSave.Add(new Team
                    {
                        TeamId = teamToDelete.TeamId,
                        Name = teamToDelete.Name,
                        CompanyId = companyId,
                        IsLatest = true,
                        IsActive = false, // mark as deleted
                        CreatedDate = teamToDelete.CreatedDate,
                        ModifiedDate = DateTime.UtcNow
                    });
                }

                // Save all new team versions
                if (teamsToSave.Any())
                {
                    await _db.Teams.AddRangeAsync(teamsToSave);
                    await _db.SaveChangesAsync();
                }

                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                _logger.LogError(e, "Error: HrDAL : SaveTeams");
            }
        }


        private async Task<int> GetNextTeamId(int companyId)
        {
            var maxTeamId = await _db.Teams
                .Where(t => t.CompanyId == companyId)
                .MaxAsync(t => (int?)t.TeamId) ?? 0;

            return maxTeamId + 1;
        }


        public async Task AssignTeamMembers(List<AssignTeamDTO> dto, int companyId, string teamKey)
        {
            var teamGuid = Guid.Parse(teamKey);

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var existing = await _db.TeamUserRoles
                    .Where(tur =>
                        tur.CompanyId == companyId &&
                        tur.TeamKey == teamGuid)
                    .ToListAsync();

                //remove
                var removeIds = dto
                    .Where(d => d.SaveAction == (int)Constants.SaveAction.Remove)
                    .Select(d => d.EmployeeId)
                    .ToHashSet();

                var toRemove = existing
                    .Where(e => removeIds.Contains(e.EmployeeId))
                    .ToList();

                _db.TeamUserRoles.RemoveRange(toRemove);

                //add
                var existingEmployeeIds = existing
                    .Select(e => e.EmployeeId)
                    .ToHashSet();

                var toAdd = dto
                    .Where(d => d.SaveAction == (int)Constants.SaveAction.Save)
                    .Where(d => !existingEmployeeIds.Contains(d.EmployeeId));

                foreach (var item in toAdd)
                {
                    _db.TeamUserRoles.Add(new TeamUserRole
                    {
                        TeamKey = teamGuid,
                        CompanyId = companyId,
                        EmployeeId = item.EmployeeId,
                        RoleId = item.RoleId
                    });
                }

                //update
                //var toUpdate = dto
                //    .Where(d => d.SaveAction == (int)Constants.SaveAction.Update);

                //foreach (var item in toUpdate)
                //{
                //    var existingRow = existing
                //        .FirstOrDefault(e => e.EmployeeId == item.EmployeeId);

                //    if (existingRow != null &&
                //        existingRow.RoleId != item.RoleId)
                //    {
                //        existingRow.RoleId = item.RoleId;
                //    }
                //}

                await _db.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Error: HrDAL.AssignTeamMembers()");
            }
        }

    }
}
