using Microsoft.EntityFrameworkCore;
using Voyage.Models;

namespace Voyage.Data.TableModels
{
    public class SectionHistory : BaseClass, IModelBuilderEF
    {
        public SectionHistory()
        {
            Title = string.Empty;
        }

        public Guid SectionHistoryKey { get; set; }
        public int SectionId { get; set; }
        public string Title { get; set; }
        public int SectionOrder { get; set; }

        public Guid SettingsHistoryKey { get; set; }
        public SettingsHistory SettingsHistory { get; set; } = null!;

        public void CreateEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SectionHistory>()
                .ToTable("SectionsHistory");

            modelBuilder.Entity<SectionHistory>()
                .HasKey(t => t.SectionHistoryKey);

            modelBuilder.Entity<SectionHistory>()
                .Property(t => t.SectionHistoryKey)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<SectionHistory>()
                .HasOne(s => s.SettingsHistory)
                .WithMany(s => s.Sections)
                .HasForeignKey(s => s.SettingsHistoryKey)
                .HasPrincipalKey(s => s.SettingsHistoryKey)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
