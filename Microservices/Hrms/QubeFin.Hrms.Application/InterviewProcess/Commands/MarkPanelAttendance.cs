using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.InterviewProcess.Commands;

public record MarkPanelAttendanceCommand(Guid PanelId, bool IsAttended, Guid ModifiedBy) : IRequest<Result<string>>;

public class MarkPanelAttendanceCommandValidator : AbstractValidator<MarkPanelAttendanceCommand>
{
    public MarkPanelAttendanceCommandValidator()
    {
        RuleFor(x => x.PanelId).NotEmpty().WithMessage("Interview panel entry is required.");
    }
}

internal sealed class MarkPanelAttendanceCommandHandler(IInterviewPanelRepository panelRepository, IUnitOfWork unitOfWork) : IRequestHandler<MarkPanelAttendanceCommand, Result<string>>
{
    public async Task<Result<string>> Handle(MarkPanelAttendanceCommand request, CancellationToken cancellationToken)
    {
        if (request.ModifiedBy == Guid.Empty)
        {
            return Result.Fail("Authenticated user is required.");
        }

        var panel = await panelRepository.GetByIdAsync(request.PanelId);
        if (panel is null)
        {
            return Result.Fail("Interview panel entry not found.");
        }

        panel.MarkAttendance(request.IsAttended, request.ModifiedBy);

        await panelRepository.UpdateAsync(panel);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok("Panel attendance marked successfully.");
    }
}
