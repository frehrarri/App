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

        //public int? DepartmentId { get; set; }
        //public Department? Department { get; set; } = null!;

        public int? CompanyId { get; set; }
        public Company? Company { get; set; }

        public ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();

        #endregion


        public void CreateEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>()
                 .ToTable("Team");

            modelBuilder.Entity<Team>()
                .HasKey(t => t.TeamKey );

            modelBuilder.Entity<Team>()
                .Property(t => t.TeamKey)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Team>()
                .Property(t => t.TeamVersion)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Team>()
                .HasIndex(d => new { d.TeamId, d.TeamVersion })
                .IsUnique();

            modelBuilder.Entity<Team>()
            .HasOne(t => t.Company)
            .WithMany(c => c.Teams)
            .HasForeignKey(t => t.CompanyId)
            .OnDelete(DeleteBehavior.SetNull);

            ////index for faster querying latest version
            //modelBuilder.Entity<Team>()
            //    .HasIndex(t => new { t.TeamId, t.IsLatest })
            //    .HasDatabaseName("IX_Team_Latest");
        }
    }
}
