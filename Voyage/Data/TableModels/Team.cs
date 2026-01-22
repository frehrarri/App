using AngleSharp.Dom;
using Microsoft.EntityFrameworkCore;
using Voyage.Models;

namespace Voyage.Data.TableModels
{
    public class Team : BaseClass, IModelBuilderEF
    {
        public Guid TeamKey { get; set; }
        public int TeamId { get; set; }
        public decimal TeamVersion { get; set; }
        public string Name { get; set; } = string.Empty;

        #region FK
        public Guid? DepartmentKey { get; set; }
        public Department? Department { get; set; }
        public int? CompanyId { get; set; }
        public Company? Company { get; set; }
        public ICollection<TeamUserRole> TeamUserRoles { get; set; } = new List<TeamUserRole>();
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
                .Property(t => t.TeamVersion)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Team>()
                .HasIndex(t => t.TeamId);

            modelBuilder.Entity<Team>()
                .HasOne(t => t.Company)
                .WithMany(c => c.Teams)
                .HasForeignKey(t => t.CompanyId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Team>()
                .HasOne(t => t.Department)
                .WithMany(d => d.Teams)
                .HasForeignKey(t => t.DepartmentKey)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
