using AngleSharp.Dom;
using Microsoft.EntityFrameworkCore;
using Voyage.Models;

namespace Voyage.Data.TableModels
{
    public class TeamMember : BaseClass, IModelBuilderEF
    {

        //team
        public int TeamId { get; set; }
        public decimal TeamVersion { get; set; }
        public Team Team { get; set; } = null!;

        //identity user
        public int EmployeeId { get; set; }
        public int CompanyId { get; set; }
        public AppUser User { get; set; } = null!;


        public void CreateEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TeamMember>()
                .ToTable("TeamMembers");

            modelBuilder.Entity<TeamMember>()
                .HasKey(tm => new
                {
                    tm.TeamId,
                    tm.TeamVersion,
                    tm.CompanyId,
                    tm.EmployeeId
                });

            modelBuilder.Entity<TeamMember>()
                .HasOne(tm => tm.Team)
                .WithMany(t => t.TeamMembers)
                .HasForeignKey(tm => new { tm.TeamId, tm.TeamVersion })
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TeamMember>()
                .HasOne(tm => tm.User)
                .WithMany(u => u.TeamMembers)
                .HasForeignKey(tm => new { tm.CompanyId, tm.EmployeeId })
                .HasPrincipalKey(u => new { u.CompanyId, u.EmployeeId })
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TeamMember>()
                .HasIndex(tm => new { tm.TeamId, tm.TeamVersion })
                .HasDatabaseName("IX_TeamMember_TeamVersion");
        }


    }
}
