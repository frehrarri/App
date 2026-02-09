using Microsoft.EntityFrameworkCore;
using Voyage.Models;

namespace Voyage.Data.TableModels
{
    public class RolePermissions : IModelBuilderEF
    {
        public Guid RoleKey { get; set; }
        public bool IsEnabled { get; set; }

        public int CompanyId { get; set; }
        public CompanyRole CompanyRole { get; set; } = null!;

        public Permission Permission { get; set; } = null!;
        public Guid PermissionKey { get; set; }

        public void CreateEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RolePermissions>()
                .ToTable("RolePermissions");

            modelBuilder.Entity<RolePermissions>()
                .HasKey(rp => new { rp.CompanyId, rp.RoleKey, rp.PermissionKey });

            modelBuilder.Entity<RolePermissions>()
                .HasIndex(tp => new { tp.CompanyId, tp.RoleKey, tp.PermissionKey });

            modelBuilder.Entity<RolePermissions>()
                .HasOne(rp => rp.CompanyRole)
                .WithMany(cr => cr.RolePermissions)
                .HasForeignKey(rp => rp.RoleKey)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RolePermissions>()
                .HasOne(dp => dp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(dp => dp.PermissionKey)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
