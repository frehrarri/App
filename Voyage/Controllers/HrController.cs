using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Threading.Tasks;
using Voyage.Business;
using Voyage.Data;
using Voyage.Data.TableModels;
using Voyage.Models.App;
using Voyage.Models.DTO;
using static Voyage.Utilities.Constants;
using static Voyage.Utilities.CustomAttributes;

namespace Voyage.Controllers
{
    public class HrController : Controller
    {
        private readonly HrBLL _hrBLL;

        public HrController(HrBLL hrBLL)
        {
            _hrBLL = hrBLL;
        }

        [HttpGet]
        public IActionResult HrControlPartial()
        {
            HrVM vm = new HrVM();
            return PartialView("~/Views/App/HR/_HrControl.cshtml", vm);
        }

        [HttpGet]
        public async Task<IActionResult> ManagePersonnelPartial(int companyId)
        {
            ManagePersonnelVM vm = new ManagePersonnelVM();

            var dto = await GetPersonnel(companyId);
            if (dto != null)
                vm.Personnel = dto;

            return PartialView("~/Views/App/HR/_ManagePersonnel.cshtml", vm);
        }

        [HttpGet]
        public IActionResult RegisterEmployeePartial(int companyId)
        {
            RegisterVM vm = new RegisterVM();
            return PartialView("~/Views/App/HR/_RegisterEmployee.cshtml", vm);
        }

        [HttpGet]
        public async Task<IActionResult> ManageDepartmentPartial(int companyId)
        {
            ManageDepartmentsVM vm = new ManageDepartmentsVM();

            var dto = await GetDepartments(companyId);
            if (dto != null)
                vm.Departments = dto;

            return PartialView("~/Views/App/HR/_ManageDepartments.cshtml", vm);
        }

        [HttpGet]
        public async Task<IActionResult> ManageTeamsPartial(int companyId)
        {
            ManageTeamsVM vm = new ManageTeamsVM();

            var dto = await _hrBLL.GetAssignedTeams(companyId);
            if (dto != null)
                vm.ManageTeamsDTO = dto;

            return PartialView("~/Views/App/HR/_ManageTeams.cshtml", vm);
        }

        [HttpGet]
        public async Task<IActionResult> ManageRolesPartial(int companyId)
        {
            ManageRolesVM vm = new ManageRolesVM();

            var dto = await GetRoles(companyId);
            if (dto != null)
                vm.Roles = dto;

            return PartialView("~/Views/App/HR/_ManageRoles.cshtml", vm);
        }

        [HttpGet]
        public async Task<IActionResult> ManagePermissionsPartial()
        {
            ManagePermissionsVM vm = new ManagePermissionsVM();

            var dto = await GetPermissions();
            if (dto != null)
                vm.Permissions = dto;

            return PartialView("~/Views/App/HR/_ManagePermissions.cshtml", vm);
        }

        [HttpGet]
        public async Task<List<ManagePersonnelDTO>> GetPersonnel(int companyId)
        {
            return await _hrBLL.GetPersonnel(companyId);
        }

        [HttpGet]
        public async Task<List<ManageRolesDTO>> GetRoles(int companyId)
        {
            return await _hrBLL.GetRoles(companyId);
        }

        [HttpGet]
        public async Task<List<ManageDepartmentsDTO>> GetDepartments(int companyId)
        {
            return await _hrBLL.GetDepartments(companyId);
        }

        [HttpGet]
        public async Task<List<TeamDTO>> GetTeams()
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId")!.Value;
            return await _hrBLL.GetTeams(companyId);
        }

        [HttpGet]
        public async Task<List<ManagePermissionsDTO>> GetPermissions()
        {
            return await _hrBLL.GetPermissions();
        }

        //[HttpGet]
        //public async Task<List<TeamMemberDTO>> GetTeamMembers()
        //{
        //    return await _hrBLL.GetTeamMembers();
        //}


        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public async Task SaveRoles([FromBody] List<string> roles)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            var dto = roles.Select(d => new RoleDTO
            {
                Name = d,
                CompanyId = companyId!.Value
            }).ToList();

            await _hrBLL.SaveRoles(dto);
        }

        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public async Task SaveDepartments([FromBody] List<string> departments)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            var dto = departments.Select(d => new DepartmentDTO
            {
                Name = d,
                CompanyId = companyId!.Value
            }).ToList();

            await _hrBLL.SaveDepartments(dto);
        }

        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public async Task SavePermissions([FromBody] List<string> permissions)
        {
            await _hrBLL.SavePermissions(permissions);
        }

        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public async Task<List<TeamDTO>> SaveTeams([FromBody] List<string> teams)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            var dto = teams.Select(d => new TeamDTO
            {
                Name = d,
                CompanyId = companyId!.Value
            }).ToList();

            return await _hrBLL.SaveTeams(dto, companyId!.Value);
        }

        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public async Task SaveTeamMembers([FromBody] List<TeamDTO> teamMembers)
        {
            await _hrBLL.SaveTeamMembers(teamMembers);
        }

    }
}
