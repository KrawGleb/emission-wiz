using EmissionWiz.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace EmissionWiz.DataProvider.Database;

internal sealed class DatabaseContext : EmissionWizDbContext, IDatabaseContext
{
    public DatabaseContext(DbContextOptions<EmissionWizDbContext> options) : base(options)
    { }

    public async Task<int> CommitChangesAsync(CancellationToken cancellationToken = default)
    {
        return await SaveChangesAsync(cancellationToken);
    }
}
