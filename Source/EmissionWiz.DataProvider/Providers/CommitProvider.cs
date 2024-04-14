using EmissionWiz.Models.Database;
using EmissionWiz.Models.Interfaces.Providers;

namespace EmissionWiz.DataProvider.Providers;

public class CommitProvider : ICommitProvider
{
    private readonly IDatabaseContext _dbContext;

    public CommitProvider(IDatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.CommitChangesAsync(cancellationToken);
    }
}
