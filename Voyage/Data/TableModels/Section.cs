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

            modelBuilder.Entity<Section>()
                .HasOne(s => s.Settings)
                .WithMany(s => s.Sections)
                .HasForeignKey(s => s.SettingsId) // int FK
                .HasPrincipalKey(s => s.SettingsId) // points to alternate key
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
