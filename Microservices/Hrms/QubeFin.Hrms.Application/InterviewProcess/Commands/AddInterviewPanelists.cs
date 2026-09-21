using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Hrms.Application.InterviewProcess.Services;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.InterviewProcess.Commands;

/// <summary>Adds one or more panelists to a candidate's existing interview panel.</summary>
public record AddInterviewPanelistsCommand(Guid CandidateId, List<PanelistScheduleDto> Panelists, Guid AddedBy) : IRequest<Result<string>>;

public class AddInterviewPanelistsCommandValidator : AbstractValidator<AddInterviewPanelistsCommand>
{
    public AddInterviewPanelistsCommandValidator()
    {
        RuleFor(x => x.CandidateId).NotEmpty().WithMessage("Candidate is required.");
        RuleFor(x => x.Panelists).NotEmpty().WithMessage("At least one panelist must be selected.");

        RuleForEach(x => x.Panelists).ChildRules(panelist =>
        {
            panelist.RuleFor(p => p.EmployeeId).NotEmpty().WithMessage("Panelist is required.");
            panelist.RuleFor(p => p.ScheduledDate).NotEmpty().WithMessage("Scheduled date is required.");
            panelist.RuleFor(p => p.ScheduledTime).NotEmpty().WithMessage("Scheduled time is required.");
        });
    }
}

internal sealed class AddInterviewPanelistsCommandHandler(IInterviewPanelRepository panelRepository, ICandidateRepository candidateRepository, IUnitOfWork unitOfWork) : IRequestHandler<AddInterviewPanelistsCommand, Result<string>>
{
    public async Task<Result<string>> Handle(AddInterviewPanelistsCommand request, CancellationToken cancellationToken)
    {
        if (request.AddedBy == Guid.Empty)
        {
            return new ValidationError("Authenticated user is required.");
        }

        var candidate = await candidateRepository.GetByIdAsync(request.CandidateId);
        if (candidate is null)
        {
            return new RecordNotFoundError("Candidate not found.");
        }

        var duplicateEmployeeIds = request.Panelists
            .GroupBy(p => p.EmployeeId)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateEmployeeIds.Count > 0)
        {
            return new ValidationError("The same panelist cannot be added to a candidate more than once.");
        }

        // Only INTERVIEWER rows count as "the panel". Without this filter, an HR employee holding the HR
        // Assessment row would be rejected as "already on the panel", and the IsAttened gate below would
        // trip on that row (the HR Assessment marks itself attended).
        var existingPanelists = (await panelRepository.GetByCandidateIdAsync(request.CandidateId)).Interviewers().ToList();
        var alreadyScheduledEmployeeIds = existingPanelists.Select(p => p.EmployeeId).ToHashSet();

        var alreadyOnPanel = request.Panelists.Any(p => alreadyScheduledEmployeeIds.Contains(p.EmployeeId));
        if (alreadyOnPanel)
        {
            return new ValidationError("One or more selected panelists are already on this candidate's interview panel.");
        }

        if (existingPanelists.Any(p => p.IsAttened))
        {
            return new ValidationError("Panelists cannot be added once the interview has started for this candidate.");
        }

        var newPanelists = request.Panelists
            .Select(p => InterviewPanel.Schedule(
                request.CandidateId,
                p.EmployeeId,
                p.ScheduledDate,
                p.ScheduledTime,
                request.AddedBy))
            .ToList();

        await panelRepository.AddRangeAsync(newPanelists, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok("Panelist(s) added successfully.");
    }
}
