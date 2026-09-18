using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Hrms.Application.InterviewProcess.Services;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.InterviewProcess.Commands;

/// <summary>
/// Updates one letter-received flag on a candidate (send exactly one of the four flags per call) and
/// triggers the corresponding letter email.
/// </summary>
public record UpdateCandidateLetterStatusCommand(Guid CandidateId, CandidateLetterStatusRequest LetterStatus, Guid ModifiedBy) : IRequest<Result<string>>;

public class UpdateCandidateLetterStatusCommandValidator : AbstractValidator<UpdateCandidateLetterStatusCommand>
{
    public UpdateCandidateLetterStatusCommandValidator()
    {
        RuleFor(x => x.CandidateId).NotEmpty().WithMessage("Candidate is required.");

        RuleFor(x => x.LetterStatus)
            .Must(HasExactlyOneFlag)
            .WithMessage("Send exactly one letter status flag per request.");
    }

    private static bool HasExactlyOneFlag(CandidateLetterStatusRequest request)
    {
        var flags = new[]
        {
            request.IsInterviewLetterReceived,
            request.IsOfferLetterReceived,
            request.IsAppointmentLetterReceived,
            request.IsWelcomeLetterReceived,
        };

        return flags.Count(f => f.HasValue) == 1;
    }
}

internal sealed class UpdateCandidateLetterStatusCommandHandler(
    ICandidateRepository candidateRepository,
    ICandidateLetterMailer candidateLetterMailer,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateCandidateLetterStatusCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateCandidateLetterStatusCommand request, CancellationToken cancellationToken)
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

        var letterStatus = request.LetterStatus;

        candidate.UpdateLetterStatus(
            letterStatus.IsInterviewLetterReceived,
            letterStatus.IsOfferLetterReceived,
            letterStatus.IsAppointmentLetterReceived,
            letterStatus.IsWelcomeLetterReceived,
            request.ModifiedBy);

        await candidateRepository.UpdateAsync(candidate);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var letterType = ResolveLetterType(letterStatus);
        await candidateLetterMailer.SendLetterEmailAsync(candidate, letterType, cancellationToken);

        return Result.Ok("Candidate letter status updated successfully.");
    }

    private static CandidateLetterType ResolveLetterType(CandidateLetterStatusRequest letterStatus)
    {
        if (letterStatus.IsInterviewLetterReceived.HasValue) return CandidateLetterType.InterviewLetter;
        if (letterStatus.IsOfferLetterReceived.HasValue) return CandidateLetterType.OfferLetter;
        if (letterStatus.IsAppointmentLetterReceived.HasValue) return CandidateLetterType.AppointmentLetter;
        return CandidateLetterType.WelcomeLetter;
    }
}
