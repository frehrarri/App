using Microsoft.EntityFrameworkCore;
using Voyage.Models;

namespace Voyage.Data.TableModels
{
    public class TeamMember : BaseClass, IModelBuilderEF
    {
        public Guid TeamMemberKey { get; set; }  // surrogate PK

        // Team reference
        public Guid TeamKey { get; set; }        // FK to Team.TeamKey
        public Team Team { get; set; } = null!;

        // Identity user reference
        public int EmployeeId { get; set; }
        public int CompanyId { get; set; }
        public AppUser User { get; set; } = null!;

        public void CreateEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TeamMember>()
                .ToTable("TeamMembers");

            // Surrogate PK
            modelBuilder.Entity<TeamMember>()
                .HasKey(tm => tm.TeamMemberKey);

            modelBuilder.Entity<TeamMember>()
                .Property(tm => tm.TeamMemberKey)
                .ValueGeneratedOnAdd();

            // FK to Team using TeamKey (surrogate)
            modelBuilder.Entity<TeamMember>()
                .HasOne(tm => tm.Team)
                .WithMany(t => t.TeamMembers)
                .HasForeignKey(tm => tm.TeamKey)
                .OnDelete(DeleteBehavior.Cascade);

            // FK to User (composite CompanyId + EmployeeId is fine)
            modelBuilder.Entity<TeamMember>()
                .HasOne(tm => tm.User)
                .WithMany(u => u.TeamMembers)
                .HasForeignKey(tm => new { tm.CompanyId, tm.EmployeeId })
                .HasPrincipalKey(u => new { u.CompanyId, u.EmployeeId })
                .OnDelete(DeleteBehavior.Cascade);

            // Index for faster queries per Team
            modelBuilder.Entity<TeamMember>()
                .HasIndex(tm => tm.TeamKey)
                .HasDatabaseName("IX_TeamMember_TeamKey");
        }
    }
}
