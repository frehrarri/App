using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Microsoft.VisualBasic;
using System.Threading.Tasks;
using Voyage.Business;
using Voyage.Data;
using Voyage.Data.TableModels;
using Voyage.Models.App;
using Voyage.Models.DTO;
using Voyage.Services;
using static Voyage.Utilities.Constants;
using static Voyage.Utilities.CustomAttributes;

namespace Voyage.Controllers
{
    public class TicketsController : Controller
    {
        private TicketsBLL _ticketsB;
        private UserManager<AppUser> _userManager;
        private LoggerService _log;
        

        public TicketsController(TicketsBLL ticketsB, UserManager<AppUser> userManager, LoggerService log)
        {
            _ticketsB = ticketsB;
            _userManager = userManager;
            _log = log;
        }

        #region Partials

        [HttpGet]
        public async Task<IActionResult> TicketsPartial(int? sprintId = null)
        {
            try
            {
                TicketsVM vm = new TicketsVM();
                var companyId = HttpContext.Session.GetInt32("CompanyId");

                TicketSettingsDTO? settings = await _ticketsB.GetSettings(companyId!.Value);

                if (sprintId.HasValue && settings != null)
                {
                    vm.Tickets = await GetTickets(sprintId.Value);

                    vm.Settings = settings;
                    vm.Sections = vm.Settings.Sections;
                    vm.Sprint.StartDate = vm.Settings.SprintStart;
                    vm.Sprint.SprintLength = vm.Settings.SprintLength;
                    vm.Sprint.SprintId = sprintId.Value;
                    vm.SettingsHistory = settings.SettingsHistory;

                    return PartialView("~/Views/App/Tickets/_Tickets.cshtml", vm);
                }
                else if (settings != null)
                {
                    var dto = await _ticketsB.UpdateSprint(settings);
                    vm.Tickets = await GetTickets(dto.SprintId);

                    vm.Settings = dto;
                    vm.Sections = dto.Sections;
                    vm.Sprint.StartDate = dto.SprintStart;
                    vm.Sprint.SprintLength = dto.SprintLength;
                    vm.Sprint.SprintId = dto.SprintId;
                    vm.SettingsHistory = settings.SettingsHistory;

                    return PartialView("~/Views/App/Tickets/_Tickets.cshtml", vm);
                }
                
                return PartialView("~/Views/App/Tickets/_SetTicketSettings.cshtml");
            }
            catch (Exception ex)
            {
                string message = "Could not retrieve Tickets page";

                await _log.Log(new LogDTO
                {
                    LogType = LogType.Error,
                    Severity = LogSeverity.High,
                    StackTrace = ex.StackTrace,
                    ClientMessage = message
                });

                return Json(message);
            }
        }

        [HttpGet]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> TicketPartial(int ticketId, decimal? ticketVersion = null)
        {
            try
            {
                ViewBag.Username = HttpContext.Session.GetString("Username");

                TicketVM? vm = await GetTicket(ticketId, ticketVersion);

                return PartialView("~/Views/App/Tickets/_Ticket.cshtml", vm);
            }
            catch (Exception ex)
            {
                string message = "Could not retrieve Ticket page";

                await _log.Log(new LogDTO
                {
                    LogType = LogType.Error,
                    Severity = LogSeverity.High,
                    StackTrace = ex.StackTrace,
                    ClientMessage = message
                });

                return Json(message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ManageTicketPartial(int? ticketId = null)
        {
            try
            {
                TicketVM vm = new TicketVM();

                if (ticketId != null)
                {
                    vm = await GetTicket(ticketId.Value);
                }

                TicketSettingsDTO? dto = await GetSettings();
                if (dto != null)
                {
                    vm.TicketSettings = MapToVM(dto);
                }

                return PartialView("~/Views/App/Tickets/_ManageTicket.cshtml", vm);

            }
            catch (Exception ex)
            {
                string message = "Could not retrieve Manage Ticket page";

                await _log.Log(new LogDTO
                {
                    LogType = LogType.Error,
                    Severity = LogSeverity.High,
                    StackTrace = ex.StackTrace,
                    ClientMessage = message
                });

                return Json(message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SettingsPartial(int companyId)
        {
            try
            {
                TicketSettingsVM vm = new TicketSettingsVM();

                TicketSettingsDTO? dto = await GetSettings();

                if (dto != null)
                    vm = MapToVM(dto);

                return PartialView("~/Views/App/Tickets/_TicketSettings.cshtml", vm);
            }
            catch (Exception ex)
            {
                string message = "Could not retrieve Ticket Settings page";

                await _log.Log(new LogDTO
                {
                    LogType = LogType.Error,
                    Severity = LogSeverity.High,
                    StackTrace = ex.StackTrace,
                    ClientMessage = message
                });

                return Json(message);
            }
        }


        #endregion

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
            if (ticketDTO.CompanyId <= 0)
                ticketDTO.CompanyId = HttpContext.Session.GetInt32("CompanyId")!.Value;

            if (string.IsNullOrEmpty(ticketDTO.CreatedBy)) 
                ticketDTO.CreatedBy = HttpContext.Session.GetString("Username")!;

            //ticketDTO.EmployeeId = HttpContext.Session.GetInt32("EmployeeId")!.Value;

            return await _ticketsB.SaveTicket(ticketDTO);
        }

        [HttpDelete]
        [ValidateHeaderAntiForgeryToken]
        public async Task<bool> DeleteTicket(int ticketId)
        {
            return await _ticketsB.DeleteTicket(ticketId);
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
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            details.CompanyId = companyId!.Value;

            var username = HttpContext.Session.GetString("Username");
            details.CreatedBy = username!;

            return await _ticketsB.SaveTicketDetails(details);
        }
        
        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public async Task MarkCompleted([FromBody] List<TicketDTO> dto)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId")!.Value;
            var username = HttpContext.Session.GetString("Username")!;

            dto.ForEach(t => {
                t.CompanyId = companyId;
                t.CreatedBy = username;
            });

            await _ticketsB.MarkCompleted(dto);
        }

        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public async Task Discontinue([FromBody] List<TicketDTO> dto)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId")!.Value;
            var username = HttpContext.Session.GetString("Username")!;

            dto.ForEach(t => {
                t.CompanyId = companyId;
                t.ModifiedBy = username;
            });

            await _ticketsB.Discontinue(dto);
        }

        [HttpGet]
        public async Task<TicketSettingsDTO?> GetSettings()
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            var settings = await _ticketsB.GetSettings(companyId!.Value);

            if (settings != null)
                settings.Sections = settings.Sections.Where(s => s.Title != RequiredTicketSections.Completed.ToString() 
                                        && s.Title != RequiredTicketSections.Discontinued.ToString()).ToList();

            return settings;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task SaveSettings([FromBody] TicketSettingsDTO dto)
        {
            dto.CompanyId = HttpContext.Session.GetInt32("CompanyId")!.Value;
            dto.EmployeeId = HttpContext.Session.GetInt32("EmployeeId")!.Value;
            dto.CreatedBy = HttpContext.Session.GetString("Username")!;
            await _ticketsB.SaveSettings(dto);
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
                SprintLength = t.SprintLength,
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
            vm.SettingsId = dto.SettingsId;
            vm.SprintStart = dto.SprintStart.HasValue ? dto.SprintStart.Value.Date : null;
            vm.SprintLength = dto.SprintLength;
            vm.RepeatSprintOption = (int?)dto.RepeatSprintOption;
            vm.Sections = dto.Sections;
            vm.SectionSetting = dto.SectionSetting;
            return vm;
        }

        #endregion

    }
}
