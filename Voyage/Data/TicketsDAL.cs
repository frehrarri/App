using AngleSharp.Dom;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Voyage.Business;
using Voyage.Data.TableModels;
using Voyage.Models.App;
using Voyage.Models.DTO;
using Voyage.Services;
using Voyage.Utilities;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Voyage.Utilities.Constants;

namespace Voyage.Data
{
    public class TicketsDAL
    {
        private _AppDbContext _db;
        private LoggerService _logger;

        public TicketsDAL(_AppDbContext db, LoggerService logger)
        {
            _db = db;
            _logger = logger;
        }

        //public async Task<List<TicketDTO>> GetTickets(DateTime date)
        //{
        //    try
        //    {
        //        return await _db.Tickets
        //                    .Where(t =>
        //                        t.IsActive == true
        //                        && t.IsLatest == true
        //                        && t.SprintStartDate <= date) 
        //                    .Select(t => new TicketDTO
        //                    {
        //                        TicketId = t.TicketId,
        //                        TicketVersion = t.TicketVersion,
        //                        Title = t.Title,
        //                        Status = t.Status,
        //                        Description = t.Description,
        //                        AssignedTo = t.AssignedTo,
        //                        PriorityLevel = t.PriorityLevel,
        //                        DueDate = t.DueDate,
        //                        ParentTicketId = t.ParentTicketId,
        //                        SectionTitle = t.SectionTitle,
        //                        SprintId = t.SprintId,
        //                        SprintStartDate = t.SprintStartDate,
        //                        SprintLength = t.SprintLength,
        //                        CreatedBy = t.CreatedBy,
        //                        CreatedDate = t.CreatedDate,
        //                        ModifiedBy = t.ModifiedBy,
        //                        ModifiedDate = t.ModifiedDate
        //                    })
        //                    .ToListAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        string message = "Could not retrieve tickets";

        //        await _logger.Log(new LogDTO
        //        {
        //            LogType = LogType.Error,
        //            Severity = LogSeverity.High,
        //            StackTrace = ex.StackTrace,
        //            ClientMessage = message
        //        });

        //        return null!;
        //    }   
        //}

        public async Task<List<TicketDTO>> GetTickets(int? sprintId)
        {
            try
            {
                if (sprintId == 0) 
                    sprintId = null;

                return await _db.Tickets
                    .Where(t =>
                        t.IsActive == true
                        && t.IsLatest == true
                        && (t.SprintId == sprintId ||
                        sprintId == null))
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
                        SprintLength = t.SprintLength,
                        CreatedBy = t.CreatedBy,
                        CreatedDate = t.CreatedDate,
                        ModifiedBy = t.ModifiedBy,
                        ModifiedDate = t.ModifiedDate
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                string message = "Could not retrieve tickets";

                await _logger.Log(new LogDTO
                {
                    LogType = LogType.Error,
                    Severity = LogSeverity.High,
                    StackTrace = ex.StackTrace,
                    ClientMessage = message
                });

                throw;
            }
        }

        public async Task<TicketsDTO> GetPaginatedTickets(int sprintId, string sectionTitle, int pageNumber, int pageSize)
        {
            try
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
                        SprintLength = t.SprintLength,
                        CreatedBy = t.CreatedBy,
                        CreatedDate = t.CreatedDate,
                        ModifiedBy = t.ModifiedBy,
                        ModifiedDate = t.ModifiedDate
                    })
                    .ToList();

                ticketsDTO.ResultCount = ticketsDTO.Tickets.Count();

                return ticketsDTO;
            }
            catch (Exception ex)
            {
                string message = "Could not retrieve tickets";

                await _logger.Log(new LogDTO
                {
                    LogType = LogType.Error,
                    Severity = LogSeverity.High,
                    StackTrace = ex.StackTrace,
                    ClientMessage = message
                });

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
                        .Where(t => t.CompanyId == ticketDTO.CompanyId
                            && t.TicketId == ticketDTO.TicketId
                            && t.IsActive == true
                            && t.IsLatest == true)
                        .SingleOrDefaultAsync();

                    if (existingTicket == null)
                        throw new Exception("Ticket not found");

                    // Mark old version as not latest
                    existingTicket.IsLatest = false;
                    existingTicket.ModifiedBy = ticketDTO.CreatedBy;
                    existingTicket.ModifiedDate = DateTime.UtcNow;

                    // Create new version
                    Ticket newVersion = new Ticket
                    {
                        CompanyId = ticketDTO.CompanyId,
                        TicketId = existingTicket.TicketId,
                        TicketVersion = existingTicket.TicketVersion + 1,
                        IsActive = true,
                        IsLatest = true,
                        ParentTicketId = ticketDTO.ParentTicketId ?? existingTicket.ParentTicketId,
                        Title = ticketDTO.Title ?? existingTicket.Title,
                        SectionTitle = ticketDTO.SectionTitle ?? existingTicket.SectionTitle,
                        SectionId = ticketDTO.SectionId != 0 ? ticketDTO.SectionId : existingTicket.SectionId,
                        AssignedTo = ticketDTO.AssignedTo ?? existingTicket.AssignedTo,
                        Description = ticketDTO.Description ?? existingTicket.Description,
                        DueDate = ticketDTO.DueDate ?? existingTicket.DueDate,
                        PriorityLevel = ticketDTO.PriorityLevel,
                        Status = ticketDTO.Status ?? existingTicket.Status,
                        SprintId = ticketDTO.SprintId != 0 ? ticketDTO.SprintId : existingTicket.SprintId,
                        SprintStartDate = ticketDTO.SprintStartDate ?? existingTicket.SprintStartDate,
                        SprintLength = ticketDTO.SprintLength,
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
                    var maxValues = await _db.Tickets
                        .GroupBy(t => 1) // dummy grouping to aggregate across all rows
                        .Select(g => new
                        {
                            MaxTicketId = g.Max(t => (int?)t.TicketId),
                            MaxSprintId = g.Max(t => (int?)t.SprintId)
                        })
                        .FirstOrDefaultAsync();

                    int newTicketId = (maxValues?.MaxTicketId ?? 0) + 1;
                    int newSprintId = (maxValues?.MaxSprintId ?? 0) + 1;

                    Ticket newTicket = new Ticket
                    {
                        CompanyId = ticketDTO.CompanyId,
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
                        SprintId = ticketDTO.SprintId == 0 ? newSprintId : ticketDTO.SprintId,
                        SprintStartDate = ticketDTO.SprintStartDate,
                        SprintLength = ticketDTO.SprintLength,
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
                string message = "Could not save ticket";

                await _logger.Log(new LogDTO
                {
                    LogType = LogType.Error,
                    Severity = LogSeverity.High,
                    StackTrace = ex.StackTrace,
                    ClientMessage = message
                });

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
                    SectionId = t.SectionId,
                    SprintId = t.SprintId,
                    SprintStartDate = t.SprintStartDate,
                    SprintLength = t.SprintLength,
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
                string message = "Could not retrieve ticket";

                await _logger.Log(new LogDTO
                {
                    LogType = LogType.Error,
                    Severity = LogSeverity.High,
                    StackTrace = ex.StackTrace,
                    ClientMessage = message
                });

                throw;
            }
        }

        //public async Task<List<TicketVersionDTO>> GetAllTicketVersions(int ticketId)
        //{
        //    try
        //    {
        //        return await _db.Tickets
        //            .Where(t => t.TicketId == ticketId)
        //            .Select(t => new TicketVersionDTO
        //            { 
        //                TicketVersion = t.TicketVersion,
        //                TicketChangeAction = t.TicketChangeAction
        //            })
        //            .ToListAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        string message = "Could not retrieve ticket history";

        //        await _logger.Log(new LogDTO
        //        {
        //            LogType = LogType.Error,
        //            Severity = LogSeverity.High,
        //            StackTrace = ex.StackTrace,
        //            ClientMessage = message
        //        });

        //        return null!;
        //    }
        //}

        public async Task<TicketDetailsDTO?> SaveTicketDetails(TicketDetailsDTO details)
        {
            try
            {
                if (!details.TicketId.HasValue)
                    return null;

                var ticket = await _db.Tickets
                    .Include(t => t.TicketDetails)
                    .Where(t => t.CompanyId == details.CompanyId
                        && t.TicketId == details.TicketId.Value
                        && t.IsActive == true
                        && t.IsLatest == true)
                    .FirstOrDefaultAsync();

                if (ticket == null)
                    return null;

                // Update existing note (on current version only)
                if (details.TicketDetailsId > 0)
                {
                    var existing = ticket.TicketDetails.FirstOrDefault(td => td.TicketDetailsId == details.TicketDetailsId);

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
                string message = "Could not save ticket";

                await _logger.Log(new LogDTO
                {
                    LogType = LogType.Error,
                    Severity = LogSeverity.High,
                    StackTrace = ex.StackTrace,
                    ClientMessage = message
                });

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
                string message = "Could not delete ticket";

                await _logger.Log(new LogDTO
                {
                    LogType = LogType.Error,
                    Severity = LogSeverity.High,
                    StackTrace = ex.StackTrace,
                    ClientMessage = message
                });

                throw;
            }
        }

        public async Task<TicketSettingsDTO> GetSettings(int companyId)
        {
            try
            {
                var dto = await _db.Settings.Include(s => s.Sections)
                    .Include(s => s.SettingsHistory)
                    .Where(s => s.Feature == Constants.Feature.Tickets
                        && s.IsActive == true
                        && s.CompanyId == companyId) //get company settings and global settings
                    .Select(s => new TicketSettingsDTO()
                    {
                        DepartmentKey = s.DepartmentKey,
                        TeamKey = s.TeamKey,
                        CompanyId = companyId,
                        SettingsId = s.SettingsId,
                        RepeatSprintOption = s.RepeatSprintOption,
                        SettingsVersion = s.SettingsVersion,
                        SectionSetting = (SectionSettings)s.SectionSetting,
                        SprintLength = s.SprintLength,
                        SprintStart = s.SprintStartDate,
                        SprintId = s.SprintId,
                        CreatedBy = s.CreatedBy,
                        CreatedDate = s.CreatedDate,
                        ModifiedBy = s.ModifiedBy,
                        ModifiedDate = s.ModifiedDate,

                        Sections = s.Sections.Select(s => new SectionDTO
                        {
                            SectionId = s.SectionId,
                            Title = s.Title,
                            SectionOrder = s.SectionOrder,
                        }).ToList(),

                        SettingsHistory = s.SettingsHistory.Select(sh => new TicketSettingsDTO()
                        {
                            DepartmentKey = sh.DepartmentKey,
                            TeamKey = sh.TeamKey,
                            CompanyId = companyId,
                            SettingsId = sh.SettingsId,
                            RepeatSprintOption = sh.RepeatSprintOption,
                            SettingsVersion = sh.SettingsVersion,
                            SectionSetting = (SectionSettings)s.SectionSetting,
                            SprintLength = sh.SprintLength,
                            SprintStart = sh.SprintStartDate,
                            SprintEnd = sh.SprintEndDate,
                            SprintId = sh.SprintId,
                            CreatedBy = sh.CreatedBy,
                            CreatedDate = sh.CreatedDate,
                            ModifiedBy = sh.ModifiedBy,
                            ModifiedDate = sh.ModifiedDate,

                            Sections = sh.Sections.Select(sh => new SectionDTO
                            {
                                SectionId = sh.SectionId,
                                Title = sh.Title,
                                SectionOrder = sh.SectionOrder,
                            }).ToList()

                        }).ToList()
                    })
                    .SingleOrDefaultAsync();

                return dto!;
            }
            catch (Exception ex)
            {
                string message = "Could not retrieve ticket settings";

                await _logger.Log(new LogDTO
                {
                    LogType = LogType.Error,
                    Severity = LogSeverity.High,
                    StackTrace = ex.StackTrace,
                    ClientMessage = message
                });

                throw;
            }
        }

        public async Task SaveSettings(TicketSettingsDTO dto)
        {
            try
            {
                //List<Settings> settingsList = new List<Settings>();
                Settings? settings;
                bool isUpdate = true;
                var date = DateTime.UtcNow;

                settings = await _db.Settings
                   .Include(s => s.Sections)
                   .Where(s =>
                       //s.SettingsId == dto.SettingsId
                       s.CompanyId == dto.CompanyId
                       && (s.DepartmentKey == dto.DepartmentKey || dto.DepartmentKey == null)
                       && (s.TeamKey == dto.TeamKey || dto.TeamKey == null)
                       && s.Feature == Constants.Feature.Tickets
                       && s.IsActive == true
                       && s.IsLatest == true)
                   .SingleOrDefaultAsync();

                if (settings == null) //create new
                {
                    settings = new Settings();
                    settings.CompanyId = dto.CompanyId;
                    settings.SprintId = 1;
                    settings.SettingsId = 1;
                    settings.SettingsVersion = 1.0M;

                    settings!.IsLatest = true;
                    settings.IsActive = true;
                    settings.CreatedBy = dto.CreatedBy;
                    settings.CreatedDate = date;

                    isUpdate = false;
                }
                else //update
                {
                    //sprint end date has passed so the sprint autoincrements from UpdateSprint method
                    if (dto.IsEndDatePassed)
                        settings.SprintId = dto.SprintId;

                    //if there is a change to sprint intervals boundaries then create a new sprint id
                    else if (settings.RepeatSprintOption != dto.RepeatSprintOption || settings.SprintLength != dto.SprintLength || settings.SprintStartDate != dto.SprintStart)
                        settings.SprintId++;
                        

                    settings.SettingsVersion++;

                    settings.Sections.Clear();

                    settings.ModifiedBy = dto.CreatedBy;
                    settings.ModifiedDate = date;
                }
                
                settings.DepartmentKey = dto.DepartmentKey;
                settings.TeamKey = dto.TeamKey;
                settings.SectionSetting = (int)dto.SectionSetting;
                settings.RepeatSprintOption = (int)dto.RepeatSprintOption;
                settings.SprintLength = dto.SprintLength;
                settings.Feature = Constants.Feature.Tickets;

                if (dto.SprintStart != null)
                    settings.SprintStartDate = DateTime.SpecifyKind(dto.SprintStart!.Value, DateTimeKind.Utc);

                settings.Sections = dto.Sections.Select(s => new Section()
                {
                    Settings = settings,
                    Title = s.Title,
                    SectionOrder = s.SectionOrder,
                    IsActive = true,
                    IsLatest = true,
                    CreatedDate = date,
                    CreatedBy = dto.CreatedBy
                })
                .ToList();

                //tracked entities are updated automatically so dont need update
                if (!isUpdate) 
                {
                    await _db.Settings.AddAsync(settings);
                }

                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                string message = "Could not save ticket settings";

                await _logger.Log(new LogDTO
                {
                    LogType = LogType.Error,
                    Severity = LogSeverity.High,
                    StackTrace = ex.StackTrace,
                    ClientMessage = message
                });

                throw;
            }
        }

        public async Task SaveSettingsHistory(List<TicketSettingsDTO> dtos)
        {
            try
            {
                List<SettingsHistory> settings = new List<SettingsHistory>();
                DateTime today = DateTime.UtcNow;

                TicketSettingsDTO latest = await GetSettings(dtos[0].CompanyId);
                if (latest == null)
                    return;

                int previousSprintId = 0;
                foreach (var dto in dtos)
                {
                    SettingsHistory setting = new SettingsHistory();
                    setting.CompanyId = latest.CompanyId;
                    setting.EmployeeId = latest.EmployeeId;
                    setting.DepartmentKey = latest.DepartmentKey;
                    setting.TeamKey = latest.TeamKey;

                    setting.SettingsId = latest.SettingsId;
                    setting.SprintLength = latest.SprintLength;
                    setting.SectionSetting = (int)latest.SectionSetting;
                    setting.Feature = Constants.Feature.Tickets;
                    setting.RepeatSprintOption = latest.RepeatSprintOption;

                    setting.Sections = latest.Sections.Select(s => new SectionHistory()
                    {
                        Title = s.Title,
                        SectionOrder = s.SectionOrder,
                        IsActive = true,
                        IsLatest = true,
                        CreatedDate = today,
                        CreatedBy = dto.CreatedBy
                    }).ToList();

                    //either sprint is updating from elapsed time or stays the same because save is a version change
                    setting.SprintId = dto.SprintId > 0 ? dto.SprintId : latest.SprintId;
                    previousSprintId = setting.SprintId;

                    setting.SettingsVersion = latest.SettingsVersion;

                    //either sprint start date is updating from elapsed time or stays the same because save is a version change
                    setting.SprintStartDate = dto.SprintStart.HasValue ? dto.SprintStart : latest.SprintStart;
                    setting.SprintEndDate = today;

                    setting.CreatedBy = latest.CreatedBy;
                    setting.CreatedDate = today;
                    setting.IsLatest = false;
                    setting.IsActive = true;

                    //modified by user
                    if (dto.SprintId == previousSprintId)
                    {
                        setting.ModifiedBy = dto.CreatedBy;
                        setting.ModifiedDate = today;
                    }

                    settings.Add(setting);

                    //modified by user
                    if (dto.SprintId == previousSprintId)
                    {
                        latest.SettingsVersion++;
                    }
                        
                }

                await _db.AddRangeAsync(settings);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                string message = "Could not save ticket settings history";

                await _logger.Log(new LogDTO
                {
                    LogType = LogType.Error,
                    Severity = LogSeverity.High,
                    StackTrace = ex.StackTrace,
                    ClientMessage = message
                });

                throw;
            }
        }

        public async Task MarkCompleted(List<TicketDTO> dto)
        {
            try
            {
                var ticketIds = dto.Select(t => t.TicketId).ToList();

                var tickets = await _db.Tickets.Where(t => ticketIds.Contains(t.TicketId)).ToListAsync();
                foreach(var t in tickets)
                {
                    t.Status = Constants.TicketStatus.Completed.ToString();
                    t.SectionTitle = Constants.TicketStatus.Completed.ToString();
                    t.ModifiedDate = DateTime.UtcNow;
                    t.ModifiedBy = dto[0].ModifiedBy;
                }

                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                string message = "Could not mark ticket completed";

                await _logger.Log(new LogDTO
                {
                    LogType = LogType.Error,
                    Severity = LogSeverity.High,
                    StackTrace = ex.StackTrace,
                    ClientMessage = message
                });
                throw;
            }
        }

        public async Task Discontinue(List<TicketDTO> dto)
        {
            try
            {
                var ticketIds = dto.Select(t => t.TicketId).ToList();

                var tickets = await _db.Tickets.Where(t => ticketIds.Contains(t.TicketId)).ToListAsync();
                foreach (var t in tickets)
                {
                    t.Status = Constants.TicketStatus.Discontinued.ToString();
                    t.SectionTitle = Constants.TicketStatus.Discontinued.ToString();
                    t.ModifiedDate = DateTime.UtcNow;
                    t.ModifiedBy = dto[0].ModifiedBy;
                }

                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                string message = "Could not mark ticket discontinued";

                await _logger.Log(new LogDTO
                {
                    LogType = LogType.Error,
                    Severity = LogSeverity.High,
                    StackTrace = ex.StackTrace,
                    ClientMessage = message
                });

                throw;
            }
        }

    }
}
