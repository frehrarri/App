using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Voyage.Business;
using Voyage.Data;
using Voyage.Data.TableModels;
using Voyage.Models.App;
using Voyage.Models.DTO;
using static Voyage.Utilities.CustomAttributes;

namespace Voyage.Controllers
{
    public class TicketsController : Controller
    {
        private TicketsB _ticketsB;
        private UserManager<AppUser> _userManager;
        

        public TicketsController(TicketsB ticketsB, UserManager<AppUser> userManager)
        {
            _ticketsB = ticketsB;
            _userManager = userManager;
        }

        #region Partials

        [HttpGet]
        public async Task<IActionResult> TicketsPartial()
        {
            MainVM mainVM = new MainVM();

            if (mainVM.TicketsVM != null)
            {
                mainVM.TicketsVM.Sections = _ticketsB.SetSectionsDevelopment();
                //mainVM.TicketsVM.Tickets = await GetTickets(DateTime.UtcNow); // switch to this once we have settings to 
                mainVM.TicketsVM.Tickets = await GetTickets(1);
                mainVM.TicketsVM.Sprint = SetSprint();
            }

            return PartialView("~/Views/Tickets/_Tickets.cshtml", mainVM?.TicketsVM);
        }

        [HttpGet]
        public async Task<IActionResult> TicketPartial(int ticketId)
        {
            TicketVM? vm = await GetTicket(ticketId);
            return PartialView("~/Views/Tickets/_Ticket.cshtml", vm);
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

        #endregion


        //[HttpGet]
        //public async Task<List<TicketVM>> GetTickets(DateTime date)
        //{
        //    var tickets = await _ticketsB.GetTickets(date);
        //    return MapToVM(tickets);
        //}

        public async Task<List<TicketVM>> GetTickets(int sprintId)
        {
            List<TicketVM> list = new List<TicketVM>();

            var tickets = await _ticketsB.GetTickets(sprintId);

            if (tickets != null)
                list = MapToVM(tickets);

            return list;
        }

        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public async Task<bool> SaveTicket([FromBody] TicketDTO ticketDTO)
        {
            Sprint sprint = SetSprint();
            ticketDTO.SprintId = sprint.SprintId;
            ticketDTO.SprintEndDate = sprint.EndDate;
            ticketDTO.SprintStartDate = sprint.StartDate;
            return await _ticketsB.SaveTicket(ticketDTO);
        }

        private Sprint SetSprint()
        {
            Sprint sprint = new Sprint();
            sprint.SprintId = 1;
            sprint.StartDate = DateTime.UtcNow.AddDays(-15);
            sprint.EndDate = DateTime.UtcNow.AddDays(15);

            return sprint;
        }

        [HttpGet]
        public async Task<TicketVM> GetTicket(int ticketId)
        {
            TicketVM vm = new TicketVM();

            var tickets = await _ticketsB.GetTicket(ticketId);

            if (tickets != null)
                vm = MapToVM(tickets);

            return vm;
        }


        [HttpGet]
        public async Task<List<TicketDetailsDTO>> GetTicketDetails(int ticketId)
        {
            return await _ticketsB.GetTicketDetails(ticketId);
        }

        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public async Task<TicketDetailsDTO?> SaveTicketDetails([FromBody] TicketDetailsDTO details)
        {
            return await _ticketsB.SaveTicketDetails(details);
        }


        #region VM Mappings

        private List<TicketVM> MapToVM(List<TicketDTO> tickets)
        {
            return tickets.Select(t => new TicketVM()
            {
                TicketId = t.TicketId,
                TicketVersion = t.TicketVersion,
                ParentTicketId = t.ParentTicketId,
                Title = t.Title,
                SectionTitle = t.SectionTitle,
                AssignedTo = t.AssignedTo,
                CreatedBy = t.CreatedBy,
                Description = t.Description,
                CreatedDate = t.CreatedDate,
                DueDate = t.DueDate,
                ModifiedBy = t.ModifiedBy,
                ModifiedDate = t.ModifiedDate,
                PriorityLevel = t.PriorityLevel,
                Status = t.Status,
                SprintId = t.SprintId,
                SprintStartDate = t.SprintStartDate.GetValueOrDefault(),
                SprintEndDate = t.SprintEndDate.GetValueOrDefault()
            }).ToList();
        }

        private TicketVM MapToVM(TicketDTO ticket)
        {
            TicketVM ticketVM = new TicketVM();

            ticketVM.TicketId = ticket.TicketId;
            ticketVM.TicketVersion = ticket.TicketVersion;
            ticketVM.ParentTicketId = ticket.ParentTicketId;
            ticketVM.Title = ticket.Title;
            ticketVM.SectionTitle = ticket.SectionTitle;
            ticketVM.AssignedTo = ticket.AssignedTo;
            ticketVM.CreatedBy = ticket.CreatedBy;
            ticketVM.Description = ticket.Description;
            ticketVM.CreatedDate = ticket.CreatedDate;
            ticketVM.DueDate = ticket.DueDate;
            ticketVM.ModifiedBy = ticket.ModifiedBy;
            ticketVM.ModifiedDate = ticket.ModifiedDate;
            ticketVM.PriorityLevel = ticket.PriorityLevel;
            ticketVM.Status = ticket.Status;
            ticketVM.TicketDetails = ticket.TicketDetailsDTOs;

            return ticketVM;
        }

        #endregion













        //[HttpDelete]
        //[ValidateHeaderAntiForgeryToken]
        //public async Task<bool> DeleteTicket(int ticketId)
        //{
        //    return await _ticketsB.DeleteTicket(ticketId);
        //}

        //public void MoveTicket(int ticketId)
        //{

        //}

        //public void AssignTicket(string userId, int? ticketId = null)
        //{
        //    _ticketsB.AssignTicket(userId, ticketId);
        //}


    }
}
