using Microsoft.EntityFrameworkCore;
using Voyage.Models;
using static Voyage.Utilities.Constants;

namespace Voyage.Data.TableModels
{
    public class Ticket : BaseClass, IModelBuilderEF
    {
        public Ticket()
        {
            Title = string.Empty;
            Status = TicketStatus.NotStarted.ToString();
            Description = string.Empty;
            AssignedTo = string.Empty;
            PriorityLevel = PriorityLevel.Low;
            SectionTitle = string.Empty;
            TicketDetails = new List<TicketDetails>();
            TicketChangeAction = string.Empty;
        }

        public int TicketId { get; set; } 
        public decimal TicketVersion { get; set; }
        public string TicketChangeAction { get; set; }

        public string Title { get; set; }
        public string Status { get; set; } 
        public string Description { get; set; } 
        public string AssignedTo { get; set; } 
        public PriorityLevel PriorityLevel { get; set; }

        public DateTime? DueDate { get; set; }
        public int? ParentTicketId { get; set; }
        public string SectionTitle { get; set; } 

        public int SprintId { get; set; }
        public DateTime? SprintStartDate { get; set; }
        public DateTime? SprintEndDate { get; set; }

        public ICollection<TicketDetails> TicketDetails { get; set; } 

        public void CreateEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ticket>()
                .ToTable("Tickets");

            modelBuilder.Entity<Ticket>()
                .HasKey(t => new { t.TicketId, t.TicketVersion });

            // Index for finding latest active tickets
            modelBuilder.Entity<Ticket>()
                .HasIndex(t => new { t.TicketId, t.IsLatest, t.IsActive });
        }
    }
}
