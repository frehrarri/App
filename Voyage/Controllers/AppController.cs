using Microsoft.AspNetCore.Mvc;
using Voyage.Business;
using Voyage.Models.App;

namespace Voyage.Controllers
{
    public class AppController : Controller
    {

        public AppController()
        {
        }

        public IActionResult Index()
        {
            MainVM vm = new MainVM();
            return View("~/Views/App/Main.cshtml", vm);
        }

        [HttpGet]
        public IActionResult MainDashboardPartial()
        {
            return PartialView("~/Views/App/_MainDashboard.cshtml");
        }
    }
}
