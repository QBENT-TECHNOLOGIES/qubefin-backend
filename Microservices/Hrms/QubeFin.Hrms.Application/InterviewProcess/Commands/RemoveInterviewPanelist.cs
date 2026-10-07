using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Hrms.Application.InterviewProcess.Services;

namespace QubeFin.Hrms.Application.InterviewProcess.Commands;
public record RemoveInterviewPanelistCommand(Guid CandidateId, Guid EmployeeId, Guid RemovedBy) : IRequest<Result<string>>, ICandidateWorkflowCommand;

public class RemoveInterviewPanelistCommandValidator : AbstractValidator<RemoveInterviewPanelistCommand>
{
    public RemoveInterviewPanelistCommandValidator()
    {
        RuleFor(x => x.CandidateId).NotEmpty().WithMessage("Candidate is required.");
        RuleFor(x => x.EmployeeId).NotEmpty().WithMessage("Panelist is required.");
    }
}

internal sealed class RemoveInterviewPanelistCommandHandler(IInterviewPanelRepository panelRepository, IUnitOfWork unitOfWork) : IRequestHandler<RemoveInterviewPanelistCommand, Result<string>>
{
    public async Task<Result<string>> Handle(RemoveInterviewPanelistCommand request, CancellationToken cancellationToken)
    {
        if (request.RemovedBy == Guid.Empty)
        {
            return new ValidationError("Authenticated user is required.");
        }

        var panel = await panelRepository.GetByCandidateAndEmployeeAsync(request.CandidateId, request.EmployeeId, includeEmployee: false);
        if (panel is null)
        {
            return new RecordNotFoundError("Interview panel entry not found for the given candidate and employee.");
        }

        if (panel.IsSubmitted)
        {
            return new ValidationError("This panelist has already submitted an assessment and cannot be removed.");
        }

        await panelRepository.DeleteAsync(panel.Id);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok("Panelist removed successfully.");
    }
}
