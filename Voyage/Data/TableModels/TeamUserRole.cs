using Microsoft.EntityFrameworkCore;
using Voyage.Models;

namespace Voyage.Data.TableModels
{
    public class TeamUserRole : BaseClass, IModelBuilderEF
    {
        public Guid TeamUserRoleKey { get; set; } // surrogate PK
        public Guid TeamKey { get; set; }
        public int TeamId { get; set; }
        public int CompanyId { get; set; }
        public int EmployeeId { get; set; }
        public int RoleId { get; set; }

        public Team Team { get; set; } = null!;
        public AppUser User { get; set; } = null!;
        public CompanyRole Role { get; set; } = null!;

        public void CreateEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TeamUserRole>()
                .ToTable("TeamUserRoles");

            modelBuilder.Entity<TeamUserRole>()
                .HasKey(tur => tur.TeamUserRoleKey);

            modelBuilder.Entity<TeamUserRole>()
                .Property(tur => tur.TeamUserRoleKey)
                .ValueGeneratedOnAdd();

            // Alternate key for uniqueness
            modelBuilder.Entity<TeamUserRole>()
                .HasAlternateKey(tur => new { tur.TeamId, tur.CompanyId, tur.EmployeeId});

            // FK to Team
            modelBuilder.Entity<TeamUserRole>()
                .HasOne(tur => tur.Team)
                .WithMany(t => t.TeamUserRoles)
                .HasForeignKey(tur => new { tur.CompanyId, tur.TeamId })
                .HasPrincipalKey(t => new { t.CompanyId, t.TeamId })
                .OnDelete(DeleteBehavior.Cascade);

            // FK to User
            modelBuilder.Entity<TeamUserRole>()
                .HasOne(tur => tur.User)
                .WithMany(u => u.TeamUserRoles)
                .HasForeignKey(tur => new { tur.CompanyId, tur.EmployeeId })
                .HasPrincipalKey(u => new { u.CompanyId, u.EmployeeId })
                .OnDelete(DeleteBehavior.Cascade);

            // FK to Role
            modelBuilder.Entity<TeamUserRole>()
                .HasOne(tur => tur.Role)
                .WithMany(r => r.TeamUserRoles)
                .HasForeignKey(tur => new { tur.CompanyId, tur.RoleId })
                .HasPrincipalKey(r => new { r.CompanyId, r.RoleId })
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
