using Microsoft.EntityFrameworkCore;
using Voyage.Models;

namespace Voyage.Data.TableModels
{
    public class Department : BaseClass, IModelBuilderEF
    {
        public Department() 
        { 
            Name = string.Empty;
        }

        public Guid DepartmentKey { get; set; }
        public int DepartmentId { get; set; }
        public decimal DepartmentVersion { get; set; }
        public string Name { get; set; }


        #region FK

        public int? CompanyId { get; set; }
        public Company? Company { get; set; }
        //public ICollection<Team> Teams { get; set; } = new List<Team>();
        public ICollection<DepartmentRole> DepartmentRoles { get; set; } = new List<DepartmentRole>();

        #endregion

        public void CreateEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>()
                 .ToTable("Department");

            modelBuilder.Entity<Department>()
                .HasKey(t => t.DepartmentKey);

            modelBuilder.Entity<Department>()
                .Property(t => t.DepartmentKey)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Department>()
                .Property(t => t.DepartmentVersion)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Department>()
                .HasIndex(d => new { d.DepartmentId, d.DepartmentVersion })
                .IsUnique();

            modelBuilder.Entity<Department>()
                .HasOne(d => d.Company)
                .WithMany(c => c.Departments)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
