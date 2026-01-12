using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Voyage.Business;
using Voyage.Data;
using Voyage.Data.TableModels;
using Voyage.Models.App;
using Voyage.Models.DTO;
using static Voyage.Utilities.Constants;
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
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> TicketPartial(int ticketId, decimal? ticketVersion = null)
        {
            TicketVM? vm = await GetTicket(ticketId, ticketVersion);
            try
            {
                return PartialView("~/Views/Tickets/_Ticket.cshtml", vm);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
            //return PartialView("~/Views/Tickets/_Ticket.cshtml", vm);
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
        public async Task<IActionResult> SettingsPartial()
        {
            TicketSettingsDTO dto = await GetSettings();
            return PartialView("~/Views/Tickets/_TicketSettings.cshtml", MapToVM(dto));
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

        [HttpGet]
        public async Task<TicketsDTO> GetPaginatedTickets(int sprintId, string sectionTitle, int pageNumber = 1, int pageSize = 5)
        {
            return await _ticketsB.GetPaginatedTickets(sprintId, sectionTitle, pageNumber, pageSize);
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

        [HttpDelete]
        [ValidateHeaderAntiForgeryToken]
        public async Task<bool> DeleteTicket(int ticketId)
        {
            return await _ticketsB.DeleteTicket(ticketId);
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
        public async Task<TicketVM> GetTicket(int ticketId, decimal? ticketVersion = null)
        {
            TicketVM vm = new TicketVM();

            var tickets = await _ticketsB.GetTicket(ticketId, ticketVersion);

            if (tickets != null)
                vm = MapToVM(tickets);

            return vm;
        }

        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public async Task<TicketDetailsDTO?> SaveTicketDetails([FromBody] TicketDetailsDTO details)
        {
            return await _ticketsB.SaveTicketDetails(details);
        }


        [HttpGet]
        public async Task<TicketSettingsDTO> GetSettings()
        {
            return await _ticketsB.GetSettings();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<bool> SaveSettings([FromBody] TicketSettingsDTO dto)
        {
            return await _ticketsB.SaveSettings(dto);
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
                IsActive = t.IsActive,
                IsLatest = t.IsLatest,
                SprintStartDate = t.SprintStartDate.GetValueOrDefault(),
                SprintEndDate = t.SprintEndDate.GetValueOrDefault(),
                TicketChangeAction = t.TicketChangeAction
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
            ticketVM.VersionHistory = ticket.TicketVersionHistory;
            ticketVM.IsActive = ticket.IsActive;
            ticketVM.IsLatest = ticket.IsLatest;
            ticketVM.TicketChangeAction = ticket.TicketChangeAction;

            return ticketVM;
        }

        private TicketSettingsVM MapToVM(TicketSettingsDTO dto)
        {
            TicketSettingsVM vm = new TicketSettingsVM();
            vm.SprintStart = dto.SprintStart!.Value;
            vm.SprintEnd = dto.SprintEnd!.Value;
            vm.RepeatSprintOption = (int)dto.RepeatSprintOption;
            return vm;
        }
        #endregion

    }
}
