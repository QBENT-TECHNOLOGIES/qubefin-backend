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

public record ScheduleInterviewPanelCommand(Guid CandidateId, List<PanelistScheduleDto> Panelists, Guid ScheduledBy, IFormFile? AcknowledgementFile = null) : IRequest<Result<string>>;

public class ScheduleInterviewPanelCommandValidator : AbstractValidator<ScheduleInterviewPanelCommand>
{
    public ScheduleInterviewPanelCommandValidator()
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

internal sealed class ScheduleInterviewPanelCommandHandler(IInterviewPanelRepository panelRepository, ICandidateRepository candidateRepository, IUnitOfWork unitOfWork, IPanelInvitationMailer panelInvitationMailer) : IRequestHandler<ScheduleInterviewPanelCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ScheduleInterviewPanelCommand request, CancellationToken cancellationToken)
    {
        if (request.ScheduledBy == Guid.Empty)
        {
            return Result.Fail("Authenticated user is required.");
        }

        var candidate = await candidateRepository.GetByIdAsync(request.CandidateId);
        if (candidate is null)
        {
            return Result.Fail("Candidate not found.");
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

        // Only INTERVIEWER rows count as "the panel" - the HR Assessment row lives in the same table.
        var existingPanelists = (await panelRepository.GetByCandidateIdAsync(request.CandidateId)).Interviewers().ToList();
        if (existingPanelists.Count > 0)
        {
            return new ValidationError("This candidate already has an interview panel. Use \"Add Panelists\" to add more.");
        }

        var panelists = request.Panelists
            .Select(p => InterviewPanel.Schedule(
                request.CandidateId,
                p.EmployeeId,
                p.ScheduledDate,
                p.ScheduledTime,
                request.ScheduledBy))
            .ToList();

        await panelRepository.AddRangeAsync(panelists, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // The panel is already saved - a mail failure is reported in the message rather than failing the request.
        try
        {
            var missingEmail = await panelInvitationMailer.SendInvitationsAsync(candidate, request.Panelists, request.AcknowledgementFile, cancellationToken);
            if (missingEmail > 0)
            {
                return Result.Ok($"Interview panel scheduled successfully. Invitation mail was not sent to {missingEmail} panelist(s) without an email address.");
            }
        }
        catch (Exception)
        {
            return Result.Ok("Interview panel scheduled successfully. However, the invitation mail could not be sent to the panelists.");
        }

        return Result.Ok("Interview panel scheduled successfully.");
    }
}