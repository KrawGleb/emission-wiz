using EmissionWiz.Models.Interfaces.Providers;

namespace EmissionWiz.Logic.Providers;

internal class CurrentTimeProvider : ICurrentTimeProvider
{
    public DateTime NowUtc => DateTime.UtcNow;
}

internal class DateTimeProvider : IDateTimeProvider
{
    private readonly ICurrentTimeProvider _currentTimeProvider;

    public DateTimeProvider(ICurrentTimeProvider currentTimeProvider)
    {
        _currentTimeProvider = currentTimeProvider;
    }

    public DateTime GetUtcNow()
    {
        return _currentTimeProvider.NowUtc;
    }

    public DateTime GetUtcToday()
    {
        return GetUtcNow().Date;
    }

    public DateTime NowUtc => GetUtcNow();

    public DateTime TodayUtc => GetUtcToday();

    public DateTime StartOfDay(DateTime date)
    {
        return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
    }

    public DateTime EndOfDay(DateTime date)
    {
        return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
    }

    public DateTime FirstDayOfWeek(DateTime date)
    {
        var firstDay = DayOfWeek.Monday;
        var firstDayOfWeek = date.Date;

        while (firstDayOfWeek.DayOfWeek != firstDay)
        {
            firstDayOfWeek = firstDayOfWeek.AddDays(-1);
        }

        return firstDayOfWeek;
    }

    public DateTime FirstDayOfMonth(DateTime date)
    {
        return new DateTime(date.Year, date.Month, 1);
    }

    public DateTime LastDayOfWeek(DateTime date)
    {
        return FirstDayOfWeek(date).AddDays(6);
    }

    public DateTime LastDayOfMonth(DateTime date)
    {
        return new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);
    }
}
