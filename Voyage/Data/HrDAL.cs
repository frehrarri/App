using AngleSharp.Css;
using Microsoft.AspNetCore.Identity;
using System.Data;
using Voyage.Data.TableModels;
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
                List<Team> teamsToSave = teams.Select(t => new Team() { Name = t }).ToList();

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
    }
}
