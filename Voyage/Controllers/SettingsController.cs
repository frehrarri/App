using Microsoft.AspNetCore.Mvc;

namespace Voyage.Controllers
{
    public class SettingsController : Controller
    {
        public IActionResult AdminSettingsPartial()
        {
            return PartialView("~/Views/App/AdminSettings/_AdminSettings.cshtml");
        }

    }
}
