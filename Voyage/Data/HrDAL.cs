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
                        Role = u.IndividualUserRoles.Where(iur => iur.EmployeeId == u.EmployeeId).Select(iur => iur.Role.RoleName).FirstOrDefault() ?? ""
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
                  .Where(r => r.CompanyId == companyId
                    || r.CompanyId == null
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

        //public async Task<List<UserDTO>> GetAvailableUsers(int companyId)
        //{
        //    try
        //    {
        //        var users = await _db.Users
        //            .Where(u => u.CompanyId == companyId
        //                && (u.IndividualUserRoles.Any(r => r.RoleId == (int)Constants.DefaultRoles.Unassigned)
        //                    || u.TeamUserRoles.Any(r => r.RoleId == (int)Constants.DefaultRoles.Unassigned)
        //                    || u.DepartmentUserRoles.Any(r => r.RoleId == (int)Constants.DefaultRoles.Unassigned)))
        //            .ToListAsync();
        //            //.Select(u => new UserDTO()
        //            //{

        //            //}
                   
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError(e, "Error: HrDAL : GetTeams");
        //        return null!;
        //    }
        //}

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

        public async Task<List<TeamMemberDTO>> GetTeamMembers(int companyId)
        {
            try
            {
                return new List<TeamMemberDTO>();
                //return await _db.TeamMembers
                //    .Where(tm => tm.CompanyId == companyId)
                //    .Select(tm => new TeamMemberDTO
                //    {
                //        //TeamId = tm.TeamId,
                //        TeamName = tm.Team.Name,
                //        EmployeeId = tm.EmployeeId, 
                //        Username = tm.User.UserName,
                //        FirstName = tm.User.FirstName,
                //        LastName = tm.User.LastName,
                //        Email = tm.User.Email,
                //        PhoneNumber = tm.User.PhoneNumber
                //    })
                //    .ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error: HrDAL : GetTeamMembers");
                return null!;
            }
        }

        public async Task SaveRoles(List<RoleDTO> roles, int companyId)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                //make old roles inactive because we will create new ones to keep history
                await _db.CompanyRoles
                    .Where(r => r.CompanyId == companyId
                             && r.IsLatest == true
                             && r.IsActive == true
                             && r.RoleName != "Principal" //ignore default roles
                             && r.RoleName != "Unassigned") //ignore default roles
                    .ExecuteUpdateAsync(setters => 
                        setters.SetProperty(r => r.IsLatest, false));


                var latestRoles = await _db.CompanyRoles
                    .AsNoTracking()
                    .Where(r => r.CompanyId == companyId 
                        && r.IsLatest == true 
                        && r.IsActive == true)
                    .ToListAsync();


                int nextRoleId = await GetNextRoleId(companyId);

                var rolesToSave = new List<CompanyRole>();

                // Process incoming roles - create new versions for all
                if (roles != null && roles.Any())
                {
                    foreach (var roleDto in roles)
                    {
                        // Check if this role existed before (excluding Principal and Unassigned)
                        var existing = latestRoles.FirstOrDefault(ct => ct.RoleName == roleDto.Name
                                                                      && ct.RoleName != "Principal"
                                                                      && ct.RoleName != "Unassigned");
                        if (existing != null)
                        {
                            rolesToSave.Add(new CompanyRole
                            {
                                RoleId = existing.RoleId,
                                RoleName = roleDto.Name,
                                CompanyId = companyId,
                                RoleVersion = existing.RoleVersion + 1.0M,
                                IsLatest = true,
                                IsActive = true,
                                CreatedDate = existing.CreatedDate,
                                ModifiedDate = DateTime.UtcNow
                            });
                        }
                        else
                        {
                            rolesToSave.Add(new CompanyRole
                            {
                                RoleId = nextRoleId,
                                RoleName = roleDto.Name,
                                CompanyId = companyId,
                                RoleVersion = 1.0M,
                                IsLatest = true,
                                IsActive = true,
                                CreatedDate = DateTime.UtcNow,
                                ModifiedDate = DateTime.UtcNow
                            });

                            nextRoleId++;
                        }
                    }
                }

                if (rolesToSave.Any())
                {
                    await _db.CompanyRoles.AddRangeAsync(rolesToSave);
                    await _db.SaveChangesAsync();
                }

                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                _logger.LogError(e, "Error: HrDAL : SaveRoles");
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
                                DepartmentVersion = existing.DepartmentVersion + 1.0M,
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
                                DepartmentVersion = 1.0M,
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
                        DepartmentVersion = deptToDelete.DepartmentVersion + 1.0M,
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

        public async Task AssignTeam()
        {
            try
            {

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error: HrDAL : AssignTeam");
            }
        }

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
                                TeamVersion = existing.TeamVersion + 1.0M,
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
                                TeamVersion = 1.0M,
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
                        TeamVersion = teamToDelete.TeamVersion + 1.0M,
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
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var toRemove = dto.Where(u => u.SaveAction == (int)Constants.SaveAction.Remove);
                var toAdd = dto.Where(u => u.SaveAction == (int)Constants.SaveAction.Add);

                var users = _db.TeamUserRoles.Where(tur => tur.CompanyId == companyId && tur.TeamKey.ToString() == teamKey);

                //foreach (var user in toAdd)
                //{
                //    var newMember = new TeamUserRole();
                //    newMember.CompanyId = companyId;
                //    newMember.TeamKey = teamKey;

                //    await _db.TeamUserRoles.AddAsync();
                //}
                
                //users.Add()

                //TeamUserRole teamUserRole = new TeamUserRole();
                //teamUserRole.CompanyId = companyId;
                //teamUserRole.
                //_db.TeamUserRoles.Add();
                //List<TeamMember> members = new List<TeamMember>();

                //foreach (var m in teamMembers)
                //{
                //    members.Add(new TeamMember()
                //    {
                //        //TeamId = m.TeamId,
                //        //CompanyId = m.UserId
                //    });
                //}

                //await _db.TeamMembers.AddRangeAsync(members);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                _logger.LogError(e, "Error: HrDAL : AssignTeamMembers");
            }
        }

    }
}
