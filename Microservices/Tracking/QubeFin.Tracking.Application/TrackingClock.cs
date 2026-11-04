namespace QubeFin.Tracking.Application;

// "Today" for every tracking rule is the date in India (Asia/Kolkata)
internal static class TrackingClock
{
    private static readonly TimeZoneInfo IndiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Kolkata");

    public static DateOnly Today() => DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndiaTimeZone));
}
