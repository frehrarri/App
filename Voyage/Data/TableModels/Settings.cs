using AngleSharp.Dom;
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

        public Guid SettingsKey { get; set; }
        public int SettingsId { get; set; }
        public decimal SettingsVersion { get; set; }
        public Feature Feature { get; set; }
        public RepeatSprint RepeatSprintOption { get; set; }
        public DateTime SprintStartDate { get; set; }
        public DateTime? SprintEndDate { get; set; }
        public int SprintId { get; set; }
        public SectionSettings SectionSetting { get; set; }

        #region FK

        public ICollection<Section> Sections { get; set; }

        //each team may have one setting
        public Guid? TeamKey { get; set; }
        public Team? Team { get; set; }

        //each department may have one setting
        public Guid? DepartmentKey { get; set; }
        public Department? Department { get; set; }

        //each user may have one setting this will be determined by individual basis and role level
        //roles are already a fk from AppUser to CompanyUserRoles, DepartmentUserRoles, TeamUserRoles
        public int? CompanyId { get; set; }
        public int? EmployeeId { get; set; }
        public int? RoleId { get; set; }

        public string? UserId { get; set; }//AppUser
        public AppUser? User { get; set; }


        #endregion

        public void CreateEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Settings>()
                .ToTable("Settings");

            modelBuilder.Entity<Settings>()
                .HasKey(t => t.SettingsKey);

            modelBuilder.Entity<Settings>()
                .Property(t => t.SettingsKey)
                .ValueGeneratedOnAdd();

            //versioning
            modelBuilder.Entity<Settings>()
                .HasAlternateKey(t => new { t.SettingsId, t.SettingsVersion });

            //Fk to team
            modelBuilder.Entity<Settings>()
                .HasOne(s => s.Team)
                .WithOne(t => t.Settings)
                .HasForeignKey<Settings>(s => s.TeamKey)
                .OnDelete(DeleteBehavior.Cascade);

            //Fk to Departments
            modelBuilder.Entity<Settings>()
                .HasOne(s => s.Department)
                .WithOne(t => t.Settings)
                .HasForeignKey<Settings>(s => s.DepartmentKey)
                .OnDelete(DeleteBehavior.Cascade);

            //FK to User
            modelBuilder.Entity<Settings>()
                .HasOne(s => s.User)
                .WithOne(u => u.Settings)
                .HasForeignKey<Settings>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
