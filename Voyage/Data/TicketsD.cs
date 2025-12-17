using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Voyage.Business;
using Voyage.Data.TableModels;
using Voyage.Models.App;
using static Voyage.Utilities.Constants;

namespace Voyage.Data
{
    public class TicketsD
    {
        _AppDbContext _db;

        public TicketsD(_AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Ticket>> GetTickets()
        {
            try
            {
                return await _db.Tickets.Where(t => t.IsActive && t.IsLatest).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
            
        }

        public async Task<Ticket?> GetTicket(int ticketId)
        {
            try
            {
                return await _db.Tickets.Where(t => t.TicketId == ticketId && t.IsActive && t.IsLatest).SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }


        public async Task<bool> SaveTicket(Ticket ticket)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                Ticket newTicket = new Ticket();
                newTicket.ParentTicketId = ticket.ParentTicketId;
                newTicket.Title = ticket.Title;
                newTicket.SectionTitle = ticket.SectionTitle;
                newTicket.AssignedTo = ticket.AssignedTo;
                newTicket.Description = ticket.Description;
                newTicket.DueDate = ticket.DueDate;
                newTicket.PriorityLevel = ticket.PriorityLevel;
                newTicket.Status = ticket.Status;
                newTicket.IsActive = true;
                newTicket.IsLatest = true;

                bool isUpdate = ticket.TicketId != 0 ? true : false;

                if (isUpdate) //update old ticket and create new one
                {
                    Ticket? existingTicket = await GetTicket(ticket.TicketId);
                    if (existingTicket != null)
                    {
                        //add new ticket
                        newTicket.ParentTicketId = ticket.ParentTicketId ?? existingTicket.ParentTicketId;
                        newTicket.Title = string.IsNullOrEmpty(ticket.Title) ? existingTicket.Title : ticket.Title;
                        newTicket.SectionTitle = string.IsNullOrEmpty(ticket.SectionTitle) ? existingTicket.SectionTitle : ticket.SectionTitle;
                        newTicket.AssignedTo = string.IsNullOrEmpty(ticket.AssignedTo) ? existingTicket.AssignedTo : ticket.AssignedTo;
                        newTicket.Description = string.IsNullOrEmpty(ticket.Description) ? existingTicket.Description : ticket.Description;
                        newTicket.DueDate = ticket.DueDate == null ? existingTicket.DueDate : ticket.DueDate;
                        newTicket.PriorityLevel = string.IsNullOrEmpty(ticket.PriorityLevel) ? existingTicket.PriorityLevel : ticket.PriorityLevel;
                        newTicket.Status = string.IsNullOrEmpty(ticket.Status) ? existingTicket.Status : ticket.Status;

                        newTicket.CreatedBy = existingTicket.CreatedBy;
                        newTicket.CreatedDate = existingTicket.CreatedDate;
                        newTicket.ModifiedBy = ticket.ModifiedBy;
                        newTicket.ModifiedDate = DateTime.Now.ToString();
                        await _db.Tickets.AddAsync(newTicket);
                        await _db.SaveChangesAsync();

                        //set old ticket to be inactive
                        existingTicket.IsActive = false;
                        existingTicket.IsLatest = false;

                        _db.Tickets.Update(existingTicket);
                        await _db.SaveChangesAsync();
                    }
                }
                else //add new ticket
                {
                    newTicket.CreatedBy = ticket.CreatedBy;
                    newTicket.CreatedDate = DateTime.Now.ToString();
                    await _db.Tickets.AddAsync(newTicket);
                    await _db.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public async Task<bool> DeleteTicket(int ticketId)
        {
            try
            {
                Ticket? existingTicket = await GetTicket(ticketId);
                if (existingTicket != null)
                {
                    existingTicket.IsActive = false;
                    existingTicket.IsLatest = false;
                    _db.Tickets.Update(existingTicket);
                }

                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }


    }
}
