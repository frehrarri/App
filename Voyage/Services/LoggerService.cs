using System.Diagnostics;
using Voyage.Data;
using Voyage.Data.TableModels;
using Voyage.Models;
using Voyage.Utilities;

namespace Voyage.Services
{
    public class LoggerService
    {
        private readonly _AppDbContext _db;

        public LoggerService(_AppDbContext db)
        {
            _db = db;
        }

        public async Task Log(LogDTO dto)
        {
            try
            {
                await _db.Logs.AddAsync(new Log
                {
                    LogId = Guid.NewGuid(),
                    Type = dto.LogType,
                    Severity = dto.Severity,
                    StackTrace = dto.StackTrace,
                    ClientMessage = dto.ClientMessage,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = dto.CreatedBy
                });

                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logger Failed: StackTrace: {ex.StackTrace}");
            }

        }
    }

    public class LogDTO : BaseClass
    {
        public Constants.LogType LogType { get; set; }
        public Constants.LogSeverity Severity { get; set; }
        public string? StackTrace { get; set; }
        public string? ClientMessage { get; set; }
    }
}
