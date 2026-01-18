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

        public async Task<List<ManagePersonnelDTO>> GetPersonnel()
        {
            try
            {
                return await _db.Users
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

        public async Task<List<ManageRolesDTO>> GetRoles()
        {
            try
            {
                return await _db.Roles
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

        public async Task<List<ManageDepartmentsDTO>> GetDepartments()
        {
            try
            {
                return await _db.Departments
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

        public async Task<List<TeamDTO>> GetTeams()
        {
            try
            {
                return await _db.Teams
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

        public async Task<List<TeamMemberDTO>> GetTeamMembers()
        {
            try
            {
                return await _db.TeamMembers
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

        public async Task SaveRoles(List<IdentityRole> roles)
        {
            try
            {
                if (roles.Any())
                {
                    await _db.Roles.AddRangeAsync(roles);
                    await _db.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error: HrDAL : SaveRoles");
            }
        }


        public async Task SaveDepartments(List<string> departments)
        {
            try
            {
                List<Department> departmentsToSave = departments.Select(d => new Department() { Name = d }).ToList();

                if (departmentsToSave.Any())
                {
                    await _db.Departments.AddRangeAsync(departmentsToSave);
                }
                
                await _db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error: HrDAL : SaveDepartments");
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

        public async Task SaveTeams(List<string> teams)
        {
            try
            {
                //delete all teams
                _db.Teams.RemoveRange(_db.Teams);

                List<Team> teamsToSave = teams
                    .Distinct()
                    .Select(t => new Team { Name = t })
                    .ToList();

                if (teamsToSave.Any())
                {
                    await _db.Teams.AddRangeAsync(teamsToSave);
                }

                await _db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error: HrDAL : SaveTeams");
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
