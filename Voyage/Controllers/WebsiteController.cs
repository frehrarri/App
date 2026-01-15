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
        TicketsBLL _ticketsB;

        public WebsiteController(TicketsBLL ticketsB)
        {
            _ticketsB = ticketsB;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            MainVM mainVM = new MainVM();

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
