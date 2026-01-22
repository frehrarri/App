using Microsoft.EntityFrameworkCore;
using Voyage.Models;

namespace Voyage.Data.TableModels
{
    public class Role : BaseClass, IModelBuilderEF
    {
        public int CompanyId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string RoleDescription { get; set; } = string.Empty;
        public int RoleId { get; set; }

        // Navigation properties
        public Company Company { get; set; } = null!;
        public ICollection<CompanyUserRole> CompanyUserRoles { get; set; } = new List<CompanyUserRole>();
        public ICollection<DepartmentUserRole> DepartmentUserRoles { get; set; } = new List<DepartmentUserRole>();
        public ICollection<TeamUserRole> TeamUserRoles { get; set; } = new List<TeamUserRole>();

        public void CreateEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>()
                            .ToTable("Roles")
                            .HasKey(r => r.RoleId);

            modelBuilder.Entity<Role>()
                .Property(r => r.RoleId)
                .ValueGeneratedOnAdd();

            // Role names must be unique within a company
            modelBuilder.Entity<Role>()
                .HasIndex(r => new { r.CompanyId, r.RoleName })
                .IsUnique();

            modelBuilder.Entity<Role>()
                .HasOne(r => r.Company)
                .WithMany(c => c.Roles)
                .HasForeignKey(r => r.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
    
}
