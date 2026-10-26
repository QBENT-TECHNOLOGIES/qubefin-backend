using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.InterviewProcess.Services;
using QubeFin.Hrms.Persistence.Repositories;

namespace QubeFin.Hrms.Application.InterviewProcess.Commands;

public record AcknowledgeInterviewsCommand(IReadOnlyList<Guid> CandidateIds, Guid EmployeeId, Guid AcknowledgedBy) : IRequest<Result<string>>;

public record AcknowledgeInterviewsRequest(List<Guid> CandidateIds);

public class AcknowledgeInterviewsCommandValidator : AbstractValidator<AcknowledgeInterviewsCommand>
{
    public AcknowledgeInterviewsCommandValidator()
    {
        RuleFor(x => x.CandidateIds).NotEmpty().WithMessage("Select at least one interview to acknowledge.");
        RuleFor(x => x.CandidateIds.Count).LessThanOrEqualTo(200).WithMessage("Acknowledge at most 200 interviews at a time.");
    }
}

internal sealed class AcknowledgeInterviewsCommandHandler(IInterviewPanelRepository panelRepository) : IRequestHandler<AcknowledgeInterviewsCommand, Result<string>>
{
    public async Task<Result<string>> Handle(AcknowledgeInterviewsCommand request, CancellationToken cancellationToken)
    {
        if (request.AcknowledgedBy == Guid.Empty || request.EmployeeId == Guid.Empty)
        {
            return new ValidationError("Authenticated user is required.");
        }

        var candidateIds = request.CandidateIds.Distinct().ToList();
        var acknowledged = await panelRepository.AcknowledgeAsync(request.EmployeeId, candidateIds, CandidateWorkflow.Today, request.AcknowledgedBy, cancellationToken);

        if (acknowledged == 0)
        {
            return new ValidationError("None of the selected interviews can be acknowledged - they are already acknowledged or closed.");
        }

        return Result.Ok(acknowledged == candidateIds.Count
            ? $"{acknowledged} interview(s) acknowledged successfully."
            : $"{acknowledged} of {candidateIds.Count} interview(s) acknowledged. The rest were already acknowledged or closed.");
    }
}
