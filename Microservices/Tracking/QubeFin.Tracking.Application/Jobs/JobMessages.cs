using QubeFin.Tracking.Application.Jobs.Models;

namespace QubeFin.Tracking.Application.Jobs;

// Write APIs answer with a message (like HRMS); tasks that were skipped or warned about are listed in it
internal static class JobMessages
{
    public static string With(string message, IReadOnlyCollection<SkippedTask> skipped, IReadOnlyCollection<TaskWarning> warnings, string skippedLabel = "Not added")
    {
        var parts = new List<string> { message };
        if (skipped.Count > 0) parts.Add($"{skippedLabel}: {string.Join("; ", skipped.Select(s => $"{s.Title} — {s.Reason}"))}.");
        if (warnings.Count > 0) parts.Add($"Note: {string.Join("; ", warnings.Select(w => $"{w.Title} — {w.Reason}"))}.");
        return string.Join(" ", parts);
    }

    public static string Tasks(int count) => count == 1 ? "1 task" : $"{count} tasks";
}
