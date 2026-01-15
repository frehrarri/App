using Microsoft.AspNetCore.Mvc;

namespace Voyage.Controllers
{
    public class HrController : Controller
    {

        [HttpGet]
        public IActionResult ManagePersonnelPartial()
        {
            return PartialView("~/Views/App/HR/_ManagePersonnel.cshtml");
        }
    }
}
