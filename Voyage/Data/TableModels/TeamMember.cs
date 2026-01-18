using AngleSharp.Dom;
using Microsoft.EntityFrameworkCore;
using Voyage.Models;

namespace Voyage.Data.TableModels
{
    public class TeamMember : BaseClass, IModelBuilderEF
    {

        //team
        public int TeamId { get; set; }
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
                    tm.CompanyId,
                    tm.EmployeeId 
                });

            modelBuilder.Entity<TeamMember>()
                .HasOne(tm => tm.Team)
                .WithMany(t => t.TeamMembers)
                .HasForeignKey(tm => tm.TeamId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TeamMember>()
                .HasOne(tm => tm.User)
                .WithMany(u => u.TeamMembers)
                .HasForeignKey(tm => new { tm.CompanyId, tm.EmployeeId })
                .HasPrincipalKey(u => new { u.CompanyId, u.EmployeeId })
                .OnDelete(DeleteBehavior.Cascade);


        }


    }
}
