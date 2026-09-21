using FluentResults;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Persistence.Repositories;

namespace QubeFin.Hrms.Application.InterviewProcess.Commands;

/// <summary>Placeholder for the "Add Additional Info" action shown once the candidate's Offer Letter has been
/// received. Intentionally does nothing yet - the fields/behavior for this step haven't been defined.</summary>
public record AddCandidateAdditionalInfoCommand(Guid CandidateId, Guid ModifiedBy) : IRequest<Result<string>>;

internal sealed class AddCandidateAdditionalInfoCommandHandler(ICandidateRepository candidateRepository)
    : IRequestHandler<AddCandidateAdditionalInfoCommand, Result<string>>
{
    public async Task<Result<string>> Handle(AddCandidateAdditionalInfoCommand request, CancellationToken cancellationToken)
    {
        if (request.ModifiedBy == Guid.Empty)
        {
            return new ValidationError("Authenticated user is required.");
        }

        var candidate = await candidateRepository.GetByIdAsync(request.CandidateId);
        if (candidate is null)
        {
            return new RecordNotFoundError("Candidate not found.");
        }

        // TODO: not implemented yet - define what "additional info" HR needs to capture here.
        return Result.Ok("Not implemented yet.");
    }
}
