using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Voyage.Models;

namespace Voyage.Data.TableModels
{
    public class DepartmentRole : BaseClass, IModelBuilderEF
    {
        public Guid DepartmentRoleKey { get; set; }  // surrogate PK

        #region FK

        public Guid DepartmentKey { get; set; }      // FK to Department.DepartmentKey
        public Department Department { get; set; } = null!;

        public string RoleId { get; set; } = null!;
        public AppRole Role { get; set; } = null!;

        #endregion

        public string Name { get; set; } = string.Empty;

        public void CreateEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DepartmentRole>()
                 .ToTable("DepartmentRole");

            // Surrogate PK
            modelBuilder.Entity<DepartmentRole>()
                .HasKey(dr => dr.DepartmentRoleKey);

            modelBuilder.Entity<DepartmentRole>()
                .Property(dr => dr.DepartmentRoleKey)
                .ValueGeneratedOnAdd();

            // FK to Department using DepartmentKey
            modelBuilder.Entity<DepartmentRole>()
                .HasOne(dr => dr.Department)
                .WithMany(d => d.DepartmentRoles)
                .HasForeignKey(dr => dr.DepartmentKey)
                .OnDelete(DeleteBehavior.Cascade);

            // FK to Role
            modelBuilder.Entity<DepartmentRole>()
                .HasOne(dr => dr.Role)
                .WithMany()
                .HasForeignKey(dr => dr.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Optional: add index for fast lookups by Department
            modelBuilder.Entity<DepartmentRole>()
                .HasIndex(dr => dr.DepartmentKey)
                .HasDatabaseName("IX_DepartmentRole_DepartmentKey");
        }
    }
}
