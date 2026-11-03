using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;
using System.Net;

namespace QubeFin.Hrms.Application.InterviewProcess.Services;

/// <summary>How the panel invitations went: panelists skipped for having no email, and those whose mail failed.</summary>
public record PanelInvitationResult(int MissingEmail, int Failed, string? FirstError);

public interface IPanelInvitationMailer
{
    /// <summary>Emails each panelist from the HR mailbox of the candidate's company.</summary>
    Task<PanelInvitationResult> SendInvitationsAsync(Candidate candidate, IReadOnlyList<PanelistScheduleDto> panelists, IFormFile? acknowledgementFile, CancellationToken cancellationToken = default);
}

internal sealed class PanelInvitationMailer(QubeFinDataContext context, IHrMailSender hrMailSender) : IPanelInvitationMailer
{
    public async Task<PanelInvitationResult> SendInvitationsAsync(Candidate candidate, IReadOnlyList<PanelistScheduleDto> panelists, IFormFile? acknowledgementFile, CancellationToken cancellationToken = default)
    {
        var employeeIds = panelists.Select(p => p.EmployeeId).ToList();
        var employees = await context.TblEmployees
            .AsNoTracking()
            .Where(e => employeeIds.Contains(e.Id))
            .Select(e => new { e.Id, e.FullName, Email = e.OfficialEmail ?? e.PersonalEmail })
            .ToDictionaryAsync(e => e.Id, cancellationToken);

        // Read the upload once - every panelist's mail carries the same attachment.
        var attachments = new List<HrMailAttachment>();
        if (acknowledgementFile is { Length: > 0 })
        {
            await using var stream = acknowledgementFile.OpenReadStream();
            using var buffer = new MemoryStream();
            await stream.CopyToAsync(buffer, cancellationToken);
            attachments.Add(new HrMailAttachment(
                string.IsNullOrWhiteSpace(acknowledgementFile.FileName) ? "interview_panel_acknowledgement.pdf" : acknowledgementFile.FileName,
                acknowledgementFile.ContentType ?? "application/pdf",
                buffer.ToArray()));
        }

        var missingEmail = 0;
        var failed = 0;
        string? firstError = null;
        foreach (var panelist in panelists)
        {
            if (!employees.TryGetValue(panelist.EmployeeId, out var employee) || string.IsNullOrWhiteSpace(employee.Email))
            {
                missingEmail++;
                continue;
            }

            var mail = new HrMail(
                employee.Email,
                $"Interview Panel Invitation - {candidate.ReferenceNo}",
                GetEmailBody(employee.FullName, candidate, panelist),
                attachments);

            var result = await hrMailSender.SendAsync(candidate.CompanyId, mail, cancellationToken);
            if (result.IsFailed)
            {
                failed++;
                firstError ??= result.Errors.FirstOrDefault()?.Message;
            }
        }

        return new PanelInvitationResult(missingEmail, failed, firstError);
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
