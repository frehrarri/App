using Microsoft.EntityFrameworkCore;
using Voyage.Models;
using Voyage.Utilities;
using static Voyage.Utilities.Constants;

namespace Voyage.Data.TableModels
{
    public class Settings : BaseClass, IModelBuilderEF
    {
        public Settings()
        {
            Sections = new List<Section>();
        }

        public int SettingsId { get; set; }
        public Feature Feature { get; set; }
        public RepeatSprint RepeatSprintOption { get; set; }
        public DateTime SprintStartDate { get; set; }
        public DateTime SprintEndDate { get; set; }
        
        public ICollection<Section> Sections { get; set; }

        public void CreateEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Settings>()
                .ToTable("Settings");

            modelBuilder.Entity<Settings>()
                .HasKey(t => t.SettingsId);

        }
    }
}
