using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voyage.Business;
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
        public IActionResult ManagePersonnelPartial()
        {
            return PartialView("~/Views/App/HR/_ManagePersonnel.cshtml");
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
