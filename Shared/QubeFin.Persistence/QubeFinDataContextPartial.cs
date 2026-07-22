using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence.Models.App;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Persistence;

public partial class QubeFinDataContext : DbContext, IUnitOfWork
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MenuHierarchyItem>()
            .HasNoKey()
            .ToView(null);

        modelBuilder.Entity<AdministrativeHierarchyItem>()
            .HasNoKey()
            .ToView(null);

        modelBuilder.Entity<OrganizationUnitHierarchyItem>()
            .HasNoKey()
            .ToView(null);

        modelBuilder.Entity<SurveyResults>()
            .HasNoKey()
            .ToView(null);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }
}