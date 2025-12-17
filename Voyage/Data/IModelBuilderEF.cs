using Microsoft.EntityFrameworkCore;

namespace Voyage.Data
{
    public interface IModelBuilderEF
    {
        public void CreateEntities(ModelBuilder modelBuilder);
    }
}
