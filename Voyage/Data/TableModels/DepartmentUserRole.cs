using Microsoft.EntityFrameworkCore;
using Voyage.Models;

namespace Voyage.Data.TableModels
{
    public class DepartmentUserRole : BaseClass, IModelBuilderEF
    {
        public Guid DepartmentKey { get; set; }
        public int CompanyId { get; set; }
        public int EmployeeId { get; set; }
        public int RoleId { get; set; }

        // Navigation properties
        public Department Department { get; set; } = null!;
        public AppUser User { get; set; } = null!;
        public Role Role { get; set; } = null!;

        public void CreateEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DepartmentUserRole>()
                .ToTable("DepartmentUserRoles")
                .HasKey(dur => new { dur.DepartmentKey, dur.CompanyId, dur.EmployeeId, dur.RoleId });

            modelBuilder.Entity<DepartmentUserRole>()
                .HasOne(dur => dur.Department)
                .WithMany(d => d.DepartmentUserRoles)
                .HasForeignKey(dur => dur.DepartmentKey)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DepartmentUserRole>()
                .HasOne(dur => dur.User)
                .WithMany(u => u.DepartmentUserRoles)
                .HasForeignKey(dur => new { dur.CompanyId, dur.EmployeeId })
                .HasPrincipalKey(u => new { u.CompanyId, u.EmployeeId })
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DepartmentUserRole>()
                .HasOne(dur => dur.Role)
                .WithMany(r => r.DepartmentUserRoles)
                .HasForeignKey(dur => dur.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
