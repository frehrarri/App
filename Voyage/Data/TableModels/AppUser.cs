using AngleSharp.Dom;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Voyage.Data.TableModels
{
    public class AppUser : IdentityUser
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

        public int CompanyId { get; set; }
        public Company? Company { get; set; } = null!;

        public ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();

        #endregion



        public void CreateEntities(ModelBuilder modelBuilder)
        {
            // Alternate (business) key
            modelBuilder.Entity<AppUser>()
                .HasAlternateKey(u => new { u.CompanyId, u.EmployeeId });

            modelBuilder.Entity<AppUser>()
                .HasIndex(u => new { u.CompanyId, u.EmployeeId })
                .IsUnique();

        }
    }
}
