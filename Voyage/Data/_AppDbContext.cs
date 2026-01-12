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
        public DbSet<Company> Companies { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketDetails> TicketDetails { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            _MasterEF masterEF = new _MasterEF();
            masterEF.SetOnModelCreatingEntities(modelBuilder);
        }


    }
}
