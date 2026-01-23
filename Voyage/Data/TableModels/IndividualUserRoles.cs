using Microsoft.EntityFrameworkCore;
using Voyage.Models;

namespace Voyage.Data.TableModels
{
    public class IndividualUserRole : BaseClass, IModelBuilderEF
    {
        public Guid IndivUserRoleKey { get; set; } // surrogate PK
        public decimal IndivUserRoleVersion { get; set; } // versioning

        public int CompanyId { get; set; }
        public int EmployeeId { get; set; }
        public int RoleId { get; set; }

        // Navigation properties
        public Company Company { get; set; } = null!;
        public AppUser User { get; set; } = null!;
        public Role Role { get; set; } = null!;

        public void CreateEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IndividualUserRole>()
                .ToTable("IndividualUserRoles");

            modelBuilder.Entity<IndividualUserRole>()
                .HasKey(cur => cur.IndivUserRoleKey);

            modelBuilder.Entity<IndividualUserRole>()
                .Property(cur => cur.IndivUserRoleKey)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<IndividualUserRole>()
                .Property(cur => cur.IndivUserRoleVersion)
                .HasPrecision(5, 2);

            // Alternate key for uniqueness
            modelBuilder.Entity<IndividualUserRole>()
                .HasAlternateKey(cur => new { cur.CompanyId, cur.EmployeeId, cur.RoleId, cur.IndivUserRoleVersion });

            // FK to Company
            modelBuilder.Entity<IndividualUserRole>()
                .HasOne(cur => cur.Company)
                .WithMany(c => c.IndividualUserRoles)
                .HasForeignKey(cur => cur.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            // FK to User
            modelBuilder.Entity<IndividualUserRole>()
                .HasOne(cur => cur.User)
                .WithMany(u => u.IndividualUserRoles)
                .HasForeignKey(cur => new { cur.CompanyId, cur.EmployeeId })
                .HasPrincipalKey(u => new { u.CompanyId, u.EmployeeId })
                .OnDelete(DeleteBehavior.Cascade);

            // FK to Role
            modelBuilder.Entity<IndividualUserRole>()
                .HasOne(cur => cur.Role)
                .WithMany(r => r.IndividualUserRoles)
               .HasForeignKey(dur => new { dur.RoleId, dur.IndivUserRoleVersion })
               .HasPrincipalKey(r => new { r.RoleId, r.RoleVersion }) // matches the alternate key
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
