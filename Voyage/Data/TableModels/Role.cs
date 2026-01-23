using Microsoft.EntityFrameworkCore;
using Voyage.Models;

namespace Voyage.Data.TableModels
{
    public class Role : BaseClass, IModelBuilderEF
    {
        public Guid RoleKey { get; set; }             
        public int RoleId { get; set; }                
        public decimal RoleVersion { get; set; }      

        public int CompanyId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string RoleDescription { get; set; } = string.Empty;

        //every company can have many roles
        public Company Company { get; set; } = null!;

        //there can be many different roles within each UserRole table
        public ICollection<IndividualUserRole> IndividualUserRoles { get; set; } = new List<IndividualUserRole>();
        public ICollection<DepartmentUserRole> DepartmentUserRoles { get; set; } = new List<DepartmentUserRole>();
        public ICollection<TeamUserRole> TeamUserRoles { get; set; } = new List<TeamUserRole>();

        public void CreateEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>()
                .ToTable("Roles");

            // Surrogate PK
            modelBuilder.Entity<Role>()
                .HasKey(r => r.RoleKey);

            modelBuilder.Entity<Role>()
                .Property(r => r.RoleKey)
                .ValueGeneratedOnAdd();

            //versioning
            modelBuilder.Entity<Role>()
                .HasAlternateKey(r => new { r.CompanyId, r.RoleId, r.RoleVersion });

            // FK to Company
            modelBuilder.Entity<Role>()
                .HasOne(r => r.Company)
                .WithMany(c => c.Roles)
                .HasForeignKey(r => r.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
