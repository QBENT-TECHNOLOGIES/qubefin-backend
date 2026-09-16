using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.InterviewProcess.Commands;

public record AcknowledgePanelInvitationCommand(Guid PanelId, Guid AcknowledgedBy) : IRequest<Result>;

public class AcknowledgePanelInvitationCommandValidator : AbstractValidator<AcknowledgePanelInvitationCommand>
{
    public AcknowledgePanelInvitationCommandValidator()
    {
        RuleFor(x => x.PanelId).NotEmpty().WithMessage("Interview panel entry is required.");
    }
}

internal sealed class AcknowledgePanelInvitationCommandHandler(
    IInterviewPanelRepository panelRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<AcknowledgePanelInvitationCommand, Result>
{
    public async Task<Result> Handle(AcknowledgePanelInvitationCommand request, CancellationToken cancellationToken)
    {
        if (request.AcknowledgedBy == Guid.Empty)
        {
            return new ValidationError("Authenticated user is required.");
        }

        var panel = await panelRepository.GetByIdAsync(request.PanelId);
        if (panel is null)
        {
            return Result.Fail("Interview panel entry not found.");
        }

        panel.Acknowledge(request.AcknowledgedBy);

        await panelRepository.UpdateAsync(panel);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}

public record MarkPanelAttendanceCommand(Guid PanelId, bool IsAttended, Guid ModifiedBy) : IRequest<Result>;

public class MarkPanelAttendanceCommandValidator : AbstractValidator<MarkPanelAttendanceCommand>
{
    public MarkPanelAttendanceCommandValidator()
    {
        RuleFor(x => x.PanelId).NotEmpty().WithMessage("Interview panel entry is required.");
    }
}

internal sealed class MarkPanelAttendanceCommandHandler(
    IInterviewPanelRepository panelRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<MarkPanelAttendanceCommand, Result>
{
    public async Task<Result> Handle(MarkPanelAttendanceCommand request, CancellationToken cancellationToken)
    {
        if (request.ModifiedBy == Guid.Empty)
        {
            return new ValidationError("Authenticated user is required.");
        }

        var panel = await panelRepository.GetByIdAsync(request.PanelId);
        if (panel is null)
        {
            return Result.Fail("Interview panel entry not found.");
        }

        panel.MarkAttendance(request.IsAttended, request.ModifiedBy);

        await panelRepository.UpdateAsync(panel);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}