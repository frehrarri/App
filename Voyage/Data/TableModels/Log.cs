using Microsoft.EntityFrameworkCore;
using Voyage.Models;
using Voyage.Utilities;

namespace Voyage.Data.TableModels
{
    public class Log : BaseClass, IModelBuilderEF
    {
        public Guid LogId { get; set; }
        public Constants.LogType Type { get; set; }
        public Constants.LogSeverity Severity { get; set; }
        public string? StackTrace { get; set; }
        public string? ClientMessage { get; set; }

        public void CreateEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Log>()
                .ToTable("Logs");

            modelBuilder.Entity<Log>()
                .HasKey(c => c.LogId);
        }
    }
}
