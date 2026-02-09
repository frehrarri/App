using AngleSharp.Dom;
using Microsoft.EntityFrameworkCore;
using Voyage.Models;

namespace Voyage.Data.TableModels
{
    public class Team : BaseClass, IModelBuilderEF
    {
        public Guid TeamKey { get; set; }
        public int TeamId { get; set; }
        public string Name { get; set; } = string.Empty;

        #region FK

        //every company can have multiple teams
        public int CompanyId { get; set; }
        public Company? Company { get; set; }


        //every department can have multiple teams
        public Guid? DepartmentKey { get; set; }
        public Department? Department { get; set; }
     
        //every team can have multiple TeamUserRoles
        public ICollection<TeamUserRole> TeamUserRoles { get; set; } = new List<TeamUserRole>();

        public ICollection<TeamPermissions> TeamPermissions { get; set; } = new List<TeamPermissions>();

        //every team can have a single setting
        public Settings? Settings { get; set; }

        #endregion

        public void CreateEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>()
                .ToTable("Team");

            modelBuilder.Entity<Team>()
                .HasKey(t => t.TeamKey);

            modelBuilder.Entity<Team>()
                .Property(t => t.TeamKey)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Team>()
                .HasAlternateKey(t => new { t.CompanyId, t.TeamId });

            //FK to Company
            modelBuilder.Entity<Team>()
                .HasOne(t => t.Company)
                .WithMany(c => c.Teams)
                .HasForeignKey(t => t.CompanyId)
                .OnDelete(DeleteBehavior.SetNull);

            //FK to Department
            modelBuilder.Entity<Team>()
                .HasOne(t => t.Department)
                .WithMany(d => d.Teams)
                .HasForeignKey(t => t.DepartmentKey)
                .OnDelete(DeleteBehavior.SetNull);

            //FK to setting
            modelBuilder.Entity<Team>()
                .HasOne(t => t.Settings)
                .WithOne(s => s.Team)
                .HasForeignKey<Settings>(s => s.TeamKey)
                .IsRequired(false);
        }
    }
}
