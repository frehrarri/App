using Microsoft.EntityFrameworkCore;
using Voyage.Models;

namespace Voyage.Data.TableModels
{
    public class Team : BaseClass, IModelBuilderEF
    {
        public int TeamId { get; set; }
        public string Name { get; set; } = string.Empty;



        #region FK

        public int? DepartmentId { get; set; }
        public Department? Department { get; set; } = null!;

        public int? CompanyId { get; set; }
        public Company? Company { get; set; } = null!;

        #endregion


        public void CreateEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>()
                 .ToTable("Team");

            modelBuilder.Entity<Team>()
                .HasKey(t => t.TeamId);

            modelBuilder.Entity<Team>()
                .Property(t => t.TeamId)
                .ValueGeneratedOnAdd();

            // optional Department
            modelBuilder.Entity<Team>()
              .HasOne(t => t.Department)
              .WithMany(d => d.Teams)
              .HasForeignKey(t => t.DepartmentId)
              .OnDelete(DeleteBehavior.SetNull);

            // optional Company
            modelBuilder.Entity<Team>()
            .HasOne(t => t.Company)
            .WithMany(c => c.Teams)
            .HasForeignKey(t => t.CompanyId)
            .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
