using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Hrms.Application.InterviewProcess.Services;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.InterviewProcess.Commands;

public record SendLetterToCandidateCommand(Guid CandidateId, CandidateLetterStatusRequest LetterStatus, Guid ModifiedBy) : IRequest<Result<string>>;


public class SendLetterToCandidateCommandValidator : AbstractValidator<SendLetterToCandidateCommand>
{
    public SendLetterToCandidateCommandValidator()
    {
        RuleFor(x => x.CandidateId).NotEmpty().WithMessage("Candidate is required.");
        RuleFor(x => x.LetterStatus).Must(HasExactlyOneFlag).WithMessage("Send exactly one letter status flag per request.");
        RuleFor(x => x.LetterStatus.File).NotNull().WithMessage("File is required.").Must(file => file != null && file.Length > 0).WithMessage("File cannot be empty.");
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


internal sealed class SendLetterToCandidateCommandHandler(QubeFinDataContext context, ICandidateLetterMailer candidateLetterMailer, ICandidateRepository candidateRepository) : IRequestHandler<SendLetterToCandidateCommand, Result<string>>
{
    public async Task<Result<string>> Handle(SendLetterToCandidateCommand request, CancellationToken cancellationToken)
    {
        var letterStatus = request.LetterStatus;
        var letterType = ResolveLetterType(letterStatus);
        var candidate = await candidateRepository.GetByIdAsync(request.CandidateId);

        if (candidate == null)
        {
            return Result.Fail("Candidate not found.");
        }

        if (string.IsNullOrWhiteSpace(candidate.Email))
        {
            return Result.Fail("Candidate email address is not available.");
        }

        if (letterStatus.File == null || letterStatus.File.Length == 0)
        {
            return Result.Fail("File is required.");
        }

        await candidateLetterMailer.SendLetterEmailAsync(candidate, letterType, letterStatus.File, cancellationToken);

        return Result.Ok($"{GetLetterName(letterType)} mail sent successfully.");
    }


    private static CandidateLetterType ResolveLetterType(CandidateLetterStatusRequest letterStatus)
    {
        if (letterStatus.IsInterviewLetterReceived.HasValue)
            return CandidateLetterType.InterviewLetter;

        if (letterStatus.IsOfferLetterReceived.HasValue)
            return CandidateLetterType.OfferLetter;

        if (letterStatus.IsAppointmentLetterReceived.HasValue)
            return CandidateLetterType.AppointmentLetter;

        return CandidateLetterType.WelcomeLetter;
    }


    private static string GetLetterName(CandidateLetterType letterType)
    {
        return letterType switch
        {
            CandidateLetterType.InterviewLetter => "Interview Letter",
            CandidateLetterType.OfferLetter => "Offer Letter",
            CandidateLetterType.AppointmentLetter => "Appointment Letter",
            CandidateLetterType.WelcomeLetter => "Welcome Letter",
            _ => "Candidate Letter"
        };
    }
}