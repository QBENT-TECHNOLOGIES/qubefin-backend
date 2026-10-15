using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;
using System.Net;
using System.Net.Mail;

namespace QubeFin.Hrms.Application.InterviewProcess.Services;

public interface IPanelInvitationMailer
{
    Task<int> SendInvitationsAsync(Candidate candidate, IReadOnlyList<PanelistScheduleDto> panelists, IFormFile? acknowledgementFile, CancellationToken cancellationToken = default);

    /// <summary>Panel Tagging: one mail per panelist listing every candidate they were just added for.
    /// Acknowledgement files are matched to candidates by file name ("{candidateId}.pdf").</summary>
    Task<int> SendPanelAssignmentsAsync(IReadOnlyDictionary<Guid, List<Candidate>> candidatesByEmployee, IReadOnlyList<IFormFile> acknowledgementFiles, CancellationToken cancellationToken = default);
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

    public async Task<int> SendPanelAssignmentsAsync(IReadOnlyDictionary<Guid, List<Candidate>> candidatesByEmployee, IReadOnlyList<IFormFile> acknowledgementFiles, CancellationToken cancellationToken = default)
    {
        var employeeIds = candidatesByEmployee.Keys.ToList();
        var employees = await context.TblEmployees
            .AsNoTracking()
            .Where(e => employeeIds.Contains(e.Id))
            .Select(e => new { e.Id, e.FullName, Email = e.OfficialEmail ?? e.PersonalEmail })
            .ToDictionaryAsync(e => e.Id, cancellationToken);

        // Read each upload once - every panelist's message gets its own attachment streams.
        var filesByCandidate = new Dictionary<Guid, (string ContentType, byte[] Bytes)>();
        foreach (var file in acknowledgementFiles.Where(f => f.Length > 0))
        {
            if (!Guid.TryParse(Path.GetFileNameWithoutExtension(file.FileName), out var candidateId))
            {
                continue;
            }

            await using var stream = file.OpenReadStream();
            using var buffer = new MemoryStream();
            await stream.CopyToAsync(buffer, cancellationToken);
            filesByCandidate[candidateId] = (file.ContentType ?? "application/pdf", buffer.ToArray());
        }

        using var smtpClient = new SmtpClient(_settings.Host, _settings.Port)
        {
            EnableSsl = _settings.EnableSsl,
            Credentials = new NetworkCredential(_settings.Username, _settings.Password)
        };

        var missingEmail = 0;
        foreach (var (employeeId, candidates) in candidatesByEmployee)
        {
            if (!employees.TryGetValue(employeeId, out var employee) || string.IsNullOrWhiteSpace(employee.Email))
            {
                missingEmail++;
                continue;
            }

            var ordered = candidates.OrderBy(c => c.InterviewDate).ThenBy(c => c.InterviewTime).ToList();
            using var message = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail, _settings.FromName),
                Subject = ordered.Count == 1
                    ? $"Interview Panel Invitation - {ordered[0].ReferenceNo}"
                    : $"Interview Panel Invitation - {ordered.Count} candidates on {ordered[0].InterviewDate:dd/MM/yyyy}",
                Body = GetPanelAssignmentBody(employee.FullName, ordered),
                IsBodyHtml = true
            };
            message.To.Add(employee.Email);

            foreach (var candidate in ordered)
            {
                if (filesByCandidate.TryGetValue(candidate.Id, out var file))
                {
                    message.Attachments.Add(new Attachment(new MemoryStream(file.Bytes), $"interview_panel_acknowledgement_{candidate.ReferenceNo?.Replace('/', '_') ?? candidate.Id.ToString()}.pdf", file.ContentType));
                }
            }

            await smtpClient.SendMailAsync(message, cancellationToken);
        }

        return missingEmail;
    }

    // Same wording as the single-candidate invitation; the candidates are listed in a table.
    private static string GetPanelAssignmentBody(string employeeName, IReadOnlyList<Candidate> candidates)
    {
        const string cell = "padding: 4px 8px; border: 1px solid #dddddd; text-align: left;";

        var rows = string.Concat(candidates.Select(c => $"""
                    <tr>
                        <td style="{cell}">{WebUtility.HtmlEncode($"{c.FirstName} {c.LastName}")}</td>
                        <td style="{cell}">{WebUtility.HtmlEncode(c.ReferenceNo)}</td>
                        <td style="{cell}">{WebUtility.HtmlEncode(c.InterviewPostName)}</td>
                        <td style="{cell}">{c.InterviewDate:dd/MM/yyyy}</td>
                        <td style="{cell}">{c.InterviewTime:hh\:mm tt}</td>
                    </tr>
            """));

        return $"""
            <html>
            <body>
                <p>Dear {WebUtility.HtmlEncode(employeeName)},</p>

                <p>You have been added to the interview panel for the following candidate(s):</p>

                <table style="border-collapse: collapse;">
                    <tr>
                        <th style="{cell}">Candidate</th>
                        <th style="{cell}">Ref. No.</th>
                        <th style="{cell}">Post</th>
                        <th style="{cell}">Date</th>
                        <th style="{cell}">Time</th>
                    </tr>
            {rows}
                </table>

                <p>Please find the interview panel acknowledgement attached to this email.</p>

                <p>
                    Regards,<br />
                    <strong>QubeFin HRMS</strong>
                </p>
            </body>
            </html>
            """;
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
