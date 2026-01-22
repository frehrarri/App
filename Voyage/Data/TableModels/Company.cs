using Microsoft.EntityFrameworkCore;
using Voyage.Models;

namespace Voyage.Data.TableModels
{
    public class Company : BaseClass, IModelBuilderEF
    {
        public int CompanyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string StreetAddress { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public int PostalCode { get; set; }
        public string Country { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public long Phone { get; set; }
        public string Email { get; set; } = string.Empty;

        #region Foreign Keys
        public ICollection<Department> Departments { get; set; } = new List<Department>();
        public ICollection<Team> Teams { get; set; } = new List<Team>();
        public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        public ICollection<Role> Roles { get; set; } = new List<Role>();
        public ICollection<CompanyUserRole> CompanyUserRoles { get; set; } = new List<CompanyUserRole>();
        #endregion

        public void CreateEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>()
                .ToTable("Company");

            modelBuilder.Entity<Company>()
                .HasKey(c => c.CompanyId);

            modelBuilder.Entity<Company>()
                .Property(c => c.CompanyId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Company>()
                .HasMany(c => c.Users)
                .WithOne(u => u.Company)
                .HasForeignKey(u => u.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    
    }
}