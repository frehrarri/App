using Microsoft.AspNetCore.Identity;
using Voyage.Data;
using Voyage.Data.TableModels;
using Voyage.Models.App;
using Voyage.Utilities;

namespace Voyage.Business
{
    public class TicketsB
    {
        private readonly UserManager<AppUser> _userManager;
        TicketsD _ticketsD;

        public TicketsB(UserManager<AppUser> userManager, TicketsD ticketsD)
        {
            _userManager = userManager;
            _ticketsD = ticketsD;
        }

        public async Task<List<TicketVM>> GetTickets()
        {
            List<Ticket> tickets = await _ticketsD.GetTickets();

            return tickets.Select(t => new TicketVM
            {
                TicketId = t.TicketId,
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
                Status = t.Status
            }).ToList();
        }

        public async Task<TicketVM> GetTicket(int ticketId)
        {
            Ticket? t = await _ticketsD.GetTicket(ticketId);

            return new TicketVM
            {
                TicketId = t.TicketId,
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
                Status = t.Status
            };
        }

        public async Task<bool> SaveTicket(Ticket ticket)
        {
            if (ticket.Status == nameof(Constants.TicketStatus.Completed) || ticket.Status == nameof(Constants.TicketStatus.Discontinued))
            {
                ticket.SectionTitle = ticket.Status;
            }

            if (String.IsNullOrEmpty(ticket.AssignedTo))
            {
                ticket.AssignedTo = nameof(Constants.Roles.Unassigned);
            }

            return await _ticketsD.SaveTicket(ticket);
        }

        public async Task<bool> DeleteTicket(int ticketId)
        {
            return await _ticketsD.DeleteTicket(ticketId);
        }

        public void AssignTicket(string userId, int? ticketId)
        {

        }
    }
}
