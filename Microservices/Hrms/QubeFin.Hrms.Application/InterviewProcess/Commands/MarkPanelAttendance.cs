using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.InterviewProcess.Commands;

/// <summary>Marks whether a panelist attended the interview.</summary>
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
            return new ValidationError("Authenticated user is required.");
        }

        var panel = await panelRepository.GetByIdAsync(request.PanelId);
        if (panel is null)
        {
            return new RecordNotFoundError("Interview panel entry not found.");
        }

        panel.MarkAttendance(request.IsAttended, request.ModifiedBy);

        await panelRepository.UpdateAsync(panel);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok("Attendance updated successfully.");
    }
}
