using Microsoft.EntityFrameworkCore;

namespace Voyage.Data.TableModels
{
    public class _MasterEF : IModelBuilderEF
    {
        public int MasterId { get; set; }
        public int OrganizationId { get; set; }

        public void SetOnModelCreatingEntities(ModelBuilder modelBuilder)
        {
            var type = typeof(IModelBuilderEF);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                                                .SelectMany(a => a.GetTypes())
                                                .Where(t => type.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            var instances = types.Select(t => Activator.CreateInstance(t) as IModelBuilderEF)
                                    .Where(i => i != null)
                                    .ToList();

            foreach (var instance in instances!)
            {
                instance!.CreateEntities(modelBuilder);
            }
        }

        public void CreateEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<_MasterEF>()
                 .ToTable("_Master");

            modelBuilder.Entity<_MasterEF>()
                .HasKey(m => m.MasterId);
        }

    }
}
