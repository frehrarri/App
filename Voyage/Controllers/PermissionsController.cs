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

        [HttpGet]
        public async Task<PermissionsDTO> GetRolePermissions(string roleKey)
        {
            int companyId = HttpContext.Session.GetInt32("RoleId")!.Value;
            return await _permissionsBLL.GetRolePermissions(companyId, roleKey);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task SetDefaultRolePermissions(string roleKey, int roleType)
        {
            PermissionsDTO dto = new PermissionsDTO();
            dto.CompanyId = HttpContext.Session.GetInt32("RoleId")!.Value;
            dto.RoleKey = roleKey;
            dto.RoleType = roleType;
            dto.CreatedBy = HttpContext.Session.GetString("Username")!;

            await _permissionsBLL.SetDefaultRolePermissions(dto);
        }


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
