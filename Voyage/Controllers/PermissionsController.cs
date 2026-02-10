using Microsoft.AspNetCore.Mvc;
using Voyage.Business;
using Voyage.Data;
using Voyage.Models.App;
using Voyage.Models.DTO;

namespace Voyage.Controllers
{
    public class PermissionsController : Controller
    {
        private readonly PermissionsBLL _permissionsBLL;

        public PermissionsController(PermissionsBLL permissionsBLL)
        {
            _permissionsBLL = permissionsBLL;
        }

        #region Partials

        [HttpGet]
        public async Task<IActionResult> RolePermissionsPartial([FromQuery] string name, [FromQuery] string roleKey)
        {
            PermissionsVM vm = new PermissionsVM();

            ViewBag.RoleName = name;
            ViewBag.RoleKey = roleKey;

            var dto = await GetRolePermissions(roleKey);
            if (dto != null)
                MapToVM(ref vm, dto);

            return PartialView("~/Views/App/Permissions/_RolePermissions.cshtml", vm);
        }

        public async Task<IActionResult> DeptPermissionsPartial([FromQuery] string name, [FromQuery] string deptKey)
        {
            PermissionsVM vm = new PermissionsVM();

            ViewBag.DeptName = name;
            ViewBag.DeptKey = deptKey;

            var dto = await GetDeptPermissions(deptKey);
            if (dto != null)
                MapToVM(ref vm, dto);

            return PartialView("~/Views/App/Permissions/_DepartmentPermissions.cshtml", vm);
        }

        public async Task<IActionResult> TeamPermissionsPartial([FromQuery] string name, [FromQuery] string teamKey)
        {
            PermissionsVM vm = new PermissionsVM();

            ViewBag.TeamName = name;
            ViewBag.TeamKey = teamKey;

            var dto = await GetTeamPermissions(teamKey);
            if (dto != null)
                MapToVM(ref vm, dto);

            return PartialView("~/Views/App/Permissions/_TeamPermissions.cshtml", vm);
        }

        public async Task<IActionResult> UserPermissionsPartial([FromQuery] string name, [FromQuery] string userKey)
        {
            PermissionsVM vm = new PermissionsVM();

            ViewBag.UserName = name;
            ViewBag.UserKey = userKey;

            var dto = await GetUserPermissions(userKey);
            if (dto != null)
                MapToVM(ref vm, dto);

            return PartialView("~/Views/App/Permissions/_UserPermissions.cshtml", vm);
        }

        #endregion


        #region Get Methods

        [HttpGet]
        public async Task<PermissionsDTO> GetRolePermissions(string roleKey)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId")!.Value;
            return await _permissionsBLL.GetRolePermissions(companyId, roleKey);
        }

        [HttpGet]
        public async Task<PermissionsDTO> GetDeptPermissions(string deptKey)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId")!.Value;
            return await _permissionsBLL.GetDeptPermissions(companyId, deptKey);
        }

        [HttpGet]
        public async Task<PermissionsDTO> GetTeamPermissions(string teamKey)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId")!.Value;
            return await _permissionsBLL.GetTeamPermissions(companyId, teamKey);
        }

        [HttpGet]
        public async Task<PermissionsDTO> GetUserPermissions(string userKey)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId")!.Value;
            return await _permissionsBLL.GetUserPermissions(companyId, userKey);
        }

        #endregion


        #region Save methods

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task SetPermissions([FromBody] PermissionsDTO dto)
        {
            dto.CreatedBy = HttpContext.Session.GetString("Username")!;
            dto.CompanyId = HttpContext.Session.GetInt32("CompanyId")!.Value;

            await _permissionsBLL.SetPermissions(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task SetDefaultRolePermissions(string roleKey, int roleType)
        {
            PermissionsDTO dto = new PermissionsDTO();
            dto.CompanyId = HttpContext.Session.GetInt32("CompanyId")!.Value;
            dto.RoleKey = roleKey;
            dto.RoleType = roleType;
            dto.CreatedBy = HttpContext.Session.GetString("Username")!;

            await _permissionsBLL.SetDefaultRolePermissions(dto);
        }

        #endregion

     
        private void MapToVM(ref PermissionsVM vm, PermissionsDTO dto)
        {
            foreach (var p in dto.Permissions)
            {
                vm.Permissions.Add(new PermissionVM()
                {
                    Permission = p.PermissionName,
                    PermissionKey = p.PermissionKey,
                    InheritsFrom = p.InheritsFrom,
                    IsEnabled = p.IsEnabled
                });
            }

            vm.AccessGranted = dto.AccessGranted;
        }
    }
}
