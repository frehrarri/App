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
        public string Email {  get; set; } = string.Empty;




        #region Foreign Keys

        public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

        #endregion

        public void CreateEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>()
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