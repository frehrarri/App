using Microsoft.EntityFrameworkCore;
using Voyage.Models;
using static Voyage.Utilities.Constants;

namespace Voyage.Data.TableModels
{
    public class Ticket : BaseClass, IModelBuilderEF
    {
        public int TicketId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string AssignedTo { get; set; } = string.Empty;
        public string PriorityLevel { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; } = null;
        public int? ParentTicketId { get; set; } = null;
        public string SectionTitle { get; set; } = string.Empty;
        public int SprintId { get; set; }
        public string StartDate {  get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;

        public void CreateEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ticket>()
                 .ToTable("Tickets");

            modelBuilder.Entity<Ticket>()
                .HasKey(t => t.TicketId);

            modelBuilder.Entity<Ticket>()
                .Property(t => t.TicketId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Ticket>()
                .Property(t => t.SprintId)
                .ValueGeneratedOnAdd();
        }
    }
}
