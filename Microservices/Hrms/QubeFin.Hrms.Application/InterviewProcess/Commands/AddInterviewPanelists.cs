using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using Microsoft.AspNetCore.Http;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Hrms.Application.InterviewProcess.Services;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.InterviewProcess.Commands;

public record AddInterviewPanelistsCommand(Guid CandidateId, List<PanelistScheduleDto> Panelists, Guid AddedBy, IFormFile? AcknowledgementFile = null) : IRequest<Result<string>>, ICandidateWorkflowCommand;

public class AddInterviewPanelistsCommandValidator : AbstractValidator<AddInterviewPanelistsCommand>
{
    public AddInterviewPanelistsCommandValidator()
    {
        RuleFor(x => x.CandidateId).NotEmpty().WithMessage("Candidate is required.");
        RuleFor(x => x.Panelists).NotEmpty().WithMessage("At least one panelist must be selected.");

        RuleForEach(x => x.Panelists).ChildRules(panelist =>
        {
            panelist.RuleFor(p => p.EmployeeId).NotEmpty().WithMessage("Panelist is required.");
        });
    }
}

internal sealed class AddInterviewPanelistsCommandHandler(IInterviewPanelRepository panelRepository, ICandidateRepository candidateRepository, IUnitOfWork unitOfWork, IPanelInvitationMailer panelInvitationMailer) : IRequestHandler<AddInterviewPanelistsCommand, Result<string>>
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

        if (candidate.InterviewDate is not { } interviewDate || candidate.InterviewTime is not { } interviewTime)
        {
            return new ValidationError("Schedule the interview date and time before adding panelists.");
        }

        if (candidate.IsHrAssessmentCompleted)
        {
            return new ValidationError("Panelists cannot be added once the HR Assessment is completed.");
        }

        var existingPanelists = await panelRepository.GetByCandidateIdAsync(request.CandidateId, includeEmployee: false);
        var alreadyScheduledEmployeeIds = existingPanelists.Select(p => p.EmployeeId).ToHashSet();

        var alreadyOnPanel = request.Panelists.Any(p => alreadyScheduledEmployeeIds.Contains(p.EmployeeId));
        if (alreadyOnPanel)
        {
            return new ValidationError("One or more selected panelists are already on this candidate's interview panel.");
        }

        var newPanelists = request.Panelists
            .Select(p => InterviewPanel.Schedule(
                request.CandidateId,
                p.EmployeeId,
                interviewDate,
                interviewTime,
                request.AddedBy))
            .ToList();

        await panelRepository.AddRangeAsync(newPanelists, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        try
        {
            var missingEmail = await panelInvitationMailer.SendInvitationsAsync(candidate, request.Panelists, request.AcknowledgementFile, cancellationToken);
            if (missingEmail > 0)
            {
                return Result.Ok($"Panelist(s) added successfully. Invitation mail was not sent to {missingEmail} panelist(s) without an email address.");
            }
        }
        catch (Exception)
        {
            return Result.Ok("Panelist(s) added successfully. However, the invitation mail could not be sent to the panelists.");
        }

        return Result.Ok("Panelist(s) added successfully.");
    }
}
