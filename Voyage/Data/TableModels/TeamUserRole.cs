using Microsoft.EntityFrameworkCore;
using Voyage.Data;
using Voyage.Data.TableModels;
using Voyage.Models;

public class TeamUserRole : BaseClass, IModelBuilderEF
{
    public Guid TeamKey { get; set; }
    public int CompanyId { get; set; }
    public int EmployeeId { get; set; }
    public int RoleId { get; set; }

    // Navigation properties
    public Team Team { get; set; } = null!;
    public AppUser User { get; set; } = null!;
    public Role Role { get; set; } = null!;

    public void CreateEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TeamUserRole>()
            .ToTable("TeamUserRoles")
            .HasKey(tur => new { tur.TeamKey, tur.CompanyId, tur.EmployeeId, tur.RoleId });

        modelBuilder.Entity<TeamUserRole>()
            .HasOne(tur => tur.Team)
            .WithMany(t => t.TeamUserRoles)
            .HasForeignKey(tur => tur.TeamKey)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TeamUserRole>()
            .HasOne(tur => tur.User)
            .WithMany(u => u.TeamUserRoles)
            .HasForeignKey(tur => new { tur.CompanyId, tur.EmployeeId })
            .HasPrincipalKey(u => new { u.CompanyId, u.EmployeeId })
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TeamUserRole>()
            .HasOne(tur => tur.Role)
            .WithMany(r => r.TeamUserRoles)
            .HasForeignKey(tur => tur.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}