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
        var fullName = WebUtility.HtmlEncode($"{candidate.FirstName} {candidate.LastName}".Trim());

        var (letterName, content) = letterType switch
        {
            CandidateLetterType.InterviewLetter => ("Interview Letter", """
            <p>Thank you for your interest in the position you applied for. After reviewing your application and credentials, we are pleased to inform you that you have been shortlisted for an interview.</p>
            <p>The interview date, time and venue are mentioned in the attached <strong>Interview Letter</strong>. Please bring the following with you on the day of the interview:</p>
            <ul>
                <li>A copy of the attached Interview Letter</li>
                <li>Your updated Curriculum Vitae</li>
                <li>Copies of your educational qualifications and other relevant documents</li>
            </ul>
            <p>If you have any queries or wish to reschedule, please reply to this email at your earliest convenience.</p>
            <p>We look forward to meeting you.</p>
            """),

            CandidateLetterType.OfferLetter => ("Offer Letter", """
            <p>Congratulations! Based on your performance in the selection process, we are delighted to extend to you an offer of employment.</p>
            <p>Please find the attached <strong>Offer Letter</strong>, which contains your designation, place of posting, date of joining, compensation and the terms and conditions of the offer.</p>
            <p>Kindly review the letter carefully and confirm your acceptance by signing and returning a copy. Please note that the offer is subject to the submission of required documents and satisfactory completion of the pre-employment verification process.</p>
            <p>Should you need any clarification, feel free to reply to this email.</p>
            <p>We congratulate you once again and look forward to having you with us.</p>
            """),

            CandidateLetterType.AppointmentLetter => ("Appointment Letter", """
            <p>With reference to your acceptance of our offer, we are pleased to share your <strong>Appointment Letter</strong>, attached to this email.</p>
            <p>The letter outlines the terms and conditions of your employment, including designation, posting, compensation, probation, duties and responsibilities, and other service conditions.</p>
            <p>Kindly read the letter carefully, sign the duplicate copy as a token of your acceptance, and submit it on or before your date of joining.</p>
            <p>If you have any questions regarding the terms, please reply to this email and we will be happy to assist you.</p>
            """),

            CandidateLetterType.WelcomeLetter => ("Welcome Letter", """
            <p>A warm welcome to the team! We are excited to have you on board and hope you will find your work here both rewarding and challenging.</p>
            <p>Please find your <strong>Welcome Letter</strong> attached. As part of your onboarding, an orientation session will be arranged to introduce you to our work environment, policies and colleagues. The HR team will also share the employee handbook with you.</p>
            <p>We encourage open communication, so please feel free to share your ideas, suggestions or concerns at any time.</p>
            <p>We look forward to your contribution and wish you a successful journey ahead.</p>
            """),

            _ => ("Letter", """
            <p>Please find the attached letter for your reference. Kindly review it and reach out to us if you have any questions.</p>
            """)
        };

        return $"""
        <html>
        <body style="font-family: Arial, Helvetica, sans-serif; font-size: 14px; color: #333333; line-height: 1.6;">
            <p>Dear {fullName},</p>

            {content}

            <p>
                Warm Regards,<br />
                <strong>Human Resources Department</strong>
            </p>

            <hr style="border: none; border-top: 1px solid #dddddd; margin-top: 24px;" />
            <p style="font-size: 12px; color: #888888;">
                This is a system-generated email sent regarding your {letterName}. Please do not share the attached document with unauthorized persons.
            </p>
        </body>
        </html>
        """;
    }
}
