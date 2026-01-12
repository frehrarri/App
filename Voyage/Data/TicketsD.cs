using AngleSharp.Dom;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Voyage.Business;
using Voyage.Data.TableModels;
using Voyage.Models.App;
using Voyage.Models.DTO;
using Voyage.Utilities;
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

        public async Task<TicketsDTO> GetPaginatedTickets(int sprintId, string sectionTitle, int pageNumber, int pageSize)
        {
            TicketsDTO ticketsDTO = new TicketsDTO();

            List<TicketDTO> tickets = await GetTickets(sprintId);

            ticketsDTO.Tickets = tickets
                .Where(t => t.SectionTitle == sectionTitle)
                .OrderByDescending(t => t.PriorityLevel)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
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
                .ToList();

            ticketsDTO.ResultCount = ticketsDTO.Tickets.Count();

            return ticketsDTO;
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
                        ModifiedDate = DateTime.UtcNow,
                        TicketChangeAction = ticketDTO.TicketChangeAction
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
                        CreatedDate = DateTime.UtcNow,
                        TicketChangeAction = ticketDTO.TicketChangeAction
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

        public async Task<TicketDTO?> GetTicket(int ticketId, decimal? ticketVersion = null)
        {
            try
            {
                // Get the specific version or latest
                Ticket? t;
                if (ticketVersion.HasValue)
                {
                    t = await _db.Tickets
                        .Where(t => t.TicketId == ticketId
                            && t.TicketVersion == ticketVersion.Value
                            && t.IsActive == true)
                        .FirstOrDefaultAsync();
                }
                else
                {
                    t = await _db.Tickets
                        .Where(t => t.TicketId == ticketId
                            && t.IsLatest == true
                            && t.IsActive == true)
                        .FirstOrDefaultAsync();
                }

                if (t == null)
                    return null;

                var ticketDetails = await _db.TicketDetails
                    .Where(td => td.TicketId == ticketId
                        && td.TicketVersion <= t.TicketVersion //get all historical details that are up to the current version
                        && td.IsActive == true) 
                    .OrderBy(td => td.CreatedDate)
                    .ToListAsync();

                TicketDTO ticketDTO = new TicketDTO
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
                    ModifiedDate = t.ModifiedDate,
                    IsLatest = t.IsLatest,
                    IsActive = t.IsActive,
                    TicketChangeAction = t.TicketChangeAction,
                    TicketDetailsDTOs = ticketDetails
                        .Select(td => new TicketDetailsDTO
                        {
                            TicketDetailsId = td.TicketDetailsId,
                            TicketId = td.TicketId,
                            TicketVersion = td.TicketVersion,
                            Note = td.Note,
                            Author = td.Author,
                            CreatedDate = td.CreatedDate,
                            CreatedBy = td.CreatedBy,
                            ModifiedDate = td.ModifiedDate,
                            ModifiedBy = td.ModifiedBy
                        })
                        .ToList()
                };

                return ticketDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting ticket {TicketId}, version {TicketVersion}", ticketId, ticketVersion);
                throw;
            }
        }

        public async Task<List<TicketVersionDTO>> GetAllTicketVersions(int ticketId)
        {
            try
            {
                return await _db.Tickets
                    .Where(t => t.TicketId == ticketId)
                    .Select(t => new TicketVersionDTO
                    { 
                        TicketVersion = t.TicketVersion,
                        TicketChangeAction = t.TicketChangeAction
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

                var ticket = await _db.Tickets
                    .Where(t => t.TicketId == details.TicketId.Value
                        && t.IsActive == true
                        && t.IsLatest == true)
                    .FirstOrDefaultAsync();

                if (ticket == null)
                    throw new Exception("Ticket not found");

                // Update existing note (on current version only)
                if (details.TicketDetailsId > 0)
                {
                    var existing = await _db.TicketDetails
                        .FirstOrDefaultAsync(td => td.TicketDetailsId == details.TicketDetailsId);

                    if (existing == null)
                        return null;

                    existing.Note = details.Note;
                    existing.Author = details.Author;
                    existing.ModifiedBy = details.ModifiedBy;
                    existing.ModifiedDate = DateTime.UtcNow;

                    await _db.SaveChangesAsync();

                    return new TicketDetailsDTO
                    {
                        TicketDetailsId = existing.TicketDetailsId,
                        TicketId = existing.TicketId,
                        TicketVersion = existing.TicketVersion,
                        Note = existing.Note,
                        Author = existing.Author,
                        CreatedBy = existing.CreatedBy,
                        CreatedDate = existing.CreatedDate,
                        ModifiedBy = existing.ModifiedBy,
                        ModifiedDate = existing.ModifiedDate
                    };
                }

                // Create new note (on current ticket version)
                var ticketDetails = new TicketDetails
                {
                    TicketId = details.TicketId.Value,
                    TicketVersion = ticket.TicketVersion,
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
                _logger.LogError(ex, "Error saving ticket details");
                throw;
            }
        }
        public async Task<bool> DeleteTicket(int ticketId)
        {
            try
            {
                var ticket = await _db.Tickets
                   .Include(t => t.TicketDetails)
                   .Where(t =>
                       t.TicketId == ticketId
                       && t.IsActive == true
                       && t.IsLatest == true)
                   .SingleOrDefaultAsync();

                if (ticket != null)
                {
                    ticket.IsActive = false;
                    ticket.IsLatest = false;
                    _db.Tickets.Update(ticket);
                }

                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting ticket");
                return false;
            }
        }

        public async Task<TicketSettingsDTO?> GetSettings()
        {
            try
            {
                TicketSettingsDTO? dto = new TicketSettingsDTO();

                dto = await _db.Settings.Include(s => s.Sections)
                    .Where(s => s.Feature == Constants.Feature.Tickets
                        && s.IsActive == true
                        && s.IsLatest == true)
                    .Select(s => new TicketSettingsDTO()
                    {
                        SettingsId = s.SettingsId,
                        RepeatSprintOption = s.RepeatSprintOption,
                        SprintEnd = s.SprintEndDate,
                        SprintStart = s.SprintStartDate,

                        Sections = s.Sections.Select(s => new SectionDTO
                        {
                            SectionId = s.SectionId,
                            Title = s.Title,
                            SectionOrder = s.SectionOrder,
                        }).ToList()
                    })
                    .SingleOrDefaultAsync();

                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ticket settings.");
                return null;
            }
            
        }

        public async Task<bool> SaveSettings(TicketSettingsDTO dto)
        {
            try
            {
                Settings? settings;
                bool isUpdate = true;

                settings = await _db.Settings
                    .Include(s => s.Sections)
                   .Where(s =>
                       s.SettingsId == dto.SettingsId
                       && s.Feature == Constants.Feature.Tickets
                       && s.IsActive == true
                       && s.IsLatest == true)
                   .SingleOrDefaultAsync();


                if (settings == null) //create new
                {
                    settings = new Settings();
                    isUpdate = false;
                }
                else //empty previous sections to be replaced
                {
                    settings.Sections.Clear();
                }

                settings.RepeatSprintOption = dto.RepeatSprintOption;
                settings.SprintEndDate = dto.SprintEnd!.Value;
                settings.SprintStartDate = dto.SprintStart!.Value;
                settings.Feature = Constants.Feature.Tickets;
                settings!.IsLatest = true;
                settings.IsActive = true;

                settings.Sections = dto.Sections.Select(s => new Section()
                {
                    Settings = settings,
                    Title = s.Title,
                    SectionOrder = s.SectionOrder
                })
                .ToList();

                //tracked entities are updated automatically so dont need update
                if (!isUpdate) 
                {
                    await _db.Settings.AddAsync(settings);
                }

                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving ticket settings.");
                return false;
            }
        }

    }
}
