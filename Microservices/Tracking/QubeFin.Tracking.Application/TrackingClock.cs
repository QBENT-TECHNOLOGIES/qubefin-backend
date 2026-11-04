namespace QubeFin.Tracking.Application;

// "Today" for every tracking rule is the date in India (Asia/Kolkata)
internal static class TrackingClock
{
    private static readonly TimeZoneInfo IndiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Kolkata");

    public static DateOnly Today() => DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndiaTimeZone));

    // Times are stored with DateTime.Now (India time); the API sends them with the +05:30 offset
    public static DateTimeOffset? WithIndiaOffset(DateTime? value) =>
        value.HasValue ? new DateTimeOffset(DateTime.SpecifyKind(value.Value, DateTimeKind.Unspecified), IndiaTimeZone.GetUtcOffset(value.Value)) : null;
}
