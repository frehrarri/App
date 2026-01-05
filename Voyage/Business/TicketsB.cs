using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using System.Text.Encodings.Web;
using Voyage.Data;
using Voyage.Data.TableModels;
using Voyage.Models.App;
using Voyage.Models.DTO;
using Voyage.Utilities;
using static System.Collections.Specialized.BitVector32;
using static Voyage.Utilities.Constants;
using static Voyage.Utilities.HelperMethods;
using Section = Voyage.Models.App.Section;

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

        public async Task<TicketDTO?> GetTicket(int ticketId)
        {
            TicketDTO? ticketDTO = await _ticketsD.GetTicket(ticketId);

            return ticketDTO;
        }

        public async Task<bool> SaveTicket(TicketDTO ticketDTO)
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext!.User);

            if (user == null)
                throw new InvalidOperationException("Authenticated user not found.");
            else
                ticketDTO.CreatedBy = user.UserName ?? string.Empty;

            if (ticketDTO.Status == null)
                return false;

            //move tickets with a status to the associated section title
            if (ticketDTO.Status == nameof(Constants.TicketStatus.Completed) || ticketDTO.Status == nameof(Constants.TicketStatus.Discontinued))
                ticketDTO.SectionTitle = ticketDTO.Status;

            if (String.IsNullOrEmpty(ticketDTO.AssignedTo))
                ticketDTO.AssignedTo = nameof(Constants.Roles.Unassigned);

            return await _ticketsD.SaveTicket(ticketDTO);
        }

        public async Task<List<TicketDetailsDTO>> GetTicketDetails(int ticketId)
        {
            var dto = await _ticketsD.GetTicketDetails(ticketId);
            return dto;
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



        //public async Task<bool> DeleteTicket(int ticketId)
        //{
        //    return await _ticketsD.DeleteTicket(ticketId);
        //}

        //public void AssignTicket(string userId, int? ticketId)
        //{

        //}

        public List<Section> SetSectionsDevelopment()
        {
            List<Section> sections = new List<Section>();

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
                Section section = new Section();
                section.SectionId = i + 1;
                section.Title = sectionTitles[i];
                section.SectionOrder = i + 1;

                sections.Add(section);
            }

            return sections;
        }
    }
}
