using Microsoft.EntityFrameworkCore;
using Voyage.Models;

namespace Voyage.Data.TableModels
{
    public class UserPermissions : IModelBuilderEF
    {
        public bool IsEnabled { get; set; }
        public int CompanyId { get; set; }

        public string Id { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;

        public Permission Permission { get; set; } = null!;
        public Guid PermissionKey { get; set; }

        public bool InheritIsDenied { get; set; } 


        public void CreateEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserPermissions>()
                .ToTable("UserPermissions");

            modelBuilder.Entity<UserPermissions>()
                .HasKey(rp => new { rp.Id, rp.PermissionKey });

            modelBuilder.Entity<UserPermissions>()
                .HasIndex(tp => new { tp.Id, tp.PermissionKey });

            modelBuilder.Entity<UserPermissions>()
                .HasOne(rp => rp.AppUser)
                .WithMany(cr => cr.UserPermissions)
                .HasForeignKey(rp => rp.Id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserPermissions>()
                .HasOne(dp => dp.Permission)
                .WithMany(p => p.UserPermissions)
                .HasForeignKey(dp => dp.PermissionKey)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
