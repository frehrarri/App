using Microsoft.EntityFrameworkCore;
using Voyage.Models;

namespace Voyage.Data.TableModels
{
    public class TicketDetails : BaseClass, IModelBuilderEF
    {
        public TicketDetails()
        {
            Note = string.Empty;
            Author = string.Empty;
            Ticket = null!;
        }

        public int TicketDetailsId { get; set; } 
        public int TicketId { get; set; }
        public decimal? TicketVersion { get; set; }

        public string Note { get; set; }
        public string Author { get; set; }

        public Ticket Ticket { get; set; }

        public void CreateEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TicketDetails>()
                .ToTable("TicketDetails");

            modelBuilder.Entity<TicketDetails>()
                .HasKey(td => td.TicketDetailsId);

            modelBuilder.Entity<TicketDetails>()
                .Property(td => td.TicketDetailsId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<TicketDetails>()
                        .HasOne(td => td.Ticket)
                        .WithMany(t => t.TicketDetails)
                        .HasForeignKey(td => new { td.TicketId, td.TicketVersion })
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TicketDetails>()
                .HasIndex(td => td.TicketId);


        }
    }
}
