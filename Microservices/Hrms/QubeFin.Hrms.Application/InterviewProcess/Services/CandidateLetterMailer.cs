using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using QubeFin.Persistence.Models.Hrms;
using System.Net;
using System.Net.Mail;

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
    Task SendLetterEmailAsync(Candidate candidate, CandidateLetterType letterType, IFormFile file, CancellationToken cancellationToken = default);
}

/// <summary>
/// Stub implementation - intentionally left empty for now. To wire up real sending:
/// 1. Build or locate the letter document (e.g. render a PDF) for the given <see cref="CandidateLetterType"/>.
/// 2. Send an email to <c>candidate.Email</c> with that file attached, via whatever mail/SMTP provider
///    this project uses elsewhere (e.g. an IEmailSender / IMailService, if one exists in QubeFin.Core).
/// </summary>
internal sealed class CandidateLetterMailer(IOptions<MailSettings> options) : ICandidateLetterMailer
{
    private readonly MailSettings _settings = options.Value;
    public async Task SendLetterEmailAsync(Candidate candidate, CandidateLetterType letterType, IFormFile file, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(candidate.Email))
            throw new InvalidOperationException("Candidate email address is not available.");

        if (file == null || file.Length == 0)
            throw new InvalidOperationException("Letter file is required.");

        using var message = new MailMessage
        {
            From = new MailAddress(_settings.FromEmail, _settings.FromName),

            Subject = GetSubject(letterType),
            Body = GetEmailBody(candidate, letterType),
            IsBodyHtml = true
        };

        message.To.Add(candidate.Email);

        await using var fileStream = file.OpenReadStream();

        var attachment = new Attachment(fileStream, file.FileName, file.ContentType ?? "application/pdf");

        message.Attachments.Add(attachment);

        using var smtpClient = new SmtpClient(_settings.Host, _settings.Port)
        {
            EnableSsl = _settings.EnableSsl,
            Credentials = new NetworkCredential(_settings.Username, _settings.Password)
        };

        await smtpClient.SendMailAsync(message, cancellationToken);
    }

    private static string GetSubject(CandidateLetterType letterType)
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

    private static string GetEmailBody(Candidate candidate, CandidateLetterType letterType)
    {
        var letterName = letterType switch
        {
            CandidateLetterType.InterviewLetter => "Interview Letter",
            CandidateLetterType.OfferLetter => "Offer Letter",
            CandidateLetterType.AppointmentLetter => "Appointment Letter",
            CandidateLetterType.WelcomeLetter => "Welcome Letter",
            _ => "Letter"
        };

        return $"""
            <html>
            <body>
                <p>Dear {candidate.FirstName} {candidate.LastName},</p>

                <p>
                    Please find your <strong>{letterName}</strong>
                    attached to this email.
                </p>

                <p>
                    Regards,<br />
                    <strong>QubeFin HRMS</strong>
                </p>
            </body>
            </html>
            """;
    }
}
