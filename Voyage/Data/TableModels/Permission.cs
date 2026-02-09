using Microsoft.EntityFrameworkCore;
using Voyage.Models;

namespace Voyage.Data.TableModels
{
    public class Permission : BaseClass, IModelBuilderEF
    {
        public Guid PermissionKey { get; set; }
        public string PermissionName { get; set; } = string.Empty;

        public ICollection<DepartmentPermissions> DepartmentPermissions { get; set; } = new List<DepartmentPermissions>();
        public ICollection<TeamPermissions> TeamPermissions { get; set; } = new List<TeamPermissions>();
        public ICollection<UserPermissions> UserPermissions { get; set; } = new List<UserPermissions>();
        public ICollection<RolePermissions> RolePermissions { get; set; } = new List<RolePermissions>();


        public void CreateEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Permission>()
                .ToTable("Permission");

            modelBuilder.Entity<Permission>()
                .HasKey(t => t.PermissionKey);

            modelBuilder.Entity<Permission>()
                .Property(t => t.PermissionKey)
                .ValueGeneratedOnAdd();
        }
    }
}
