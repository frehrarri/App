using Microsoft.EntityFrameworkCore;
using Voyage.Models;

namespace Voyage.Data.TableModels
{
    public class TeamPermissions : IModelBuilderEF
    {
        public bool IsEnabled { get; set; }

        public Guid TeamKey { get; set; }
        public Team Team { get; set; } = null!;

        public Permission Permission { get; set; } = null!;
        public Guid PermissionKey { get; set; }

        public void CreateEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TeamPermissions>()
                .ToTable("TeamPermissions");

            modelBuilder.Entity<TeamPermissions>()
                    .HasKey(rp => new { rp.TeamKey, rp.PermissionKey });

            modelBuilder.Entity<TeamPermissions>()
                .HasIndex(tp => new { tp.TeamKey, tp.PermissionKey });

            modelBuilder.Entity<TeamPermissions>()
                .HasOne(rp => rp.Team)
                .WithMany(cr => cr.TeamPermissions)
                .HasForeignKey(rp => rp.TeamKey)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TeamPermissions>()
                .HasOne(dp => dp.Permission)
                .WithMany(p => p.TeamPermissions)
                .HasForeignKey(dp => dp.PermissionKey)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
