using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using Voyage.Data.TableModels;

namespace Voyage.Data
{
    public class _AppDbContext : IdentityDbContext<AppUser>
    {
        public _AppDbContext(DbContextOptions<_AppDbContext> options): base(options) { }

        public DbSet<_MasterEF> _Master { get; set; }
        public DbSet<Settings> Settings { get; set; }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Permissions> Permissions { get; set; }
        
        public DbSet<Section> Sections { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketDetails> TicketDetails { get; set; }

        public DbSet<Role> CompanyRoles { get; set; }
        public DbSet<IndividualUserRole> IndividualUserRoles { get; set; }
        public DbSet<TeamUserRole> TeamUserRoles { get; set; }
        public DbSet<DepartmentUserRole> DepartmentUserRoles { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Remove the default RoleNameIndex - will use our custom constraint in AppRole
            //modelBuilder.Entity<AppRole>(b =>
            //{
            //    b.HasIndex(r => new { r.NormalizedName, r.CompanyId })
            //     .IsUnique()
            //     .HasDatabaseName("IX_RoleName_CompanyId");

            //    // remove the default index if it exists
            //    var defaultIndex = b.Metadata.GetIndexes()
            //        .FirstOrDefault(i => i.GetDatabaseName() == "RoleNameIndex");
            //    if (defaultIndex != null)
            //        b.Metadata.RemoveIndex(defaultIndex);
            //});

            _MasterEF masterEF = new _MasterEF();
            masterEF.SetOnModelCreatingEntities(modelBuilder);
        }


    }
}
