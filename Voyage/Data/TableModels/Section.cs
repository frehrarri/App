using Microsoft.EntityFrameworkCore;
using Voyage.Models;
using static Voyage.Utilities.Constants;

namespace Voyage.Data.TableModels
{
    public class Section : BaseClass, IModelBuilderEF
    {
        public Section()
        {
            Title = string.Empty;
            Settings = null!;
        }

        public int SectionId { get; set; }
        public string Title { get; set; }
        public int SectionOrder { get; set; }

        public int SettingsId {  get; set; }
        public Settings Settings { get; set; }

        public void CreateEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Section>()
                .ToTable("Sections");

            modelBuilder.Entity<Section>()
                .HasKey(t => t.SectionId);

            modelBuilder.Entity<Settings>()
                .HasMany(s => s.Sections)
                .WithOne(sec => sec.Settings)
                .HasForeignKey(sec => sec.SettingsId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
