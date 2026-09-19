using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Hrms.Application.InterviewProcess.Services;

namespace QubeFin.Hrms.Application.InterviewProcess.Commands;

public record SendLetterToCandidateCommand(Guid CandidateId, CandidateLetterStatusRequest LetterStatus, Guid ModifiedBy) : IRequest<Result<string>>;

public class SendLetterToCandidateCommandValidator : AbstractValidator<SendLetterToCandidateCommand>
{
    public SendLetterToCandidateCommandValidator()
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

internal sealed class SendLetterToCandidateCommandHandler() : IRequestHandler<SendLetterToCandidateCommand, Result<string>>
{
    public async Task<Result<string>> Handle(SendLetterToCandidateCommand request, CancellationToken cancellationToken)
    {

        var letterStatus = request.LetterStatus;

        var letterType = ResolveLetterType(letterStatus);

        return Result.Ok($"{letterType} mail sent successfully.");
    }

    private static CandidateLetterType ResolveLetterType(CandidateLetterStatusRequest letterStatus)
    {
        if (letterStatus.IsInterviewLetterReceived.HasValue) return CandidateLetterType.InterviewLetter;
        if (letterStatus.IsOfferLetterReceived.HasValue) return CandidateLetterType.OfferLetter;
        if (letterStatus.IsAppointmentLetterReceived.HasValue) return CandidateLetterType.AppointmentLetter;
        return CandidateLetterType.WelcomeLetter;
    }
}