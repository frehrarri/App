using Microsoft.EntityFrameworkCore;
using Voyage.Models;

namespace Voyage.Data.TableModels
{
    public class Permissions : BaseClass, IModelBuilderEF
    {
        public int PermissionsId { get; set; }
        public string Name {  get; set; } = string.Empty;


        public void CreateEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Permissions>()
                 .ToTable("Permissions");

            modelBuilder.Entity<Permissions>()
                .HasKey(t => t.PermissionsId);

            modelBuilder.Entity<Permissions>()
                .Property(t => t.PermissionsId)
                .ValueGeneratedOnAdd();
        }
    }
}
