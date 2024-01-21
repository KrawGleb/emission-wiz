namespace EmissionWiz.Models.Interfaces.Providers;

public interface ICommitProvider
{
    Task CommitAsync(CancellationToken cancellationToken = default);
}
