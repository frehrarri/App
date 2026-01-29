using Microsoft.EntityFrameworkCore;
using Voyage.Models;

namespace Voyage.Data.TableModels
{
    public class CompanyRole : BaseClass, IModelBuilderEF
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
            modelBuilder.Entity<CompanyRole>()
                .ToTable("CompanyRoles");

            // Surrogate PK
            modelBuilder.Entity<CompanyRole>()
                .HasKey(r => r.RoleKey);

            modelBuilder.Entity<CompanyRole>()
                .Property(r => r.RoleKey)
                .ValueGeneratedOnAdd();

            // FK to Company
            modelBuilder.Entity<CompanyRole>()
                .HasOne(r => r.Company)
                .WithMany(c => c.Roles)
                .HasForeignKey(r => r.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
