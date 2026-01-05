using Microsoft.AspNetCore.Mvc;
using Voyage.Business;
using Voyage.Models.App;

namespace Voyage.Controllers
{
    public class AppController : Controller
    {
        TicketsB _ticketsB;

        public AppController(TicketsB ticketsB)
        {
            _ticketsB = ticketsB;
        }

        public IActionResult Index()
        {
            MainVM mainVM = new MainVM();

            return View("~/Views/App/WebAppLayout.cshtml", mainVM);
        }
    }
}
