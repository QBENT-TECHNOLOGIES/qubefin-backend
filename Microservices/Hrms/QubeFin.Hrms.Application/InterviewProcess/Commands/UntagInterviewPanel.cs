using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.InterviewProcess.Services;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.InterviewProcess.Commands;

/// <summary>Panel Tagging: removes every panelist from the given candidates, leaving them untagged.</summary>
public record UntagInterviewPanelCommand(List<Guid> CandidateIds, Guid EmployeeId, Guid RemovedBy) : IRequest<Result<string>>;

public class UntagInterviewPanelCommandValidator : AbstractValidator<UntagInterviewPanelCommand>
{
    public UntagInterviewPanelCommandValidator()
    {
        RuleFor(x => x.CandidateIds).NotEmpty().WithMessage("Select at least one candidate.");
    }
}

internal sealed class UntagInterviewPanelCommandHandler(
    ICandidateRepository candidateRepository,
    IInterviewPanelRepository panelRepository,
    QubeFinDataContext context,
    IConfiguration configuration,
    IUnitOfWork unitOfWork) : IRequestHandler<UntagInterviewPanelCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UntagInterviewPanelCommand request, CancellationToken cancellationToken)
    {
        if (request.RemovedBy == Guid.Empty || request.EmployeeId == Guid.Empty)
        {
            return new ValidationError("Authenticated user is required.");
        }

        var hrPostId = Guid.Parse(configuration["HrPost"]!);
        if (!await context.IsHrEmployeeAsync(hrPostId, request.EmployeeId, cancellationToken))
        {
            return new ForbiddenError("Only HR can untag candidates from an interview panel.");
        }

        var removed = 0;
        foreach (var candidateId in request.CandidateIds.Distinct())
        {
            var candidate = await candidateRepository.GetByIdAsync(candidateId);
            if (candidate is null)
            {
                return new RecordNotFoundError("Candidate not found.");
            }

            var panelists = await panelRepository.GetByCandidateIdAsync(candidateId, includeEmployee: false);
            if (panelists.Count == 0)
            {
                continue;
            }

            var lockedReason = candidate.IsHrAssessmentCompleted ? "The HR Assessment is already completed." : PanelTagging.LockedReason(panelists);
            if (lockedReason is not null)
            {
                return new ValidationError($"{candidate.FirstName} {candidate.LastName}: {lockedReason} The candidate cannot be untagged.");
            }

            foreach (var panelist in panelists)
            {
                await panelRepository.DeleteAsync(panelist.Id);
            }

            removed++;
        }

        if (removed == 0)
        {
            return Result.Ok("The selected candidate(s) are not tagged to any panel.");
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok($"{removed} candidate(s) untagged from the interview panel.");
    }
}
