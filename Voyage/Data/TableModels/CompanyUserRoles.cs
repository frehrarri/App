using Microsoft.EntityFrameworkCore;
using Voyage.Models;

namespace Voyage.Data.TableModels
{
    public class CompanyUserRole : BaseClass, IModelBuilderEF
    {
        public int CompanyId { get; set; }
        public int EmployeeId { get; set; }
        public int RoleId { get; set; }

        // Navigation properties
        public Company Company { get; set; } = null!;
        public AppUser User { get; set; } = null!;
        public Role Role { get; set; } = null!;

        public void CreateEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CompanyUserRole>()
                .ToTable("CompanyUserRoles")
                .HasKey(cur => new { cur.CompanyId, cur.EmployeeId, cur.RoleId });

            modelBuilder.Entity<CompanyUserRole>()
                .HasOne(cur => cur.Company)
                .WithMany(c => c.CompanyUserRoles)
                .HasForeignKey(cur => cur.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CompanyUserRole>()
                .HasOne(cur => cur.User)
                .WithMany(u => u.CompanyUserRoles)
                .HasForeignKey(cur => new { cur.CompanyId, cur.EmployeeId })
                .HasPrincipalKey(u => new { u.CompanyId, u.EmployeeId })
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CompanyUserRole>()
                .HasOne(cur => cur.Role)
                .WithMany(r => r.CompanyUserRoles)
                .HasForeignKey(cur => cur.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
