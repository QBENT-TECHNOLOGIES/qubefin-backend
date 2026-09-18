using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.InterviewProcess.Services;

/// <summary>Which candidate letter a status update / email relates to.</summary>
public enum CandidateLetterType
{
    InterviewLetter,
    OfferLetter,
    AppointmentLetter,
    WelcomeLetter,
}

/// <summary>Sends a candidate letter to the candidate by email, with the letter document attached.</summary>
public interface ICandidateLetterMailer
{
    Task SendLetterEmailAsync(Candidate candidate, CandidateLetterType letterType, CancellationToken cancellationToken = default);
}

/// <summary>
/// Stub implementation - intentionally left empty for now. To wire up real sending:
/// 1. Build or locate the letter document (e.g. render a PDF) for the given <see cref="CandidateLetterType"/>.
/// 2. Send an email to <c>candidate.Email</c> with that file attached, via whatever mail/SMTP provider
///    this project uses elsewhere (e.g. an IEmailSender / IMailService, if one exists in QubeFin.Core).
/// </summary>
internal sealed class CandidateLetterMailer : ICandidateLetterMailer
{
    public Task SendLetterEmailAsync(Candidate candidate, CandidateLetterType letterType, CancellationToken cancellationToken = default)
    {
        // TODO: implement - build the letter file for `letterType` and email it to `candidate.Email`.
        return Task.CompletedTask;
    }
}
