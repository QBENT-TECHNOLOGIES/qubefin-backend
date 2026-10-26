using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.InterviewProcess.Services;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.InterviewProcess.Commands;

public record MarkCandidateAttendanceCommand(Guid CandidateId, bool IsPresent, string? Remarks, Guid EmployeeId, Guid ModifiedBy) : IRequest<Result<string>>, ICandidateWorkflowCommand;

public record MarkCandidateAttendanceRequest(Guid CandidateId, bool IsPresent, string? Remarks);

public class MarkCandidateAttendanceCommandValidator : AbstractValidator<MarkCandidateAttendanceCommand>
{
    public MarkCandidateAttendanceCommandValidator()
    {
        RuleFor(x => x.CandidateId).NotEmpty().WithMessage("Candidate is required.");
        RuleFor(x => x.Remarks)
            .NotEmpty().When(x => !x.IsPresent).WithMessage("Enter the reason the candidate was absent.")
            .MaximumLength(100).WithMessage("Remarks cannot exceed 100 characters.");
    }
}

internal sealed class MarkCandidateAttendanceCommandHandler(
    IInterviewPanelRepository panelRepository,
    QubeFinDataContext context,
    IUnitOfWork unitOfWork) : IRequestHandler<MarkCandidateAttendanceCommand, Result<string>>
{
    public async Task<Result<string>> Handle(MarkCandidateAttendanceCommand request, CancellationToken cancellationToken)
    {
        if (request.ModifiedBy == Guid.Empty || request.EmployeeId == Guid.Empty)
        {
            return new ValidationError("Authenticated user is required.");
        }

        var panel = await panelRepository.GetByCandidateAndEmployeeAsync(request.CandidateId, request.EmployeeId, includeEmployee: false);
        if (panel is null)
        {
            return new RecordNotFoundError("You are not on this candidate's interview panel.");
        }

        if (panel.IsSubmitted || panel.IsAttendanceMarked)
        {
            return new ValidationError("The candidate's attendance has already been recorded.");
        }

        var blocker = await context.GetAssessmentBlockerAsync(panel, cancellationToken);
        if (blocker is not null)
        {
            return new ValidationError(blocker);
        }

        panel.MarkCandidateAttendance(request.IsPresent, request.Remarks, request.ModifiedBy);

        await panelRepository.UpdateAsync(panel);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(request.IsPresent ? "Candidate marked present." : "Candidate marked absent.");
    }
}
