using AngleSharp.Css;
using Microsoft.AspNetCore.Http.HttpResults;
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
                        DepartmentKey = u.DepartmentKey.ToString(),
                        DepartmentId = u.DepartmentId,
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
                        TeamKey = u.TeamKey.ToString(),
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

        public async Task<List<AssignTeamDTO>> GetAssignedTeamPersonnel(string teamKey, int companyId)
        {
            try
            {
                return await _db.TeamUserRoles
                    .Where(tur =>
                        tur.TeamKey == Guid.Parse(teamKey) &&
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
                        TeamId = u.TeamId,
                        TeamKey = u.TeamKey.ToString()
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


        public async Task<List<string>> SaveDepartments(List<DepartmentDTO> departments, int companyId)
        {
            List<string> newKeys = new();
            var newDepartments = new List<Department>();
            var datetime = DateTime.UtcNow;

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var existingDepartments = await _db.Departments.Where(d => d.CompanyId == companyId).ToListAsync();

                int nextDepartmentId = await GetNextDepartmentId(companyId);

                foreach (var dept in departments)
                {
                    switch ((SaveAction)dept.DbChangeAction)
                    {
                        case SaveAction.Save:
                            {
                                // INSERT
                                if (string.IsNullOrEmpty(dept.DeptKey))
                                {
                                    var newDept = new Department
                                    {
                                        DepartmentId = nextDepartmentId++,
                                        Name = dept.Name,
                                        CompanyId = companyId,
                                        CreatedDate = datetime,
                                        CreatedBy = dept.CreatedBy,
                                        IsLatest = true,
                                        IsActive = true
                                    };

                                    newDepartments.Add(newDept);
                                    await _db.Departments.AddAsync(newDept);
                                }
                                // UPDATE
                                else
                                {
                                    var existing = existingDepartments.FirstOrDefault(d => d.DepartmentKey == Guid.Parse(dept.DeptKey));

                                    if (existing != null)
                                    {
                                        existing.Name = dept.Name;
                                        existing.ModifiedDate = datetime;
                                        existing.ModifiedBy = dept.CreatedBy;
                                    }
                                }

                                break;
                            }

                        case SaveAction.Remove:
                            {
                                if (!string.IsNullOrEmpty(dept.DeptKey))
                                {
                                    var deptToDelete = existingDepartments
                                        .FirstOrDefault(d =>
                                            d.DepartmentKey == Guid.Parse(dept.DeptKey) &&
                                            d.CompanyId == companyId);

                                    if (deptToDelete != null)
                                    {
                                        _db.Departments.Remove(deptToDelete);
                                    }
                                }

                                break;
                            }
                    }
                }

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                newKeys = newDepartments
                    .Select(d => d.DepartmentKey.ToString())
                    .ToList();

                return newKeys;
            }
            catch (Exception e)
            {
                await tx.RollbackAsync();
                _logger.LogError(e, "Error: HrDAL.SaveDepartments()");
                return null!;
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

        public async Task<List<string>> SaveTeams(List<TeamDTO> teams, int companyId)
        {
            List<string> newKeys = new List<string>();
            var newTeams = new List<Team>();
            var datetime = DateTime.UtcNow;

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var existingTeams = await _db.Teams.Where(t => t.CompanyId == companyId).ToListAsync();
                int nextTeamId = await GetNextTeamId(companyId);

                foreach (var team in teams)
                {
                    switch (team.DbChangeAction)
                    {
                        case (int)SaveAction.Save:
                            {
                                //new insert
                                if (team.TeamKey == null)
                                {
                                    var newTeam = new Team
                                    {
                                        TeamId = nextTeamId++,
                                        Name = team.Name,
                                        CompanyId = companyId,
                                        CreatedDate = datetime,
                                        CreatedBy = team.CreatedBy,
                                        IsLatest = true,
                                        IsActive = true,
                                    };
                                    newTeams.Add(newTeam);
                                    await _db.Teams.AddAsync(newTeam);
                                }

                                // update existing
                                else
                                {
                                    var existingTeam = existingTeams.FirstOrDefault(t => t.TeamKey == Guid.Parse(team.TeamKey));
                                    
                                    if (existingTeam != null)
                                    {
                                        existingTeam.Name = team.Name;
                                        existingTeam.ModifiedDate = datetime;
                                        existingTeam.ModifiedBy = team.CreatedBy;
                                    }
                                }

                                break;
                            }

                        case (int)SaveAction.Remove:
                            {
                                var teamToDelete = existingTeams.FirstOrDefault(t => t.TeamKey == Guid.Parse(team.TeamKey) && t.CompanyId == companyId);

                                if (teamToDelete != null)
                                {
                                    _db.Teams.Remove(teamToDelete);
                                }
                                break;
                            }
                    }
                }

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                newKeys = newTeams.Select(t => t.TeamKey.ToString()).ToList();
                return newKeys;
            }
            catch (Exception e)
            {
                await tx.RollbackAsync();
                _logger.LogError(e, "Error: HrDAL.SaveTeams()");
                return null!;
            }
        }


        private async Task<int> GetNextTeamId(int companyId)
        {
            var maxTeamId = await _db.Teams
                .Where(t => t.CompanyId == companyId)
                .MaxAsync(t => (int?)t.TeamId) ?? 0;

            return maxTeamId + 1;
        }


        public async Task SaveAssignTeamMembers(List<AssignTeamDTO> dto, int companyId)
        {
            var teamKey = Guid.Parse(dto[0].TeamKey);
            var datetime = DateTime.UtcNow;

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var existing = await _db.TeamUserRoles
                    .Where(t =>
                        t.CompanyId == companyId &&
                        t.TeamKey == teamKey)
                    .ToListAsync();

                foreach (var item in dto)
                {
                    
                    switch (item.DbChangeAction)
                    {
                        // INSERT or UPDATE
                        case (int)SaveAction.Save:
                            {
                                var existingRow = existing
                                    .FirstOrDefault(e => e.EmployeeId == item.EmployeeId);

                                // insert
                                if (existingRow == null)
                                {
                                    await _db.TeamUserRoles.AddAsync(new TeamUserRole
                                    {
                                        CompanyId = companyId,
                                        TeamKey = teamKey,
                                        EmployeeId = item.EmployeeId,
                                        RoleId = item.RoleId,
                                        CreatedDate = datetime,
                                        CreatedBy = item.CreatedBy,
                                        IsLatest = true,
                                        IsActive = true
                                    });
                                }
                                // update
                                else if (existingRow.RoleId != item.RoleId)
                                {
                                    existingRow.RoleId = item.RoleId;
                                    existingRow.ModifiedDate = datetime;
                                    existingRow.ModifiedBy = item.CreatedBy;
                                }

                                break;
                            }

                        // DELETE (hard delete)
                        case (int)SaveAction.Remove:
                            {
                                var toDelete = existing
                                    .FirstOrDefault(e => e.EmployeeId == item.EmployeeId);

                                if (toDelete != null)
                                {
                                    _db.TeamUserRoles.Remove(toDelete);
                                }

                                break;
                            }
                    }
                }

                await _db.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Error: HrDAL.AssignTeamMembers()");
            }
        }

        public async Task<List<AssignDepartmentDTO>> GetAssignedDepartmentTeams(string departmentKey, int companyId)
        {
            try
            {
                return await _db.Teams
                    .Where(t => 
                        t.CompanyId == companyId 
                        && t.DepartmentKey == Guid.Parse(departmentKey))
                    .Select(t => new AssignDepartmentDTO 
                    { 
                        DepartmentKey = t.DepartmentKey.ToString(),
                        TeamId = t.TeamId,
                        TeamKey = t.TeamKey.ToString(),
                        TeamName = t.Name
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: HrDAL.GetAssignedDepartmentTeams()");
                return null!;
            }
        }

        public async Task SaveAssignDepartmentTeams(List<AssignDepartmentDTO> dto, int companyId)
        {
            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                if (!dto.Any() || companyId == 0)
                    return;

                if (string.IsNullOrEmpty(dto[0].DepartmentKey))
                    return;

                var deptKey = Guid.Parse(dto[0].DepartmentKey);
                var datetime = DateTime.UtcNow;

                var existingTeams = await _db.Teams.Where(t => t.CompanyId == companyId && t.IsActive == true).ToListAsync();

                foreach (var team in dto)
                {
                    if (string.IsNullOrEmpty(team.TeamKey))
                        return;

                    var teamKey = Guid.Parse(team.TeamKey);
                    var existingRow = existingTeams.FirstOrDefault(t => t.TeamKey == teamKey);

                    switch (team.DbChangeAction)
                    {
                        //assign team to department
                        case (int)SaveAction.Save:
                            {
                                if (existingRow != null)
                                {
                                    existingRow.DepartmentKey = deptKey;
                                    existingRow.ModifiedBy = team.CreatedBy;
                                    existingRow.ModifiedDate = datetime;
                                }

                                break;
                            }

                        // unassign department
                        case (int)SaveAction.Remove:
                            {
                                if (existingRow != null)
                                {
                                    existingRow.DepartmentKey = null;
                                    existingRow.ModifiedBy = team.CreatedBy;
                                    existingRow.ModifiedDate = datetime;
                                }

                                break;
                            }
                    }
                }

                await _db.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Error: HrDAL.SaveAssignDepartmentTeams()");
            }
        }

        public async Task<List<AssignDepartmentDTO>> GetAssignedDepartmentUsers(string departmentKey, int companyId)
        {
            try
            {
                return await _db.DepartmentUserRoles
                    .Where(t =>
                        t.CompanyId == companyId
                        && t.DepartmentKey == Guid.Parse(departmentKey))
                    .Select(t => new AssignDepartmentDTO
                    {
                        DepartmentKey = t.DepartmentKey.ToString(),
                        EmployeeId = t.EmployeeId,
                        RoleId = t.RoleId,
                        Username = t.User.UserName,
                        FirstName = t.User.FirstName,
                        LastName = t.User.LastName,
                        Email = t.User.Email
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: HrDAL.GetAssignedDepartmentTeams()");
                return null!;
            }
        }

        public async Task SaveAssignDepartmentUsers(List<AssignDepartmentDTO> dto, int companyId)
        {
            if (!dto.Any() || companyId == 0)
                return;

            if (string.IsNullOrEmpty(dto[0].DepartmentKey))
                return;

            var deptKey = Guid.Parse(dto[0].DepartmentKey);
            var datetime = DateTime.UtcNow;

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var existing = await _db.DepartmentUserRoles
                    .Where(t =>
                        t.CompanyId == companyId &&
                        t.DepartmentKey == deptKey)
                    .ToListAsync();

                foreach (var user in dto)
                {

                    switch (user.DbChangeAction)
                    {
                        // INSERT or UPDATE
                        case (int)SaveAction.Save:
                            {
                                var existingRow = existing.FirstOrDefault(e => e.EmployeeId == user.EmployeeId);

                                // insert
                                if (existingRow == null)
                                {
                                    await _db.DepartmentUserRoles.AddAsync(new DepartmentUserRole
                                    {
                                        CompanyId = companyId,
                                        DepartmentKey = deptKey,
                                        EmployeeId = user.EmployeeId,
                                        RoleId = user.RoleId,
                                        CreatedDate = datetime,
                                        CreatedBy = user.CreatedBy,
                                        IsLatest = true,
                                        IsActive = true
                                    });
                                }
                                // update
                                else if (existingRow.RoleId != user.RoleId)
                                {
                                    existingRow.RoleId = user.RoleId;
                                    existingRow.ModifiedDate = datetime;
                                    existingRow.ModifiedBy = user.CreatedBy;
                                }

                                break;
                            }

                        // DELETE
                        case (int)SaveAction.Remove:
                            {
                                var toDelete = existing
                                    .FirstOrDefault(e => e.EmployeeId == user.EmployeeId);

                                if (toDelete != null)
                                {
                                    _db.DepartmentUserRoles.Remove(toDelete);
                                }

                                break;
                            }
                    }
                }

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
