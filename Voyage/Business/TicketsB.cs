using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;
using Voyage.Data;
using Voyage.Data.TableModels;
using Voyage.Models.App;
using Voyage.Models.DTO;
using Voyage.Utilities;
using static Voyage.Models.DTO.SectionDTO;
using static Voyage.Utilities.Constants;
using static Voyage.Utilities.HelperMethods;

namespace Voyage.Business
{
    public class TicketsB
    {
        private readonly ILogger<TicketsB> _logger;
        private readonly UserManager<AppUser> _userManager;
        private TicketsD _ticketsD;
        private IHttpContextAccessor _httpContextAccessor;


        public TicketsB(UserManager<AppUser> userManager, TicketsD ticketsD, IHttpContextAccessor httpContextAccessor, ILogger<TicketsB> logger)
        {
            _userManager = userManager;
            _ticketsD = ticketsD;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<List<TicketDTO>> GetTickets(DateTime date)
        {
            return await _ticketsD.GetTickets(date);
        }

        public async Task<List<TicketDTO>> GetTickets(int sprintId)
        {
            return await _ticketsD.GetTickets(sprintId);
        }

        public async Task<TicketsDTO> GetPaginatedTickets(int sprintId, string sectionTitle, int pageNumber, int pageSize)
        {
            return await _ticketsD.GetPaginatedTickets(sprintId, sectionTitle, pageNumber, pageSize);
        }

        public async Task<TicketDTO?> GetTicket(int ticketId, decimal? ticketVersion)
        {
            TicketDTO? ticket = await _ticketsD.GetTicket(ticketId, ticketVersion);

            if (ticket != null)
            {
                var history = await _ticketsD.GetAllTicketVersions(ticketId);
                foreach (var h in history)
                {
                    if (!string.IsNullOrEmpty(h.TicketChangeAction))
                    {
                        h.TicketChangeAction = h.TicketChangeAction.Replace("\n", "<br />");
                    }

                    ticket.TicketVersionHistory = history;
                }
            }
            return ticket;
        }

        public async Task<bool> SaveTicket(TicketDTO ticketDTO)
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext!.User);

            if (user == null)
                throw new InvalidOperationException("Authenticated user not found.");
            else
                ticketDTO.CreatedBy = user.UserName ?? string.Empty;

            ticketDTO.Description = SanitizeHtmlForXSS(ticketDTO.Description);

            //Set section and statuses for specific statuses/sections
            if (ticketDTO.Status == null)
                return false;
            
            if (ticketDTO.Status == nameof(Constants.TicketStatus.Completed))
                ticketDTO.SectionTitle = ticketDTO.Status;

            if (ticketDTO.Status == nameof(Constants.TicketStatus.Discontinued) || ticketDTO.SectionTitle == nameof(Constants.TicketStatus.Discontinued))
            {
                ticketDTO.SectionTitle = nameof(Constants.TicketStatus.Discontinued);
                ticketDTO.Status = nameof(Constants.TicketStatus.Discontinued);
            }

            if (String.IsNullOrEmpty(ticketDTO.AssignedTo))
                ticketDTO.AssignedTo = nameof(Constants.Roles.Unassigned);

            ticketDTO.TicketChangeAction = await HandleTicketChangeAction(ticketDTO);

            return await _ticketsD.SaveTicket(ticketDTO);
        }

        public async Task<bool> DeleteTicket(int ticketId)
        {
            return await _ticketsD.DeleteTicket(ticketId);
        }

        public async Task<TicketDetailsDTO?> SaveTicketDetails(TicketDetailsDTO details)
        {
            details.Note = SanitizeHtmlForXSS(details.Note);

            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext!.User);

            if (user == null)
                throw new InvalidOperationException("Authenticated user not found.");

            if (details.TicketDetailsId != 0)
            {
                details.Author = user.UserName ?? string.Empty;
                details.ModifiedBy = user.UserName ?? string.Empty;
                details.ModifiedDate = DateTime.UtcNow;
            }
            else
            {
                details.Author = user.UserName ?? string.Empty;
                details.CreatedBy = user.UserName ?? string.Empty;
                details.CreatedDate = DateTime.UtcNow;
                details.ModifiedBy = string.Empty;
                details.ModifiedDate = null;
            }

            return await _ticketsD.SaveTicketDetails(details);
        }

        public async Task<string> HandleTicketChangeAction(TicketDTO ticketToSave)
        {
            TicketDTO? ticket = await GetTicket(ticketToSave.TicketId, null);

            if (ticket == null)
                return AddSpacesToSentence(Constants.TicketChangeAction.CreatedTicket.ToString());

            StringBuilder sb = new StringBuilder();

            if (ticketToSave.SectionTitle != ticket.SectionTitle)
            {
                sb.AppendLine(String.Format("<b>{0}:</b> {1}", 
                    AddSpacesToSentence(Constants.TicketChangeAction.SectionChanged.ToString()),
                    ticketToSave.SectionTitle)
                );
            }

            if (ticketToSave.Status != ticket.Status)
            {
                sb.AppendLine(String.Format("<b>{0}:</b> {1}",
                    AddSpacesToSentence(Constants.TicketChangeAction.StatusChanged.ToString()),
                    ticketToSave.Status)
                );
            }

            if (ticketToSave.Title != ticket.Title)
            {
                sb.AppendLine(String.Format("<b>{0}:</b> {1}",
                    AddSpacesToSentence(Constants.TicketChangeAction.TitleChanged.ToString()),
                    ticketToSave.Title)
                );
            }

            if (ticketToSave.Description != ticket.Description)
            {
                sb.AppendLine(String.Format("<b>{0}:</b> {1}",
                    AddSpacesToSentence(Constants.TicketChangeAction.DescriptionChanged.ToString()),
                    ticketToSave.Description)
                );
            }

            if (ticketToSave.AssignedTo != ticket.AssignedTo)
            {
                TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                string user = textInfo.ToTitleCase(ticketToSave.AssignedTo);
                string oldUser = textInfo.ToTitleCase(ticket.AssignedTo);

                sb.AppendLine(String.Format("<b>{0}:</b> {1}",
                    Constants.TicketChangeAction.Assigned.ToString(),
                    user)
                );
            }

            if (ticketToSave.PriorityLevel != ticket.PriorityLevel)
            {
                sb.AppendLine(String.Format("<b>{0}:</b> {1}",
                    AddSpacesToSentence(Constants.TicketChangeAction.PriorityLevelChanged.ToString()),
                    ticketToSave.PriorityLevel)
                );
            }

            if (ticketToSave.DueDate != ticket.DueDate)
            {
                sb.AppendLine(String.Format("<b>{0}:</b> {1}",
                    AddSpacesToSentence(Constants.TicketChangeAction.DueDateChanged.ToString()),
                    ticketToSave.DueDate)
                );
            }

            if (ticketToSave.ParentTicketId != ticket.ParentTicketId)
            {
                sb.AppendLine(String.Format("<b>{0}:</b> {1}",
                    AddSpacesToSentence(Constants.TicketChangeAction.ParentTicketChanged.ToString()),
                    ticketToSave.ParentTicketId)
                );
            }

            return sb.ToString();
        }

        public async Task<TicketSettingsDTO?> GetSettings()
        {
            return await _ticketsD.GetSettings();
        }

        public async Task<bool> SaveSettings(TicketSettingsDTO dto)
        {
            return await _ticketsD.SaveSettings(dto);
        }

        public List<SectionDTO> SetSectionsDevelopment()
        {
            List<SectionDTO> sections = new List<SectionDTO>();

            List<string> sectionTitles = new List<string>()
            {
                "Dev",
                "Review",
                "QA",
                //"UAT",
                "Completed",
                "Discontinued",
                "Backlog"
            };

            for (int i = 0; i < sectionTitles.Count(); i++)
            {
                SectionDTO section = new SectionDTO();
                section.SectionId = i + 1;
                section.Title = sectionTitles[i];
                section.SectionOrder = i + 1;

                sections.Add(section);
            }

            return sections;
        }


    }
}
