using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Voyage.Models;

namespace Voyage.Data.TableModels
{
    public class DepartmentRole : BaseClass, IModelBuilderEF
    {
        #region FK

        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!;

        public string RoleId { get; set; } = null!;
        public IdentityRole Role { get; set; } = null!;

        #endregion

        public string Name { get; set; } = string.Empty;


        public void CreateEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DepartmentRole>()
                 .ToTable("DepartmentRole");

            modelBuilder.Entity<DepartmentRole>()
                .HasKey(dr => new { dr.DepartmentId, dr.RoleId });

            modelBuilder.Entity<DepartmentRole>()
                    .HasOne(dr => dr.Department)
                    .WithMany(d => d.DepartmentRoles)
                    .HasForeignKey(dr => dr.DepartmentId)
                    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DepartmentRole>()
                .HasOne(dr => dr.Role)
                .WithMany()
                .HasForeignKey(dr => dr.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}