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

        public async Task<IActionResult> Index()
        {
            MainVM mainVM = new MainVM();

            if (mainVM.TicketsVM != null)
            {
                mainVM.TicketsVM.SetSectionsDevelopment();
                mainVM.TicketsVM.Tickets = await _ticketsB.GetTickets();
            }

            return View("~/Views/App/WebAppLayout.cshtml", mainVM);
        }
    }
}
