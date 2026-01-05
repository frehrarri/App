using AngleSharp.Dom;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Voyage.Data.TableModels;
using Voyage.Models.DTO;
using static Voyage.Utilities.Constants;

namespace Voyage.Data
{
    public class TicketsD
    {
        private _AppDbContext _db;
        private ILogger<TicketsD> _logger;

        public TicketsD(_AppDbContext db, ILogger<TicketsD> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<List<TicketDTO>> GetTickets(DateTime date)
        {
            try
            {
                return await _db.Tickets
                            .Where(t =>
                                t.IsActive == true
                                && t.IsLatest == true
                                && t.SprintStartDate <= date 
                                && t.SprintEndDate >= date)
                            .Select(t => new TicketDTO
                            {
                                TicketId = t.TicketId,
                                TicketVersion = t.TicketVersion,
                                Title = t.Title,
                                Status = t.Status,
                                Description = t.Description,
                                AssignedTo = t.AssignedTo,
                                PriorityLevel = t.PriorityLevel,
                                DueDate = t.DueDate,
                                ParentTicketId = t.ParentTicketId,
                                SectionTitle = t.SectionTitle,
                                SprintId = t.SprintId,
                                SprintStartDate = t.SprintStartDate,
                                SprintEndDate = t.SprintEndDate,
                                CreatedBy = t.CreatedBy,
                                CreatedDate = t.CreatedDate,
                                ModifiedBy = t.ModifiedBy,
                                ModifiedDate = t.ModifiedDate
                            })
                            .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex.ToString());
                throw;
            }   
        }

        public async Task<List<TicketDTO>> GetTickets(int sprintId)
        {
            try
            {
                return await _db.Tickets
                    .Where(t =>
                        t.IsActive == true
                        && t.IsLatest == true
                        && t.SprintId == sprintId)
                    .Select(t => new TicketDTO
                    {
                        TicketId = t.TicketId,
                        TicketVersion = t.TicketVersion,
                        Title = t.Title,
                        Status = t.Status,
                        Description = t.Description,
                        AssignedTo = t.AssignedTo,
                        PriorityLevel = t.PriorityLevel,
                        DueDate = t.DueDate,
                        ParentTicketId = t.ParentTicketId,
                        SectionTitle = t.SectionTitle,
                        SprintId = t.SprintId,
                        SprintStartDate = t.SprintStartDate,
                        SprintEndDate = t.SprintEndDate,
                        CreatedBy = t.CreatedBy,
                        CreatedDate = t.CreatedDate,
                        ModifiedBy = t.ModifiedBy,
                        ModifiedDate = t.ModifiedDate
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex.ToString());
                throw;
            }
        }

        public async Task<bool> SaveTicket(TicketDTO ticketDTO)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                bool isUpdate = ticketDTO.TicketId != 0;

                if (isUpdate)
                {
                    Ticket? existingTicket = await _db.Tickets
                        .Where(t => t.TicketId == ticketDTO.TicketId
                            && t.IsActive == true
                            && t.IsLatest == true)
                        .FirstOrDefaultAsync();

                    if (existingTicket == null)
                        throw new Exception("Ticket not found");

                    // Mark old version as not latest
                    existingTicket.IsLatest = false;
                    existingTicket.ModifiedBy = ticketDTO.CreatedBy;
                    existingTicket.ModifiedDate = DateTime.UtcNow;

                    // Create new version
                    Ticket newVersion = new Ticket
                    {
                        TicketId = existingTicket.TicketId,
                        TicketVersion = existingTicket.TicketVersion + 1,
                        IsActive = true,
                        IsLatest = true,
                        ParentTicketId = ticketDTO.ParentTicketId ?? existingTicket.ParentTicketId,
                        Title = ticketDTO.Title ?? existingTicket.Title,
                        SectionTitle = ticketDTO.SectionTitle ?? existingTicket.SectionTitle,
                        AssignedTo = ticketDTO.AssignedTo ?? existingTicket.AssignedTo,
                        Description = ticketDTO.Description ?? existingTicket.Description,
                        DueDate = ticketDTO.DueDate ?? existingTicket.DueDate,
                        PriorityLevel = ticketDTO.PriorityLevel,
                        Status = ticketDTO.Status ?? existingTicket.Status,
                        SprintId = ticketDTO.SprintId != 0 ? ticketDTO.SprintId : existingTicket.SprintId,
                        SprintStartDate = ticketDTO.SprintStartDate ?? existingTicket.SprintStartDate,
                        SprintEndDate = ticketDTO.SprintEndDate ?? existingTicket.SprintEndDate,
                        CreatedBy = existingTicket.CreatedBy,
                        CreatedDate = existingTicket.CreatedDate,
                        ModifiedBy = ticketDTO.CreatedBy,
                        ModifiedDate = DateTime.UtcNow
                    };

                    await _db.Tickets.AddAsync(newVersion);
                }
                else // Create new ticket
                {
                    int newTicketId = (await _db.Tickets.MaxAsync(t => (int?)t.TicketId) ?? 0) + 1;

                    Ticket newTicket = new Ticket
                    {
                        TicketId = newTicketId,
                        TicketVersion = 1.0M,
                        IsActive = true,
                        IsLatest = true,
                        ParentTicketId = ticketDTO.ParentTicketId,
                        Title = ticketDTO.Title,
                        SectionTitle = ticketDTO.SectionTitle,
                        AssignedTo = ticketDTO.AssignedTo,
                        Description = ticketDTO.Description,
                        DueDate = ticketDTO.DueDate,
                        PriorityLevel = ticketDTO.PriorityLevel,
                        Status = ticketDTO.Status,
                        SprintId = ticketDTO.SprintId,
                        SprintStartDate = ticketDTO.SprintStartDate,
                        SprintEndDate = ticketDTO.SprintEndDate,
                        CreatedBy = ticketDTO.CreatedBy,
                        CreatedDate = DateTime.UtcNow
                    };

                    await _db.Tickets.AddAsync(newTicket);
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex.ToString());
                await transaction.RollbackAsync();
                throw;
            }
        }


        //public async Task<List<TicketVM>> GetPaginatedTickets(string sectionTitle, int pageNumber, int pageSize)
        //{
        //    return await _db.Tickets
        //        .Where(t => t.IsActive &&
        //                    t.IsLatest &&
        //                    t.SectionTitle == sectionTitle)
        //        .OrderBy(t => t.PriorityLevel)
        //        .Skip((pageNumber - 1) * pageSize)
        //        .Take(pageSize)
        //        .Select(t => new TicketVM
        //        {
        //            TicketId = t.TicketId,
        //            ParentTicketId = t.ParentTicketId,
        //            Title = t.Title,
        //            SectionTitle = t.SectionTitle,
        //            AssignedTo = t.AssignedTo,
        //            CreatedBy = t.CreatedBy,
        //            Description = t.Description,
        //            CreatedDate = t.CreatedDate,
        //            DueDate = t.DueDate,
        //            ModifiedBy = t.ModifiedBy,
        //            ModifiedDate = t.ModifiedDate,
        //            PriorityLevel = t.PriorityLevel,
        //            Status = t.Status
        //        })
        //        .ToListAsync();
        //}

        public async Task<TicketDTO?> GetTicket(int ticketId)
        {
            try
            {
                return await _db.Tickets
                    .Include(t => t.TicketDetails)
                    .Where(t =>
                    t.TicketId == ticketId
                    && t.IsActive == true
                    && t.IsLatest == true)
                    .Select(t => new TicketDTO
                    {
                        TicketId = t.TicketId,
                        TicketVersion = t.TicketVersion,
                        Title = t.Title,
                        Status = t.Status,
                        Description = t.Description,
                        AssignedTo = t.AssignedTo,
                        PriorityLevel = t.PriorityLevel,
                        DueDate = t.DueDate,
                        ParentTicketId = t.ParentTicketId,
                        SectionTitle = t.SectionTitle,
                        SprintId = t.SprintId,
                        SprintStartDate = t.SprintStartDate,
                        SprintEndDate = t.SprintEndDate,

                        TicketDetailsDTOs = t.TicketDetails.Select(td => new TicketDetailsDTO
                        {
                            TicketDetailsId = td.TicketDetailsId,
                            TicketId = td.TicketId,
                            TicketVersion = td.TicketVersion,
                            Note = td.Note,
                            Author = td.Author,
                            CreatedDate = td.CreatedDate,
                            CreatedBy = td.CreatedBy
                        }).ToList()

                    }).SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex.ToString());
                throw;
            }
        }

        public async Task<List<TicketDetailsDTO>> GetTicketDetails(int ticketId)
        {
            try
            {
                return await _db.TicketDetails
                    .Include(td => td.Ticket)
                    .Where(td => 
                        td.TicketId == ticketId
                        && td.IsLatest == true
                        && td.IsActive == true)
                    .Select(td => new TicketDetailsDTO
                    {
                        TicketId = td.TicketId,
                        TicketDetailsId = td.TicketDetailsId,
                        Author = td.Author,
                        CreatedDate = td.CreatedDate,
                        ModifiedDate = td.ModifiedDate,
                        Note = td.Note
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex.ToString());
                throw;
            }
        }

        public async Task<TicketDetailsDTO?> SaveTicketDetails(TicketDetailsDTO details)
        {
            try
            {
                if (!details.TicketId.HasValue)
                    return null;

                decimal maxVersion = await _db.TicketDetails
                    .Where(td => td.TicketId == details.TicketId.Value)
                    .Select(td => td.TicketVersion)
                    .MaxAsync() ?? 0.0M;

                decimal newVersion = maxVersion == 0 ? 1.0M : maxVersion + 1;

                // update existing note
                if (details.TicketDetailsId > 0)
                {
                    var existing = await _db.TicketDetails.FirstOrDefaultAsync(td => td.TicketDetailsId == details.TicketDetailsId);

                    if (existing == null)
                        return null;

                    // Mark old version as not latest
                    existing.IsLatest = false;
                    existing.ModifiedBy = details.ModifiedBy;
                    existing.ModifiedDate = DateTime.UtcNow;

                    // Create new version of the detail
                    var newVersionDetail = new TicketDetails
                    {
                        TicketId = existing.TicketId,
                        TicketVersion = newVersion,
                        Note = details.Note,
                        Author = details.Author,
                        CreatedBy = existing.CreatedBy,
                        CreatedDate = existing.CreatedDate,
                        ModifiedBy = details.ModifiedBy,
                        ModifiedDate = DateTime.UtcNow,
                        IsActive = true,
                        IsLatest = true
                    };

                    _db.TicketDetails.Add(newVersionDetail);
                    await _db.SaveChangesAsync();

                    return new TicketDetailsDTO
                    {
                        TicketDetailsId = newVersionDetail.TicketDetailsId,
                        TicketId = newVersionDetail.TicketId,
                        TicketVersion = newVersionDetail.TicketVersion,
                        Note = newVersionDetail.Note,
                        Author = newVersionDetail.Author,
                        CreatedBy = newVersionDetail.CreatedBy,
                        CreatedDate = newVersionDetail.CreatedDate,
                        ModifiedBy = newVersionDetail.ModifiedBy,
                        ModifiedDate = newVersionDetail.ModifiedDate
                    };
                }

                // create new note
                var ticketDetails = new TicketDetails
                {
                    TicketId = details.TicketId.Value,
                    TicketVersion = newVersion,
                    Note = details.Note,
                    CreatedBy = details.CreatedBy,
                    CreatedDate = DateTime.UtcNow,
                    Author = details.Author,
                    IsActive = true,
                    IsLatest = true
                };

                _db.TicketDetails.Add(ticketDetails);
                await _db.SaveChangesAsync();

                return new TicketDetailsDTO
                {
                    TicketDetailsId = ticketDetails.TicketDetailsId,
                    TicketId = ticketDetails.TicketId,
                    TicketVersion = ticketDetails.TicketVersion,
                    Note = ticketDetails.Note,
                    Author = ticketDetails.Author,
                    CreatedBy = ticketDetails.CreatedBy,
                    CreatedDate = ticketDetails.CreatedDate,
                    ModifiedBy = ticketDetails.ModifiedBy,
                    ModifiedDate = ticketDetails.ModifiedDate
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex.ToString());
                throw;
            }
        }


        //public async Task<bool> DeleteTicket(int ticketId)
        //{
        //    try
        //    {
        //        ////Ticket? existingTicket = await GetTicket(ticketId);
        //        //if (existingTicket != null)
        //        //{
        //        //    existingTicket.IsActive = false;
        //        //    existingTicket.IsLatest = false;
        //        //    _db.Tickets.Update(existingTicket);
        //        //}

        //        await _db.SaveChangesAsync();
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.ToString());
        //        return false;
        //    }
        //}






    }
}
