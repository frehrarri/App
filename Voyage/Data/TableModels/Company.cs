using Microsoft.EntityFrameworkCore;

namespace Voyage.Data.TableModels
{
    public class Company
    {
        public int CompanyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string StreetAddress { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        //WithMany(c => c.Users)
        public ICollection<AppUser> Users { get; set; } = new List<AppUser>();

        public void CreateEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ticket>()
                 .ToTable("Company");

            modelBuilder.Entity<Company>()
                .HasKey(t => t.CompanyId);

            modelBuilder.Entity<Company>()
                .Property(t => t.CompanyId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<AppUser>()
                .HasOne(u => u.Company)
                .WithMany(c => c.Users)
                .HasForeignKey(u => u.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}