using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.InterviewProcess.Services;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.InterviewProcess.Commands;

/// <summary>
/// Panel Tagging: sets the panel of every given candidate to exactly <see cref="EmployeeIds"/>. Used to tag
/// untagged candidates, move candidates to another panel and change the members of a panel. Panelists that
/// stay keep their acknowledgement; new panelists are invited; dropped panelists are removed.
/// </summary>
public record AssignInterviewPanelCommand(
    List<Guid> CandidateIds,
    List<Guid> EmployeeIds,
    Guid EmployeeId,
    Guid AssignedBy,
    IReadOnlyList<IFormFile> AcknowledgementFiles) : IRequest<Result<string>>;

public class AssignInterviewPanelCommandValidator : AbstractValidator<AssignInterviewPanelCommand>
{
    public AssignInterviewPanelCommandValidator()
    {
        RuleFor(x => x.CandidateIds).NotEmpty().WithMessage("Select at least one candidate.");
        RuleFor(x => x.EmployeeIds).NotEmpty().WithMessage("At least one panelist must be selected.");
        RuleFor(x => x.CandidateIds).Must(ids => ids.Distinct().Count() == ids.Count).WithMessage("The same candidate is selected more than once.");
        RuleFor(x => x.EmployeeIds).Must(ids => ids.Distinct().Count() == ids.Count).WithMessage("The same panelist cannot be added more than once.");
        RuleForEach(x => x.EmployeeIds).NotEmpty().WithMessage("Panelist is required.");
    }
}

internal sealed class AssignInterviewPanelCommandHandler(
    ICandidateRepository candidateRepository,
    IInterviewPanelRepository panelRepository,
    QubeFinDataContext context,
    IConfiguration configuration,
    IUnitOfWork unitOfWork,
    IPanelInvitationMailer panelInvitationMailer) : IRequestHandler<AssignInterviewPanelCommand, Result<string>>
{
    public async Task<Result<string>> Handle(AssignInterviewPanelCommand request, CancellationToken cancellationToken)
    {
        if (request.AssignedBy == Guid.Empty || request.EmployeeId == Guid.Empty)
        {
            return new ValidationError("Authenticated user is required.");
        }

        var hrPostId = Guid.Parse(configuration["HrPost"]!);
        if (!await context.IsHrEmployeeAsync(hrPostId, request.EmployeeId, cancellationToken))
        {
            return new ForbiddenError("Only HR can tag candidates to an interview panel.");
        }

        var knownEmployees = await context.TblEmployees.CountAsync(e => request.EmployeeIds.Contains(e.Id), cancellationToken);
        if (knownEmployees != request.EmployeeIds.Count)
        {
            return new ValidationError("One or more selected panelists were not found.");
        }

        var candidates = new List<Candidate>(request.CandidateIds.Count);
        foreach (var candidateId in request.CandidateIds)
        {
            var candidate = await candidateRepository.GetByIdAsync(candidateId);
            if (candidate is null)
            {
                return new RecordNotFoundError("Candidate not found.");
            }

            var error = PanelTagging.ValidateCandidate(candidate);
            if (error is not null)
            {
                return new ValidationError(error);
            }

            candidates.Add(candidate);
        }

        // A panel is for one interview date at one venue.
        if (candidates.Select(c => (c.InterviewDate, c.VenueOrganizationUnitId)).Distinct().Count() > 1)
        {
            return new ValidationError("All candidates on a panel must have the same interview date and venue.");
        }

        var newEmployeeIds = request.EmployeeIds.ToHashSet();
        var invitations = new Dictionary<Guid, List<Candidate>>();
        var changed = 0;

        foreach (var candidate in candidates)
        {
            var existing = await panelRepository.GetByCandidateIdAsync(candidate.Id, includeEmployee: false);
            var existingEmployeeIds = existing.Select(p => p.EmployeeId).ToHashSet();

            if (existingEmployeeIds.SetEquals(newEmployeeIds))
            {
                continue;
            }

            var lockedReason = PanelTagging.LockedReason(existing);
            if (lockedReason is not null)
            {
                return new ValidationError($"{candidate.FirstName} {candidate.LastName}: {lockedReason} The panel can no longer be changed.");
            }

            foreach (var removed in existing.Where(p => !newEmployeeIds.Contains(p.EmployeeId)))
            {
                await panelRepository.DeleteAsync(removed.Id);
            }

            var added = request.EmployeeIds.Where(id => !existingEmployeeIds.Contains(id)).ToList();
            await panelRepository.AddRangeAsync(
                added.Select(employeeId => InterviewPanel.Schedule(candidate.Id, employeeId, candidate.InterviewDate!.Value, candidate.InterviewTime!.Value, request.AssignedBy)),
                cancellationToken);

            foreach (var employeeId in added)
            {
                if (!invitations.TryGetValue(employeeId, out var list))
                {
                    invitations[employeeId] = list = [];
                }

                list.Add(candidate);
            }

            changed++;
        }

        if (changed == 0)
        {
            return Result.Ok("No change - the selected candidate(s) already have this panel.");
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var message = $"Interview panel updated for {changed} candidate(s).";
        if (invitations.Count == 0)
        {
            return Result.Ok(message);
        }

        // The panel is already saved - a mail failure is reported in the message rather than failing the request.
        try
        {
            var missingEmail = await panelInvitationMailer.SendPanelAssignmentsAsync(invitations, request.AcknowledgementFiles, cancellationToken);
            if (missingEmail > 0)
            {
                return Result.Ok($"{message} Invitation mail was not sent to {missingEmail} panelist(s) without an email address.");
            }
        }
        catch (Exception)
        {
            return Result.Ok($"{message} However, the invitation mail could not be sent to the panelists.");
        }

        return Result.Ok(message);
    }
}

internal static class PanelTagging
{
    /// <summary>Why the candidate cannot be put on a panel at all, or null.</summary>
    public static string? ValidateCandidate(Candidate candidate)
    {
        var name = $"{candidate.FirstName} {candidate.LastName}";

        if (candidate.RecommendationStatus == CandidateWorkflow.Rejected)
        {
            return $"{name} has been rejected.";
        }

        if (candidate.IsHrAssessmentCompleted)
        {
            return $"{name}: the HR Assessment is already completed.";
        }

        if (candidate.InterviewDate is null || candidate.InterviewTime is null)
        {
            return $"{name}: schedule the interview date and time first.";
        }

        if (candidate.VenueOrganizationUnitId is null)
        {
            return $"{name}: set the venue organization unit first.";
        }

        return candidate.InterviewDate < CandidateWorkflow.Today ? $"{name}: the interview date has passed." : null;
    }

    /// <summary>Once the interview has started on any panelist's side the candidate's panel is frozen.</summary>
    public static string? LockedReason(IEnumerable<InterviewPanel> panelists)
    {
        var list = panelists.ToList();

        if (list.Any(p => p.IsSubmitted))
        {
            return "A panelist has already submitted the assessment.";
        }

        return list.Any(p => p.IsAttened || p.AttenedRemarks != null) ? "The interview has already started." : null;
    }
}
