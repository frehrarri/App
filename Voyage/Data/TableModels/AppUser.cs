using AngleSharp.Dom;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Voyage.Data.TableModels
{
    public class AppUser : IdentityUser, IModelBuilderEF
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public int PhoneAreaCode { get; set; }
        public int PhoneCountryCode { get; set; }
        public string Country { get; set; } = string.Empty;
        public string StreetAddress { get; set; } = string.Empty;
        public int UnitNumber { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public int PostalCode { get; set; }

        #region FK
        //all users are associated with a company on registration
        public int CompanyId { get; set; }
        public Company Company { get; set; } = null!;

        // all users can be assigned different roles at the individual, department, or team levels
        public ICollection<IndividualUserRole> IndividualUserRoles { get; set; } = new List<IndividualUserRole>();
        public ICollection<DepartmentUserRole> DepartmentUserRoles { get; set; } = new List<DepartmentUserRole>();
        public ICollection<TeamUserRole> TeamUserRoles { get; set; } = new List<TeamUserRole>();

        public Guid? SettingsKey { get; set; }
        public Settings? Settings { get ; set; }

        #endregion


        public void CreateEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUser>()
                .HasAlternateKey(u => new { u.CompanyId, u.EmployeeId });

            //Settings FK
            modelBuilder.Entity<AppUser>()
                .HasOne(t => t.Settings)
                .WithOne(s => s.User)
                .HasForeignKey<Settings>(s => s.SettingsKey)
                .IsRequired(false);
        }
    }
}
