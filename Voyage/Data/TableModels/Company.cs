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

        //every company can be comprised of multiple departments, teams, and company user roles for heirarchial assignment
        public ICollection<Department> Departments { get; set; } = new List<Department>();
        public ICollection<Team> Teams { get; set; } = new List<Team>();
        public ICollection<IndividualUserRole> IndividualUserRoles { get; set; } = new List<IndividualUserRole>();

        //every company must be comprised of many users and roles (unassigned/principal) which are set on registration
        public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
        public ICollection<CompanyRole> Roles { get; set; } = new List<CompanyRole>();
       
        //every company can have many tickets
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        
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

            //FK Users Collection
            modelBuilder.Entity<Company>()
                .HasMany(c => c.Users)
                .WithOne(u => u.Company)
                .HasForeignKey(u => u.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            //FK IndividualUserRoles Collection
            modelBuilder.Entity<Company>()
                 .HasMany(c => c.IndividualUserRoles)
                 .WithOne(u => u.Company)
                 .HasForeignKey(u => u.CompanyId)
                 .OnDelete(DeleteBehavior.Cascade);

            //FK Departments Collection
            modelBuilder.Entity<Company>()
                .HasMany(c => c.Departments)
                .WithOne(d => d.Company)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            //FK Teams Collection
            modelBuilder.Entity<Company>()
                .HasMany(c => c.Teams)
                .WithOne(t => t.Company)
                .HasForeignKey(t => t.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            //FK Tickets Collection
            modelBuilder.Entity<Company>()
                .HasMany(c => c.Tickets)
                .WithOne(t => t.Company)
                .HasForeignKey(t => t.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

        }

    }
}