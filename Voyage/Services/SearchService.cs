using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Voyage.Data;
using Voyage.Data.TableModels;

namespace Voyage.Services
{
    public class SearchService : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly _AppDbContext _db;

        public SearchService(UserManager<AppUser> userManager, _AppDbContext db) 
        { 
            _userManager = userManager;
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Users(string query)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId")!.Value;
            query = query.ToLower();

            if (string.IsNullOrWhiteSpace(query))
                return Ok(Enumerable.Empty<object>());

            var users = await _userManager.Users.Where(u =>
                u.CompanyId == companyId &&
                (
                    (u.UserName.ToLower() ?? "").Contains(query)
                    || (u.Email.ToLower() ?? "").Contains(query)
                    || (u.FirstName.ToLower() ?? "").Contains(query)
                    || (u.LastName.ToLower() ?? "").Contains(query)
                ))
                .OrderBy(u => u.UserName)
                .Take(10)
                .Select(u => new {
                    id = u.Id,
                    username = u.UserName,
                    email = u.Email,
                    firstname = u.FirstName,
                    lastname = u.LastName,
                    phone = u.PhoneAreaCode + u.PhoneNumber,
                    employeeid = u.EmployeeId,
                    roleid = u.IndividualUserRoles.Select(r => r.RoleId).SingleOrDefault()
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet]
        public async Task<IActionResult> Teams(string query)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId")!.Value;

            if (string.IsNullOrWhiteSpace(query))
                return Ok(Enumerable.Empty<object>());

            var teams = await _db.Teams.Where(u => u.CompanyId == companyId && (u.Name.ToLower() ?? "")
                                    .Contains(query.ToLower()))
                                    .OrderBy(u => u.Name)
                                    .Take(10)
                                    .Select(u => new { 
                                        u.Name,
                                        u.TeamKey
                                    })
                                    .ToListAsync();

            return Ok(teams);
        }

        [HttpGet]
        public async Task<IActionResult> UnassignedDeptTeams(string query)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId")!.Value;

            if (string.IsNullOrWhiteSpace(query))
                return Ok(Enumerable.Empty<object>());

            var teams = await _db.Teams.Where(u => u.CompanyId == companyId
                                        && u.DepartmentKey == null
                                        && (u.Name.ToLower() ?? "")
                                    .Contains(query.ToLower()))
                                    .OrderBy(u => u.Name)
                                    .Take(10)
                                    .Select(u => new {
                                        u.Name,
                                        u.TeamKey
                                    })
                                    .ToListAsync();

            return Ok(teams);
        }

        [HttpGet]
        public async Task<IActionResult> UnassignedDeptUsers(string query, string parameter)
        {
            string deptKey = parameter;
            int companyId = HttpContext.Session.GetInt32("CompanyId")!.Value;

            if (string.IsNullOrWhiteSpace(query))
                return Ok(Enumerable.Empty<object>());

            var users = await _userManager.Users.Where(u =>
                     u.CompanyId == companyId
                     && !u.DepartmentUserRoles.Any()
                     && ((u.UserName.ToLower() ?? "").Contains(query)
                     || (u.Email.ToLower() ?? "").Contains(query)
                     || (u.FirstName.ToLower() ?? "").Contains(query)
                     || (u.LastName.ToLower() ?? "").Contains(query)))
                 .OrderBy(u => u.UserName)
                 .Take(10)
                 .Select(u => new {
                     id = u.Id,
                     username = u.UserName,
                     email = u.Email,
                     firstname = u.FirstName,
                     lastname = u.LastName,
                     phone = u.PhoneAreaCode + u.PhoneNumber,
                     employeeid = u.EmployeeId,
                     roleid = u.IndividualUserRoles.Select(r => r.RoleId).SingleOrDefault()
                 })
                 .ToListAsync();

            return Ok(users);
        }

        [HttpGet]
        public async Task<IActionResult> UnassignedTeamUsers(string query, string parameter)
        {
            string deptKey = parameter;
            int companyId = HttpContext.Session.GetInt32("CompanyId")!.Value;

            if (string.IsNullOrWhiteSpace(query))
                return Ok(Enumerable.Empty<object>());

            var users = await _userManager.Users.Where(u =>
                     u.CompanyId == companyId
                     && !u.TeamUserRoles.Any()
                     && ((u.UserName.ToLower() ?? "").Contains(query)
                     || (u.Email.ToLower() ?? "").Contains(query)
                     || (u.FirstName.ToLower() ?? "").Contains(query)
                     || (u.LastName.ToLower() ?? "").Contains(query)))
                 .OrderBy(u => u.UserName)
                 .Take(10)
                 .Select(u => new {
                     id = u.Id,
                     username = u.UserName,
                     email = u.Email,
                     firstname = u.FirstName,
                     lastname = u.LastName,
                     phone = u.PhoneAreaCode + u.PhoneNumber,
                     employeeid = u.EmployeeId,
                     roleid = u.IndividualUserRoles.Select(r => r.RoleId).SingleOrDefault()
                 })
                 .ToListAsync();

            return Ok(users);
        }

    }
}
