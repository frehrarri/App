using Microsoft.EntityFrameworkCore;
using Voyage.Models;

namespace Voyage.Data.TableModels
{
    public class DepartmentUserRole : BaseClass, IModelBuilderEF
    {
        public Guid DepartmentUserRoleKey { get; set; }
        public decimal DeptUserRoleVersion { get; set; }
        public Guid DepartmentKey { get; set; }

        public int CompanyId { get; set; }
        public int EmployeeId { get; set; }
        public int RoleId { get; set; }

        public Department Department { get; set; } = null!;
        public AppUser User { get; set; } = null!;

        public Guid? RoleKey { get; set; }
        public CompanyRole Role { get; set; } = null!;

        public void CreateEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DepartmentUserRole>()
                .ToTable("DepartmentUserRoles")
                .HasKey(dur => dur.DepartmentUserRoleKey);

            //versioning
            modelBuilder.Entity<DepartmentUserRole>()
                .HasAlternateKey(dur => new { dur.DepartmentKey, dur.CompanyId, dur.EmployeeId, dur.RoleId, dur.DeptUserRoleVersion });

            // Department FK
            modelBuilder.Entity<DepartmentUserRole>()
                .HasOne(dur => dur.Department)
                .WithMany(d => d.DepartmentUserRoles)
                .HasForeignKey(dur => dur.DepartmentKey)
                .OnDelete(DeleteBehavior.Cascade);

            // User FK using alternate key
            modelBuilder.Entity<DepartmentUserRole>()
                .HasOne(dur => dur.User)
                .WithMany(u => u.DepartmentUserRoles)
                .HasForeignKey(dur => new { dur.CompanyId, dur.EmployeeId })
                .HasPrincipalKey(u => new { u.CompanyId, u.EmployeeId })
                .OnDelete(DeleteBehavior.Cascade);

            // Role FK
            modelBuilder.Entity<DepartmentUserRole>()
               .HasOne(dur => dur.Role)
               .WithMany(r => r.DepartmentUserRoles)
               .HasForeignKey(dur => new { dur.RoleId, dur.DeptUserRoleVersion })
               .HasPrincipalKey(r => new { r.RoleId, r.RoleVersion }) // matches the alternate key
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
