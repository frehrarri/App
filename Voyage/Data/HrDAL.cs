using AngleSharp.Css;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
                        Email = u.Email!
                    }).ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error: HrDAL : GetPersonnel");
                return null!;
            }
        }

        public async Task<List<ManageRolesDTO>> GetRoles(int companyId)
        {
            try
            {
                return await _db.Roles
                    //.Where(u => u.)
                  .Select(u => new ManageRolesDTO
                  {
                      Name = u.Name!
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
                return await _db.TeamMembers
                    .Where(tm => tm.CompanyId == companyId)
                    .Select(tm => new TeamMemberDTO
                    {
                        TeamId = tm.TeamId,
                        TeamName = tm.Team.Name,
                        EmployeeId = tm.EmployeeId, 
                        Username = tm.User.UserName,
                        FirstName = tm.User.FirstName,
                        LastName = tm.User.LastName,
                        Email = tm.User.Email,
                        PhoneNumber = tm.User.PhoneNumber
                    })
                    .ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error: HrDAL : GetTeamMembers");
                return null!;
            }
        }

        public async Task SaveRoles(List<RoleDTO> roles)
        {
            try
            {
                //await _roleManager.CreateAsync();

                //roles.Select(r => new IdentityRole()
                //{
                //    Name = r.Name, 
                //    CompanyId = r.CompanyId
                //});

                //if (roles.Any())
                //{
                //    await _db.Roles.AddRangeAsync(roles);
                    await _db.SaveChangesAsync();
                //}
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error: HrDAL : SaveRoles");
            }
        }


        public async Task SaveDepartments(List<DepartmentDTO> departments)
        {
            if (departments == null || !departments.Any())
                return;

            int companyId = departments.First().CompanyId;

            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                //Delete existing departments for company
                await _db.Departments
                    .Where(d => d.CompanyId == companyId)
                    .ExecuteDeleteAsync();

                //Insert new departments
                var departmentsToSave = departments.Select(d => new Department
                {
                    Name = d.Name.Trim(),
                    CompanyId = companyId
                });

                await _db.Departments.AddRangeAsync(departmentsToSave);

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                _logger.LogError(e, "Error: HrDAL : SaveDepartments");
                throw;
            }
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

        public async Task SaveTeams(List<TeamDTO> teams)
        {
            if (teams == null || !teams.Any())
                return;

            int companyId = teams.First().CompanyId;

            try
            {
                //Mark old teams as not latest
                var currentTeams = await _db.Teams
                    .Where(t => t.CompanyId == companyId && t.IsLatest == true)
                    .ToListAsync();

                foreach (var t in currentTeams)
                    t.IsLatest = false;

                await _db.SaveChangesAsync();

                //Create new team entities with versioning
                var distinctTeams = teams.GroupBy(t => t.Name)
                                         .Select(g => g.First())
                                         .ToList();

                var teamsToSave = new List<Team>();

                foreach (var t in distinctTeams)
                {
                    var existing = currentTeams.FirstOrDefault(ct => ct.Name == t.Name);

                    teamsToSave.Add(new Team
                    {
                        TeamId = 0,
                        Name = t.Name,
                        CompanyId = companyId,
                        //DepartmentId = t.DepartmentId,
                        TeamVersion = existing != null ? existing.TeamVersion + 1.0M : 1.0M,
                        IsLatest = true,
                        IsActive = true
                    });
                }

                if (teamsToSave.Any())
                {
                    await _db.Teams.AddRangeAsync(teamsToSave);
                    await _db.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error: HrDAL : SaveTeams");
                throw;
            }
        }




        public async Task SaveTeamMembers(List<TeamDTO> teamMembers)
        {
            try
            {
                List<TeamMember> members = new List<TeamMember>();

                foreach (var m in teamMembers)
                {
                    members.Add(new TeamMember()
                    {
                        //TeamId = m.TeamId,
                        //CompanyId = m.UserId
                    });
                }

                await _db.TeamMembers.AddRangeAsync(members);
                await _db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error: HrDAL : SaveTeamMembers");
            }
        }

    }
}
