using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Tracking.Application.Enums;
using QubeFin.Tracking.Persistence.Repositories;

namespace QubeFin.Tracking.Application.Jobs.Queries;

#region --- QUERY ---
public record GetJobHistoryQuery(Guid UserId, Guid JobId) : IRequest<Result<List<JobHistoryResponse>>>;
#endregion

#region --- VALIDATOR ---
public class GetJobHistoryQueryValidator : AbstractValidator<GetJobHistoryQuery>
{
    public GetJobHistoryQueryValidator()
    {
        RuleFor(v => v.UserId).NotEmpty().WithMessage("User Id is required.");
        RuleFor(v => v.JobId).NotEmpty().WithMessage("Job Id is required.");
    }
}
#endregion

#region --- RESPONSE ---
public record JobHistoryResponse(
    Guid Id,
    Guid JobId,
    JobHistoryAction? Action,
    string PerformedByName,
    DateTimeOffset PerformedAt,
    string? FromAssigneeName,
    string? ToAssigneeName,
    IReadOnlyList<string> TaskNos,
    Guid? RelatedJobId,
    string? RelatedJobCode,
    DateOnly? OldCompleteBy,
    DateOnly? NewCompleteBy,
    string? Remarks);
#endregion

#region --- HANDLER ---
internal sealed class GetJobHistoryQueryHandler(ITrackingScopeRepository scopeRepository, IJobRepository jobRepository) : IRequestHandler<GetJobHistoryQuery, Result<List<JobHistoryResponse>>>
{
    public async Task<Result<List<JobHistoryResponse>>> Handle(GetJobHistoryQuery request, CancellationToken cancellationToken)
    {
        var viewer = await scopeRepository.GetViewerAsync(request.UserId, cancellationToken);
        if (viewer is null) return new ForbiddenError("You do not have access to Tracking.");

        var history = await jobRepository.GetJobHistoryAsync(viewer, request.JobId, cancellationToken);
        if (history is null) return new RecordNotFoundError("Job not found");

        return Result.Ok(history.Select(h => new JobHistoryResponse(
            h.Id,
            h.JobId,
            Enum.TryParse<JobHistoryAction>(h.Action, true, out var action) ? action : null,
            h.PerformedByName,
            h.PerformedAt,
            h.FromAssigneeName,
            h.ToAssigneeName,
            h.TaskNos,
            h.RelatedJobId,
            h.RelatedJobCode,
            h.OldCompleteBy,
            h.NewCompleteBy,
            h.Remarks)).ToList());
    }
}
#endregion
