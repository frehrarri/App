using Microsoft.EntityFrameworkCore;
using Voyage.Models;

namespace Voyage.Data.TableModels
{
    public class TeamUserRole : BaseClass, IModelBuilderEF
    {
        public Guid TeamUserRoleKey { get; set; } // surrogate PK
        public decimal TeamUserRoleVersion { get; set; } // versioning

        public Guid TeamKey { get; set; }
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

            modelBuilder.Entity<TeamUserRole>()
                .Property(tur => tur.TeamUserRoleVersion)
                .HasPrecision(5, 2);

            // Alternate key for uniqueness
            modelBuilder.Entity<TeamUserRole>()
                .HasAlternateKey(tur => new { tur.TeamKey, tur.CompanyId, tur.EmployeeId, tur.RoleId, tur.TeamUserRoleVersion });

            // FK to Team
            modelBuilder.Entity<TeamUserRole>()
                .HasOne(tur => tur.Team)
                .WithMany(t => t.TeamUserRoles)
                .HasForeignKey(tur => tur.TeamKey)
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
               .HasForeignKey(dur => new { dur.RoleId, dur.TeamUserRoleVersion })
               .HasPrincipalKey(r => new { r.RoleId, r.RoleVersion }) // matches the alternate key
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
