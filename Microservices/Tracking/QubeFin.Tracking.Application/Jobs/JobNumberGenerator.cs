using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Models.Tracking;
using QubeFin.Tracking.Persistence.Repositories;

namespace QubeFin.Tracking.Application.Jobs;

// JOB-YY0001 and TSK-YYBBB000001: YY = last two digits of the financial year's start, BBB = branch code, the rest a running number
internal sealed class JobNumberGenerator(ITrackingScopeRepository scopeRepository, IJobRepository jobRepository)
{
    public async Task<string> NextJobCodeAsync(DateOnly today, CancellationToken cancellationToken)
    {
        var prefix = $"JOB-{await YearAsync(today, cancellationToken)}";
        var last = await jobRepository.GetLastJobCodeAsync(prefix, cancellationToken);
        return $"{prefix}{NextNumber(last, prefix):D4}";
    }

    public async Task AssignTaskNosAsync(IEnumerable<TblJobTask> tasks, IReadOnlyDictionary<Guid, BranchCode> branches, DateOnly today, CancellationToken cancellationToken)
    {
        var year = await YearAsync(today, cancellationToken);
        var next = new Dictionary<string, int>();
        foreach (var task in tasks)
        {
            var prefix = $"TSK-{year}{branches[task.BranchId].CodeVal:D3}";
            if (!next.TryGetValue(prefix, out var number))
            {
                number = NextNumber(await jobRepository.GetLastTaskNoAsync(prefix, cancellationToken), prefix);
            }
            task.TaskNo = $"{prefix}{number:D6}";
            next[prefix] = number + 1;
        }
    }

    private async Task<string> YearAsync(DateOnly today, CancellationToken cancellationToken)
    {
        // Financial year from Tbl_FinancialYear; April to March when no row covers today
        var start = await scopeRepository.GetFinancialYearStartAsync(today, cancellationToken)
            ?? new DateOnly(today.Month >= 4 ? today.Year : today.Year - 1, 4, 1);
        return (start.Year % 100).ToString("D2");
    }

    private static int NextNumber(string? last, string prefix) =>
        last is not null && int.TryParse(last[prefix.Length..], out var number) ? number + 1 : 1;
}
