using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;
using System.Net;
using System.Net.Mail;

namespace QubeFin.Hrms.Application.InterviewProcess.Services;

/// <summary>Emails newly scheduled panelists their interview invitation, with the panel acknowledgement attached.</summary>
public interface IPanelInvitationMailer
{
    /// <summary>Sends one email per panelist and returns how many panelists had no email address to send to.</summary>
    Task<int> SendInvitationsAsync(Candidate candidate, IReadOnlyList<PanelistScheduleDto> panelists, IFormFile? acknowledgementFile, CancellationToken cancellationToken = default);
}

internal sealed class PanelInvitationMailer(QubeFinDataContext context, IOptions<MailSettings> options) : IPanelInvitationMailer
{
    private readonly MailSettings _settings = options.Value;

    public async Task<int> SendInvitationsAsync(Candidate candidate, IReadOnlyList<PanelistScheduleDto> panelists, IFormFile? acknowledgementFile, CancellationToken cancellationToken = default)
    {
        var employeeIds = panelists.Select(p => p.EmployeeId).ToList();
        var employees = await context.TblEmployees
            .AsNoTracking()
            .Where(e => employeeIds.Contains(e.Id))
            .Select(e => new { e.Id, e.FullName, Email = e.OfficialEmail ?? e.PersonalEmail })
            .ToDictionaryAsync(e => e.Id, cancellationToken);

        // Read the upload once - every panelist's message gets its own attachment stream.
        byte[]? fileBytes = null;
        if (acknowledgementFile is { Length: > 0 })
        {
            await using var stream = acknowledgementFile.OpenReadStream();
            using var buffer = new MemoryStream();
            await stream.CopyToAsync(buffer, cancellationToken);
            fileBytes = buffer.ToArray();
        }

        using var smtpClient = new SmtpClient(_settings.Host, _settings.Port)
        {
            EnableSsl = _settings.EnableSsl,
            Credentials = new NetworkCredential(_settings.Username, _settings.Password)
        };

        var missingEmail = 0;
        foreach (var panelist in panelists)
        {
            if (!employees.TryGetValue(panelist.EmployeeId, out var employee) || string.IsNullOrWhiteSpace(employee.Email))
            {
                missingEmail++;
                continue;
            }

            using var message = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail, _settings.FromName),
                Subject = $"Interview Panel Invitation - {candidate.ReferenceNo}",
                Body = GetEmailBody(employee.FullName, candidate, panelist),
                IsBodyHtml = true
            };
            message.To.Add(employee.Email);

            if (fileBytes is not null)
            {
                message.Attachments.Add(new Attachment(
                    new MemoryStream(fileBytes),
                    string.IsNullOrWhiteSpace(acknowledgementFile!.FileName) ? "interview_panel_acknowledgement.pdf" : acknowledgementFile.FileName,
                    acknowledgementFile.ContentType ?? "application/pdf"));
            }

            await smtpClient.SendMailAsync(message, cancellationToken);
        }

        return missingEmail;
    }

    private static string GetEmailBody(string employeeName, Candidate candidate, PanelistScheduleDto panelist)
    {
        return $"""
            <html>
            <body>
                <p>Dear {WebUtility.HtmlEncode(employeeName)},</p>

                <p>
                    You have been added to the interview panel for
                    <strong>{WebUtility.HtmlEncode($"{candidate.FirstName} {candidate.LastName}")}</strong>
                    (Ref. {WebUtility.HtmlEncode(candidate.ReferenceNo)}) for the post of
                    <strong>{WebUtility.HtmlEncode(candidate.InterviewPostName)}</strong>.
                </p>

                <p>
                    Scheduled on <strong>{panelist.ScheduledDate:dd/MM/yyyy}</strong>
                    at <strong>{panelist.ScheduledTime:hh\:mm tt}</strong>.
                </p>

                <p>Please find the interview panel acknowledgement attached to this email.</p>

                <p>
                    Regards,<br />
                    <strong>QubeFin HRMS</strong>
                </p>
            </body>
            </html>
            """;
    }
}
