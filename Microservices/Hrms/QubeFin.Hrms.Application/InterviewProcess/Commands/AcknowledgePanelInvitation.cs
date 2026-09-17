using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.InterviewProcess.Commands;

public record AcknowledgePanelInvitationCommand(Guid CandidateId, Guid EmployeeId, Guid AcknowledgedBy) : IRequest<Result<string>>;

public class AcknowledgePanelInvitationCommandValidator : AbstractValidator<AcknowledgePanelInvitationCommand>
{
    public AcknowledgePanelInvitationCommandValidator()
    {
        RuleFor(x => x.CandidateId).NotEmpty().WithMessage("Interview panel entry is required.");
    }
}

internal sealed class AcknowledgePanelInvitationCommandHandler(IInterviewPanelRepository panelRepository, IUnitOfWork unitOfWork) : IRequestHandler<AcknowledgePanelInvitationCommand, Result<string>>
{
    public async Task<Result<string>> Handle(AcknowledgePanelInvitationCommand request, CancellationToken cancellationToken)
    {
        if (request.AcknowledgedBy == Guid.Empty)
        {
            return new ValidationError("Authenticated user is required.");
        }

        var panel = await panelRepository.GetByEmployeeIdAsync(request.CandidateId, request.EmployeeId);
        if (panel is null)
        {
            return Result.Fail("Interview panel entry not found.");
        }

        panel.Acknowledge(request.AcknowledgedBy);

        await panelRepository.UpdateAsync(panel);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok("Panel invitation acknowledged successfully.");
    }
}