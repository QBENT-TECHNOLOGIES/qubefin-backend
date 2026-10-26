using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.InterviewProcess.Services;

public interface ICandidateWorkflowCommand
{
    Guid CandidateId { get; }
}

public static class CandidateWorkflow
{
    public const string Rejected = Candidate.RejectedRecommendationStatus;
    public const string Pending = "Pending";
    public static readonly string[] QualifiedStatuses = ["Strongly Recommended", "Recommended", "Recommended with Training"];
    public static bool IsQualified(string? recommendationStatus) => recommendationStatus is not null && QualifiedStatuses.Contains(recommendationStatus);
    public static DateOnly Today => DateOnly.FromDateTime(DateTime.Now);
    public static async Task<string?> GetStoppedStatusAsync(this QubeFinDataContext context, Guid candidateId, CancellationToken cancellationToken)
    {
        var state = await context.TblInterviewCandidates.AsNoTracking().Where(c => c.Id == candidateId).Select(c => new { c.RecommendationStatus, c.IsHrAssessmentCompleted }).FirstOrDefaultAsync(cancellationToken);

        if (state is null)
        {
            return null;
        }

        if (state.RecommendationStatus == Rejected)
        {
            return CandidateInterviewStatus.Rejected;
        }

        return state.IsHrAssessmentCompleted && !IsQualified(state.RecommendationStatus) ? CandidateInterviewStatus.NotSelected : null;
    }
    public static Task<bool> IsHrEmployeeAsync(this QubeFinDataContext context, Guid hrPostId, Guid employeeId, CancellationToken cancellationToken) =>
        context.TblDesignations.AnyAsync(d => d.PostId == hrPostId && d.TblEmployeeDesignations.Any(ed => ed.EmployeeId == employeeId && ed.EffectiveTo == null), cancellationToken);

    public static Task<bool> IsCandidateCreatorAsync(this QubeFinDataContext context, Guid candidateId, Guid employeeId, CancellationToken cancellationToken) =>
        context.TblInterviewCandidates.AnyAsync(c => c.Id == candidateId && c.CreatedByNavigation.EmployeeId == employeeId, cancellationToken);
}

public class CandidateWorkflowGuardBehavior<TRequest, TResponse>(QubeFinDataContext context) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not ICandidateWorkflowCommand command || !typeof(IResultBase).IsAssignableFrom(typeof(TResponse)))
        {
            return await next();
        }

        var stoppedStatus = await context.GetStoppedStatusAsync(command.CandidateId, cancellationToken);
        if (stoppedStatus is null)
        {
            return await next();
        }

        var response = (TResponse)Activator.CreateInstance(typeof(TResponse))!;
        ((IResultBase)response).Reasons.Add(new ValidationError(stoppedStatus == CandidateInterviewStatus.Rejected ? "This candidate has been rejected. No further action can be taken." : "This candidate was not selected. No further action can be taken."));

        return response;
    }
}
