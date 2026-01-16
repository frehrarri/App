using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Voyage.Business;
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
        public async Task<IActionResult> ManagePersonnelPartial()
        {
            ManagePersonnelVM vm = new ManagePersonnelVM();

            var dto = await GetPersonnel();
            if (dto != null)
                vm.Personnel = dto;

            return PartialView("~/Views/App/HR/_ManagePersonnel.cshtml", vm);
        }

        [HttpGet]
        public async Task<IActionResult> ManageDepartmentPartial()
        {
            ManageDepartmentsVM vm = new ManageDepartmentsVM();

            var dto = await GetDepartments();
            if (dto != null)
                vm.Departments = dto;

            return PartialView("~/Views/App/HR/_ManageDepartments.cshtml", vm);
        }

        [HttpGet]
        public async Task<IActionResult> ManageTeamsPartial()
        {
            ManageTeamsVM vm = new ManageTeamsVM();

            var dto = await GetTeams();
            if (dto != null)
                vm.Teams = dto;

            return PartialView("~/Views/App/HR/_ManageTeams.cshtml", vm);
        }

        [HttpGet]
        public async Task<IActionResult> ManageRolesPartial()
        {
            ManageRolesVM vm = new ManageRolesVM();

            var dto = await GetRoles();
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
        public async Task<List<ManagePersonnelDTO>> GetPersonnel()
        {
            return await _hrBLL.GetPersonnel();
        }

        [HttpGet]
        public async Task<List<ManageRolesDTO>> GetRoles()
        {
            return await _hrBLL.GetRoles();
        }

        [HttpGet]
        public async Task<List<ManageDepartmentsDTO>> GetDepartments()
        {
            return await _hrBLL.GetDepartments();
        }

        [HttpGet]
        public async Task<List<ManageTeamsDTO>> GetTeams()
        {
            return await _hrBLL.GetTeams();
        }

        [HttpGet]
        public async Task<List<ManagePermissionsDTO>> GetPermissions()
        {
            return await _hrBLL.GetPermissions();
        }


        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public async Task SaveRoles([FromBody] List<string> roles)
        {
            await _hrBLL.SaveRoles(roles);
        }

        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public async Task SaveDepartments([FromBody] List<string> departments)
        {
            await _hrBLL.SaveDepartments(departments);
        }

        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public async Task SavePermissions([FromBody] List<string> permissions)
        {
            await _hrBLL.SavePermissions(permissions);
        }

        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public async Task SaveTeams([FromBody] List<string> teams)
        {
            await _hrBLL.SaveTeams(teams);
        }

    }
}
