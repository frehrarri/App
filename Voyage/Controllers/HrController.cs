using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Utilities.IO;
using System.ComponentModel.Design;
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


        #region Partial Views

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

            vm.RoleId = HttpContext.Session.GetInt32("RoleId");

            var dto = await GetPersonnel();
            if (dto != null)
            {
                vm.Personnel = dto;
                var roles = await GetRoles();

                if (roles != null)
                    vm.Roles = roles;
            }

            return PartialView("~/Views/App/HR/_ManagePersonnel.cshtml", vm);
        }

        [HttpGet]
        public IActionResult ManageDepartmentPartial()
        {
            return PartialView("~/Views/App/HR/_ManageDepartments.cshtml");
        }

        [HttpGet]
        public IActionResult AssignDepartmentPartial([FromQuery] string deptKey, [FromQuery] string deptName)
        {
            ViewBag.DeptName = deptName;
            ViewBag.DeptKey = deptKey;

            return PartialView("~/Views/App/HR/_AssignDepartment.cshtml");
        }

        [HttpGet]
        public IActionResult ManageTeamsPartial()
        {
            return PartialView("~/Views/App/HR/_ManageTeams.cshtml");
        }

        [HttpGet]
        public IActionResult AssignTeamPartial([FromQuery] string teamKey, [FromQuery] string teamName)
        {
            ViewBag.TeamName = teamName;
            ViewBag.TeamKey = teamKey;

            return PartialView("~/Views/App/HR/_AssignTeam.cshtml");
        }

        [HttpGet]
        public IActionResult ManageRolesPartial()
        {
            return PartialView("~/Views/App/HR/_ManageRoles.cshtml");
        }

        [HttpGet]
        public IActionResult RegisterEmployeePartial()
        {
            RegisterVM vm = new RegisterVM();
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            return PartialView("~/Views/App/HR/_RegisterEmployee.cshtml", vm);
        }


        [HttpGet]
        public IActionResult RolePermissionsPartial([FromQuery] string name, [FromQuery] string roleKey)
        {
            ViewBag.RoleName = name;
            ViewBag.RoleKey = roleKey;
            return PartialView("~/Views/App/Settings/_RolePermissions.cshtml");
        }

        #endregion


        #region Get Methods

        [HttpGet]
        public async Task<List<ManagePersonnelDTO>> GetPersonnel()
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            return await _hrBLL.GetPersonnel(companyId!.Value);
        }

        [HttpGet]
        public async Task<List<ManageDepartmentsDTO>> GetDepartments()
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            return await _hrBLL.GetDepartments(companyId!.Value);
        }

        [HttpGet]
        public async Task<List<AssignDepartmentDTO>> GetAssignedDepartmentTeams(string deptKey)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId")!.Value;
            return await _hrBLL.GetAssignedDepartmentTeams(deptKey, companyId);
        }

        [HttpGet]
        public async Task<List<AssignDepartmentDTO>> GetAssignedDepartmentUsers(string departmentKey)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId")!.Value;
            return await _hrBLL.GetAssignedDepartmentUsers(departmentKey, companyId);
        }

        [HttpGet]
        public async Task<List<TeamDTO>> GetTeams()
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            return await _hrBLL.GetTeams(companyId!.Value);
        }

        [HttpGet]
        public async Task<List<AssignTeamDTO>> GetAssignedTeamPersonnel(string teamKey)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            return await _hrBLL.GetAssignedTeamPersonnel(teamKey, companyId!.Value);
        }

        [HttpGet]
        public async Task<List<ManageRolesDTO>> GetRoles()
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            return await _hrBLL.GetRoles(companyId!.Value);
        }

        #endregion


        #region Save Methods

        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public async Task<IActionResult> SavePersonnel([FromBody] List<ManagePersonnelDTO> personnel)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            var username = HttpContext.Session.GetString("Username");

            if (!string.IsNullOrEmpty(username))
                personnel.ForEach(r => r.CreatedBy = username);

            bool isSuccess = await _hrBLL.SavePersonnel(personnel, companyId!.Value);
            return Json(isSuccess);
        }

        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public async Task<List<string>> SaveTeams([FromBody] List<TeamDTO> teams)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            var username = HttpContext.Session.GetString("Username");
            teams.ForEach(t => t.CreatedBy = username);

            return await _hrBLL.SaveTeams(teams, companyId!.Value);
        }

        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public async Task<List<string>> SaveDepartments([FromBody] List<DepartmentDTO> departments)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");

            var username = HttpContext.Session.GetString("Username");
            departments.ForEach(t => t.CreatedBy = username);

            return await _hrBLL.SaveDepartments(departments, companyId!.Value);
        }

        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public async Task SaveAssignDepartmentTeams([FromBody] List<AssignDepartmentDTO> dto)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId")!.Value;
            string? username = HttpContext.Session.GetString("Username");
            dto.ForEach(d => d.CreatedBy = username!);

            await _hrBLL.SaveAssignDepartmentTeams(dto, companyId);
        }

        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public async Task SaveAssignDepartmentUsers([FromBody] List<AssignDepartmentDTO> dto)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId")!.Value;
            string? username = HttpContext.Session.GetString("Username");
            dto.ForEach(d => d.CreatedBy = username!);

            await _hrBLL.SaveAssignDepartmentUsers(dto, companyId);
        }

        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public async Task SaveAssignTeamMembers([FromBody] List<AssignTeamDTO> dto)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId")!.Value;
            string? username = HttpContext.Session.GetString("Username");
            dto.ForEach(d => d.CreatedBy = username!);

            await _hrBLL.SaveAssignTeamMembers(dto, companyId);
        }

        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public async Task<List<string>> SaveRoles([FromBody] List<ManageRolesDTO> roles)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            var username = HttpContext.Session.GetString("Username");

            if (!string.IsNullOrEmpty(username))
                roles.ForEach(r => r.CreatedBy = username);

            return await _hrBLL.SaveRoles(roles, companyId!.Value);
        }

        #endregion









        [HttpGet]
        public async Task<IActionResult> HrSettingsPartial()
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            return PartialView("~/Views/App/HR/_HrSettings.cshtml");
        }




    }
}
