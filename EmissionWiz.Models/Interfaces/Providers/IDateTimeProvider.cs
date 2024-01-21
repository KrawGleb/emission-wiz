namespace EmissionWiz.Models.Interfaces.Providers;

public interface IDateTimeProvider
{
    DateTime NowUtc { get; }
    DateTime TodayUtc { get; }
    DateTime StartOfDay(DateTime date);
    DateTime EndOfDay(DateTime date);
    DateTime FirstDayOfWeek(DateTime date);
    DateTime FirstDayOfMonth(DateTime date);
    DateTime LastDayOfWeek(DateTime date);
    DateTime LastDayOfMonth(DateTime date);
}
