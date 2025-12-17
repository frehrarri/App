using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Voyage.Business;
using Voyage.Data.TableModels;
using Voyage.Models.App;
using static Voyage.Utilities.CustomAttributes;

namespace Voyage.Controllers
{
    public class TicketsController : Controller
    {
        TicketsB _ticketsB;

        public TicketsController(TicketsB ticketsB)
        {
            _ticketsB = ticketsB;
        }

        [HttpGet]
        public async Task<IActionResult> TicketsPartial()
        {
            MainVM mainVM = new MainVM();

            if (mainVM.TicketsVM != null)
            {
                mainVM.TicketsVM.SetSectionsDevelopment();
                mainVM.TicketsVM.Tickets = await GetTickets();
                mainVM.TicketsVM.Sprint = new Sprint()
                {
                    SprintId = 1,
                    StartDate = DateTime.Now.ToShortDateString(),
                    EndDate = DateTime.Now.AddDays(21).ToShortDateString()
                };
            }

            return PartialView("~/Views/Tickets/_Tickets.cshtml", mainVM?.TicketsVM);
        }

        [HttpGet]
        public async Task<IActionResult> TicketPartial()
        {
            return PartialView("~/Views/Tickets/_Ticket.cshtml", new TicketVM());
        }

        [HttpGet]
        public async Task<IActionResult> ManageTicketPartial(int? ticketId = null)
        {
            TicketVM vm = new TicketVM();

            if (ticketId != null)
            {
                vm = await GetTicket(ticketId.Value);
            }

            return PartialView("~/Views/Tickets/_ManageTicket.cshtml", vm);
        }

        [HttpGet]
        public async Task<List<TicketVM>> GetTickets()
        {
            return await _ticketsB.GetTickets();
        }

        [HttpGet]
        public async Task<TicketVM> GetTicket(int ticketId)
        {
            return await _ticketsB.GetTicket(ticketId);
        }

        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public async Task<bool> SaveTicket([FromBody] Ticket ticket)
        {
            return await _ticketsB.SaveTicket(ticket);
        }

        [HttpDelete]
        [ValidateHeaderAntiForgeryToken]
        public async Task<bool> DeleteTicket(int ticketId)
        {
            return await _ticketsB.DeleteTicket(ticketId);
        }

        public void MoveTicket(int ticketId)
        {

        }

        public void AssignTicket(string userId, int? ticketId = null)
        {
            _ticketsB.AssignTicket(userId, ticketId);
        }

    }
}
