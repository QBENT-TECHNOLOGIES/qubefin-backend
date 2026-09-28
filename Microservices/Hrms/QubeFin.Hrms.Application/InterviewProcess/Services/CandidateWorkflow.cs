using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.InterviewProcess.Services;

/// <summary>A command that acts on a candidate's interview workflow. Once the workflow is stopped - HR
/// rejected the candidate (RecommendationStatus 'Rejected'), or submitted the HR Assessment as 'Not Recommended' - <see cref="CandidateWorkflowGuardBehavior{TRequest, TResponse}"/>
/// refuses every such command before its handler runs.</summary>
public interface ICandidateWorkflowCommand
{
    Guid CandidateId { get; }
}

public static class CandidateWorkflow
{
    /// <summary>RecommendationStatus of a candidate HR rejected (<see cref="Candidate.Reject"/>). Stops the
    /// workflow as soon as it is set.</summary>
    public const string Rejected = Candidate.RejectedRecommendationStatus;

    /// <summary>The HR Assessment's final recommendation that stops the workflow. Only counts once HR has
    /// submitted: a draft can hold it too, and a draft is still HR's to change.</summary>
    public const string NotRecommended = "Not Recommended";

    /// <summary>Why the candidate's workflow is stopped - <see cref="CandidateInterviewStatus.Rejected"/> or
    /// <see cref="CandidateInterviewStatus.NotRecommended"/> - or null while it is still running (or the
    /// candidate does not exist). Rejected wins when both apply.</summary>
    public static async Task<string?> GetStoppedStatusAsync(this QubeFinDataContext context, Guid candidateId, CancellationToken cancellationToken)
    {
        var state = await context.TblInterviewCandidates
            .AsNoTracking()
            .Where(c => c.Id == candidateId)
            .Select(c => new
            {
                IsRejected = c.RecommendationStatus == Rejected,
                IsNotRecommended = c.RecommendationStatus == NotRecommended &&
                    c.TblInterviewPanels.Any(p => p.AssessmentType == InterviewPanel.HrAssessmentType && p.IsSubmitted)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (state is null)
        {
            return null;
        }

        return state.IsRejected
            ? CandidateInterviewStatus.Rejected
            : state.IsNotRecommended
                ? CandidateInterviewStatus.NotRecommended
                : null;
    }

    /// <summary>The employee currently holds the HR post (the "HrPost" setting) - the same test the
    /// candidate list uses to show HR every candidate.</summary>
    public static Task<bool> IsHrEmployeeAsync(this QubeFinDataContext context, Guid hrPostId, Guid employeeId, CancellationToken cancellationToken) =>
        context.TblDesignations.AnyAsync(d =>
            d.PostId == hrPostId &&
            d.TblEmployeeDesignations.Any(ed => ed.EmployeeId == employeeId && ed.EffectiveTo == null),
            cancellationToken);
}

/// <summary>Stops any <see cref="ICandidateWorkflowCommand"/> on a candidate whose workflow is stopped, so the
/// API enforces what the UI hides. Answers with the handler's own failed Result (a 400 through
/// ToHttpResult) rather than throwing, like the handlers' own ValidationErrors.</summary>
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
        ((IResultBase)response).Reasons.Add(new ValidationError(
            stoppedStatus == CandidateInterviewStatus.Rejected
                ? "This candidate has been rejected. No further action can be taken."
                : "This candidate was not recommended by HR. No further action can be taken."));

        return response;
    }
}
