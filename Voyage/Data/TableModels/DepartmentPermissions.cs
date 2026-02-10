using Microsoft.EntityFrameworkCore;
using Voyage.Models;

namespace Voyage.Data.TableModels
{
    public class DepartmentPermissions : IModelBuilderEF
    {
        public bool IsEnabled { get; set; }
        public int CompanyId { get; set; }


        public Guid DepartmentKey { get; set; }
        public Department Department { get; set; } = null!;

        public Permission Permission { get; set; } = null!;
        public Guid PermissionKey { get; set; }


        public void CreateEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DepartmentPermissions>()
                .ToTable("DepartmentPermissions");

            modelBuilder.Entity<DepartmentPermissions>()
                .HasKey(rp => new { rp.DepartmentKey, rp.PermissionKey });

            modelBuilder.Entity<DepartmentPermissions>()
                .HasIndex(tp => new { tp.DepartmentKey, tp.PermissionKey });

            modelBuilder.Entity<DepartmentPermissions>()
                .HasOne(rp => rp.Department)
                .WithMany(cr => cr.DepartmentPermissions)
                .HasForeignKey(rp => rp.DepartmentKey)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DepartmentPermissions>()
                .HasOne(dp => dp.Permission)
                .WithMany(p => p.DepartmentPermissions)
                .HasForeignKey(dp => dp.PermissionKey)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
