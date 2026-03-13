using Microsoft.AspNetCore.Mvc;

namespace Voyage.Controllers
{
    public class SettingsController : Controller
    {
        [HttpGet]
        public IActionResult SettingsPartial()
        {
            return PartialView("~/Views/App/_Settings.cshtml");
        }

    }
}
