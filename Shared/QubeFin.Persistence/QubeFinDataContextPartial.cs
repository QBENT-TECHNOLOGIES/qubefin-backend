namespace QubeFin.Persistence;

public partial class QubeFinDataContext : IUnitOfWork
{
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }
}