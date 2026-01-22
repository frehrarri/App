using Microsoft.EntityFrameworkCore;
using Voyage.Models;

namespace Voyage.Data.TableModels
{
    public class Department : BaseClass, IModelBuilderEF
    {
        public Guid DepartmentKey { get; set; }
        public int DepartmentId { get; set; }
        public decimal DepartmentVersion { get; set; }
        public string Name { get; set; } = string.Empty;

        #region FK
        public int? CompanyId { get; set; }
        public Company? Company { get; set; }
        public ICollection<Team> Teams { get; set; } = new List<Team>();
        public ICollection<DepartmentUserRole> DepartmentUserRoles { get; set; } = new List<DepartmentUserRole>();
        #endregion

        public void CreateEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>()
                .ToTable("Department");

            modelBuilder.Entity<Department>()
                .HasKey(d => d.DepartmentKey);

            modelBuilder.Entity<Department>()
                .Property(d => d.DepartmentKey)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Department>()
                .HasIndex(t => t.DepartmentId);
    
            modelBuilder.Entity<Department>()
                .Property(d => d.DepartmentVersion)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Department>()
                .HasOne(d => d.Company)
                .WithMany(c => c.Departments)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
