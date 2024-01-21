namespace EmissionWiz.Models.Interfaces.Providers;

public interface ICurrentTimeProvider
{
    DateTime NowUtc { get; }
}
