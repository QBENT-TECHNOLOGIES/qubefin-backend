using Azure.Identity;
using FluentResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using Microsoft.Graph.Users.Item.SendMail;
using Microsoft.Kiota.Http.HttpClientLibrary.Middleware;

namespace QubeFin.Hrms.Application.InterviewProcess.Services;

/// <summary>A file attached to an HR mail.</summary>
public record HrMailAttachment(string FileName, string ContentType, byte[] Content);

/// <summary>One HR mail: recipient, subject, HTML body and attachments.</summary>
public record HrMail(string To, string Subject, string HtmlBody, IReadOnlyList<HrMailAttachment> Attachments);

/// <summary>Sends HR mail through Microsoft Graph from the HR mailbox of the given company
/// (MailSettings:MicrosoftGraph:JoiningEmail:CompanySenders).</summary>
public interface IHrMailSender
{
    Task<Result> SendAsync(Guid companyId, HrMail mail, CancellationToken cancellationToken = default);
}

internal sealed class HrMailSender(GraphServiceClient graphClient, IOptions<MailSettings> options, ILogger<HrMailSender> logger) : IHrMailSender
{
    public async Task<Result> SendAsync(Guid companyId, HrMail mail, CancellationToken cancellationToken = default)
    {
        var sender = ResolveSender(companyId);
        if (sender is null)
        {
            logger.LogError("HR mail not sent: no sender mailbox is configured for company {CompanyId}. Subject: {Subject}", companyId, mail.Subject);
            return Result.Fail($"No HR mailbox is configured for the candidate's company ({companyId}).");
        }

        var message = new Message
        {
            Subject = mail.Subject,
            Body = new ItemBody { ContentType = BodyType.Html, Content = mail.HtmlBody },
            From = new Recipient { EmailAddress = new EmailAddress { Address = sender.Email, Name = sender.DisplayName } },
            ToRecipients = [new Recipient { EmailAddress = new EmailAddress { Address = mail.To } }],
            Attachments = mail.Attachments
                .Select(a => (Attachment)new FileAttachment
                {
                    OdataType = "#microsoft.graph.fileAttachment",
                    Name = a.FileName,
                    ContentType = a.ContentType,
                    ContentBytes = a.Content
                })
                .ToList()
        };

        try
        {
            logger.LogInformation("Sending HR mail '{Subject}' from {Sender} to {Recipient} (company {CompanyId}).", mail.Subject, sender.Email, mail.To, companyId);

            await graphClient.Users[sender.Email].SendMail.PostAsync(
                new SendMailPostRequestBody { Message = message, SaveToSentItems = true },
                cancellationToken: cancellationToken);

            logger.LogInformation("HR mail '{Subject}' sent from {Sender} to {Recipient}.", mail.Subject, sender.Email, mail.To);
            return Result.Ok();
        }
        catch (AuthenticationFailedException ex)
        {
            // Wrong tenant / client id / secret, or the secret expired. The secret itself is never logged.
            logger.LogError(ex, "Microsoft Graph authentication failed while sending HR mail from {Sender}.", sender.Email);
            return Result.Fail("The mail service could not sign in to Microsoft 365. Please contact the administrator.");
        }
        catch (ODataError ex)
        {
            // e.g. ErrorAccessDenied when the app has no Mail.Send right on this mailbox, or a rejected recipient.
            logger.LogError(ex, "Microsoft Graph rejected HR mail from {Sender} to {Recipient}: {Code} {Message}",
                sender.Email, mail.To, ex.Error?.Code, ex.Error?.Message);
            return Result.Fail($"The mail could not be sent from {sender.Email}: {ex.Error?.Message ?? "Microsoft 365 rejected the request."}");
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Sending HR mail from {Sender} to {Recipient} failed.", sender.Email, mail.To);
            return Result.Fail("The mail could not be sent. Please try again later.");
        }
    }

    private MailSender? ResolveSender(Guid companyId)
    {
        foreach (var (key, sender) in options.Value.MicrosoftGraph.JoiningEmail.CompanySenders)
        {
            if (Guid.TryParse(key, out var configuredCompanyId) && configuredCompanyId == companyId)
            {
                return sender;
            }
        }

        return null;
    }

    /// <summary>One client for the app's lifetime: client-credentials sign-in, token cached by Azure.Identity.
    /// Built without the Graph retry handler - sendMail is not idempotent, so a retry after a timeout could deliver
    /// the same letter twice.</summary>
    public static GraphServiceClient CreateGraphClient(MicrosoftGraphMailSettings settings)
    {
        var credential = new ClientSecretCredential(settings.TenantId, settings.ClientId, settings.ClientSecret);

        var handlers = GraphClientFactory.CreateDefaultHandlers();
        foreach (var retry in handlers.OfType<RetryHandler>().ToList())
        {
            handlers.Remove(retry);
        }

        var httpClient = GraphClientFactory.Create(handlers);
        return new GraphServiceClient(httpClient, credential, ["https://graph.microsoft.com/.default"]);
    }
}
