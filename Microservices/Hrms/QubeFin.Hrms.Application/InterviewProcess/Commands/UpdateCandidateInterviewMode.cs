using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.InterviewProcess.Commands;

/// <summary>Sets whether the candidate's interview was conducted Online or Offline. Editable on its own
/// (e.g. from the HR Assessment form) rather than only via the full candidate update.</summary>
public record UpdateCandidateInterviewModeCommand(Guid CandidateId, string InterviewMode, Guid ModifiedBy) : IRequest<Result<string>>;

public class UpdateCandidateInterviewModeCommandValidator : AbstractValidator<UpdateCandidateInterviewModeCommand>
{
    public UpdateCandidateInterviewModeCommandValidator()
    {
        RuleFor(x => x.CandidateId).NotEmpty().WithMessage("Candidate is required.");
        RuleFor(x => x.InterviewMode).Must(m => m is "Online" or "Offline").WithMessage("Interview mode must be 'Online' or 'Offline'.");
    }
}

internal sealed class UpdateCandidateInterviewModeCommandHandler(
    ICandidateRepository candidateRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateCandidateInterviewModeCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateCandidateInterviewModeCommand request, CancellationToken cancellationToken)
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

        candidate.SetInterviewMode(request.InterviewMode, request.ModifiedBy);

        await candidateRepository.UpdateAsync(candidate);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok("Interview mode updated successfully.");
    }
}
