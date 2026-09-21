using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.InterviewProcess.Commands;

/// <summary>Saves the free-text "Additional Info" note (flow doc step 7) - shown after the candidate has
/// accepted/received the Offer Letter and required before the Appointment Letter can be generated.</summary>
public record UpdateCandidateAdditionalInfoCommand(Guid CandidateId, string? AdditionalInfo, Guid ModifiedBy) : IRequest<Result<string>>;

public class UpdateCandidateAdditionalInfoCommandValidator : AbstractValidator<UpdateCandidateAdditionalInfoCommand>
{
    public UpdateCandidateAdditionalInfoCommandValidator()
    {
        RuleFor(x => x.CandidateId).NotEmpty().WithMessage("Candidate is required.");
        RuleFor(x => x.AdditionalInfo).NotEmpty().WithMessage("Additional info cannot be empty.");
    }
}

internal sealed class UpdateCandidateAdditionalInfoCommandHandler(
    ICandidateRepository candidateRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateCandidateAdditionalInfoCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateCandidateAdditionalInfoCommand request, CancellationToken cancellationToken)
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

        if (!candidate.IsOfferLetterReceived)
        {
            return new ValidationError("Additional Info can only be added after the Offer Letter has been received/accepted.");
        }

        //candidate.SetAdditionalInfo(request.AdditionalInfo, request.ModifiedBy);

        await candidateRepository.UpdateAsync(candidate);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok("Additional info saved successfully.");
    }
}
