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

        public int DepartmentId { get; set; }
        public string Name { get; set; }


        #region FK

        public int? CompanyId { get; set; }
        public Company? Company { get; set; } = null!;
        public ICollection<Team> Teams { get; set; } = new List<Team>();
        public ICollection<DepartmentRole> DepartmentRoles { get; set; } = new List<DepartmentRole>();

        #endregion

        public void CreateEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>()
                 .ToTable("Department");

            modelBuilder.Entity<Department>()
                .HasKey(t => t.DepartmentId);

            modelBuilder.Entity<Department>()
                .Property(t => t.DepartmentId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Department>()
                .HasOne(d => d.Company)
                .WithMany(c => c.Departments)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
