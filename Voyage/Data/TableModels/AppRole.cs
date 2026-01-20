using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Voyage.Data;
using Voyage.Data.TableModels;

public class AppRole : IdentityRole, IModelBuilderEF
{
    public int CompanyId { get; set; }

    public void CreateEntities(ModelBuilder modelBuilder)
    {
        //EF naturally has NormalizedName as a unique key.
        //need to make it composite to allow all Company's to be able to share role names
        modelBuilder.Entity<AppRole>()
            .HasIndex(r => new { r.NormalizedName, r.CompanyId })
            .IsUnique()
            .HasDatabaseName("IX_RoleName_CompanyId");
    }

}
