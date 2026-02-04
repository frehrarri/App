using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Utilities.IO;
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
        public IActionResult RegisterEmployeePartial()
        {
            RegisterVM vm = new RegisterVM();
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            return PartialView("~/Views/App/HR/_RegisterEmployee.cshtml", vm);
        }

        [HttpGet]
        public async Task<IActionResult> ManageDepartmentPartial()
        {
            ManageDepartmentsVM vm = new ManageDepartmentsVM();
            var companyId = HttpContext.Session.GetInt32("CompanyId");

            var dto = await _hrBLL.GetDepartments(companyId!.Value);
            if (dto != null)
                vm.Departments = dto;

            return PartialView("~/Views/App/HR/_ManageDepartments.cshtml", vm);
        }

        [HttpGet]
        public async Task<IActionResult> AssignDepartmentPartial([FromQuery] string deptKey, [FromQuery] string deptName)
        {
            ViewBag.DeptName = deptName;
            ViewBag.DeptKey = deptKey;

            return PartialView("~/Views/App/HR/_AssignDepartment.cshtml");
        }


        [HttpGet]
        public async Task<IActionResult> ManageTeamsPartial()
        {
            ManageTeamsVM vm = new ManageTeamsVM();
            var companyId = HttpContext.Session.GetInt32("CompanyId");

            var dto = await _hrBLL.GetTeams(companyId!.Value);
            if (dto != null)
                vm.Teams = dto;

            return PartialView("~/Views/App/HR/_ManageTeams.cshtml", vm);
        }

        [HttpGet]
        public async Task<IActionResult> AssignTeamPartial([FromQuery] string teamKey, [FromQuery] string teamName)
        {
            List<AssignTeamVM> vm = new List<AssignTeamVM>();

            ViewBag.TeamName = teamName;
            ViewBag.TeamKey = teamKey;

            var dto = await GetAssignTeam(teamKey);

            if (dto.Count > 0)
            {
                vm = MapToVM(dto);
            }
                

            return PartialView("~/Views/App/HR/_AssignTeam.cshtml", vm);
        }

        private async Task<List<AssignTeamDTO>> GetAssignTeam(string teamKey)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            return await _hrBLL.GetAssignTeam(teamKey, companyId!.Value);
        }

        [HttpGet]
        public async Task<IActionResult> ManageRolesPartial()
        {
            ManageRolesVM vm = new ManageRolesVM();
            var companyId = HttpContext.Session.GetInt32("CompanyId");

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
        public async Task<IActionResult> HrSettingsPartial()
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            return PartialView("~/Views/App/HR/_HrSettings.cshtml");
        }

        [HttpGet]
        public async Task<List<ManagePersonnelDTO>> GetPersonnel()
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            return await _hrBLL.GetPersonnel(companyId!.Value);
        }

        [HttpGet]
        public async Task<List<ManageRolesDTO>> GetRoles()
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            return await _hrBLL.GetRoles(companyId!.Value);
        }

        [HttpGet]
        public async Task<List<TeamDTO>> GetTeams()
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            return await _hrBLL.GetTeams(companyId!.Value);
        }

        [HttpGet]
        public async Task<List<ManagePermissionsDTO>> GetPermissions()
        {
            return await _hrBLL.GetPermissions();
        }

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
        public async Task<IActionResult> SaveRoles([FromBody] List<ManageRolesDTO> roles)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            var username = HttpContext.Session.GetString("Username");

            if (!string.IsNullOrEmpty(username))
                roles.ForEach(r => r.CreatedBy = username);

            bool isSuccess = await _hrBLL.SaveRoles(roles, companyId!.Value);
            return Json(isSuccess);
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
        public async Task SavePermissions([FromBody] List<string> permissions)
        {
            await _hrBLL.SavePermissions(permissions);
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
        public async Task AssignTeamMembers([FromBody] List<AssignTeamDTO> dto)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId")!.Value;
            string? username = HttpContext.Session.GetString("Username");
            dto.ForEach(d => d.CreatedBy = username!);

            await _hrBLL.AssignTeamMembers(dto, companyId);
        }

        [HttpGet]
        public async Task<List<AssignDepartmentDTO>> GetAssignedDepartmentTeams(string deptKey)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId")!.Value;
            return await _hrBLL.GetAssignedDepartmentTeams(deptKey, companyId);
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

        [HttpGet]
        public async Task<List<AssignDepartmentDTO>> GetAssignedDepartmentUsers(string departmentKey)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId")!.Value;
            return await _hrBLL.GetAssignedDepartmentUsers(departmentKey, companyId);
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


        private List<AssignTeamVM> MapToVM(List<AssignTeamDTO> dtos)
        {
            List<AssignTeamVM> list = new List<AssignTeamVM>();

            foreach (var dto in dtos)
            {
                AssignTeamVM vm = new AssignTeamVM();
                vm.SaveAction = dto.DbChangeAction;
                vm.EmployeeId = dto.EmployeeId;
                vm.RoleId = dto.RoleId;
                vm.Role = dto.Role;
                vm.FirstName = dto.FirstName;
                vm.LastName = dto.LastName;
                vm.Username = dto.Username;
                vm.Email = dto.Email;
                vm.TeamName = dto.TeamName;
                vm.TeamId = dto.TeamId;
                vm.TeamKey = dto.TeamKey;
                list.Add(vm);
            }

            return list;
        }

    }
}
