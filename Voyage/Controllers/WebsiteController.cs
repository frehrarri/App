using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using Voyage.Business;
using Voyage.Data.TableModels;
using Voyage.Models;
using Voyage.Models.App;
using Voyage.Services;

namespace Voyage.Controllers
{
    public class WebsiteController : Controller
    {
        TicketsB _ticketsB;

        public WebsiteController(TicketsB ticketsB)
        {
            _ticketsB = ticketsB;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            MainVM mainVM = new MainVM();

            if (mainVM.TicketsVM != null)
            {
                mainVM.TicketsVM.SetSectionsDevelopment();
                mainVM.TicketsVM.Tickets = await _ticketsB.GetTickets();
            }

            return View("Website", mainVM);
        }

        public IActionResult Home()
        {
            return View("Home");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
