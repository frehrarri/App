using Microsoft.EntityFrameworkCore;
using Voyage.Models;
using static Voyage.Utilities.Constants;

namespace Voyage.Data.TableModels
{
    public class SettingsHistory : BaseClass, IModelBuilderEF
    {
        public SettingsHistory()
        {
            Sections = new List<SectionHistory>();
        }

        public Guid SettingsHistoryKey { get; set; }
        public decimal SettingsVersion { get; set; }
        public Feature Feature { get; set; }
        public int RepeatSprintOption { get; set; }
        public DateTime? SprintStartDate { get; set; }
        public int SprintLength { get; set; }
        public int SprintId { get; set; }
        public int SectionSetting { get; set; }

        #region FK

        public ICollection<SectionHistory> Sections { get; set; }

        public Guid? TeamKey { get; set; }
        public Guid? DepartmentKey { get; set; }
        public int? CompanyId { get; set; }
        public int? EmployeeId { get; set; }
        public int? RoleId { get; set; }
        public string? UserId { get; set; }//AppUser

        public int SettingsId { get; set; }
        public Settings Settings { get; set; } = null!;


        #endregion

        public void CreateEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SettingsHistory>()
                .ToTable("SettingsHistory");

            modelBuilder.Entity<SettingsHistory>()
                .HasKey(v => v.SettingsHistoryKey);

            modelBuilder.Entity<SettingsHistory>()
                .Property(t => t.SettingsHistoryKey)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<SettingsHistory>()
                .HasAlternateKey(v => new { v.SettingsId, v.SettingsVersion });

            modelBuilder.Entity<SettingsHistory>()
                .HasOne(v => v.Settings)
                .WithMany(s => s.SettingsHistory)
                .HasForeignKey(v => v.SettingsId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SettingsHistory>()
                .HasMany(sh => sh.Sections)
                .WithOne(s => s.SettingsHistory)
                .HasForeignKey(s => s.SettingsHistoryKey)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
