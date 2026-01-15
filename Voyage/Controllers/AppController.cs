using Microsoft.AspNetCore.Mvc;
using Voyage.Business;
using Voyage.Models.App;

namespace Voyage.Controllers
{
    public class AppController : Controller
    {
        TicketsBLL _ticketsB;

        public AppController(TicketsBLL ticketsB)
        {
            _ticketsB = ticketsB;
        }

        public IActionResult Index()
        {
            return View("~/Views/App/Main.cshtml");
        }
    }
}
