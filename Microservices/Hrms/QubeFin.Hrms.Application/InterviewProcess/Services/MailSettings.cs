using Microsoft.Extensions.Options;
using System.Net.Mail;

namespace QubeFin.Hrms.Application.InterviewProcess.Services;

/// <summary>The "MailSettings" section. Every HRMS mail goes through Microsoft Graph, sent from the HR mailbox of the
/// candidate's company.</summary>
public class MailSettings
{
    public MicrosoftGraphMailSettings MicrosoftGraph { get; set; } = new();
}

/// <summary>"MailSettings:MicrosoftGraph" - the Entra app (client credentials) and the HR mailbox of each company.
/// Keep ClientSecret out of the file: set MailSettings__MicrosoftGraph__ClientSecret as an environment variable.</summary>
public class MicrosoftGraphMailSettings
{
    public string TenantId { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public JoiningEmailSettings JoiningEmail { get; set; } = new();
}

public class JoiningEmailSettings
{
    /// <summary>Keyed by the company's Id (the values of "MailSettings:Company").</summary>
    public Dictionary<string, MailSender> CompanySenders { get; set; } = new();
}

public class MailSender
{
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}

/// <summary>Checked at startup (ValidateOnStart), so a missing secret or sender stops the API instead of failing the
/// first mail. Never echoes the secret.</summary>
internal sealed class MailSettingsValidator : IValidateOptions<MailSettings>
{
    public ValidateOptionsResult Validate(string? name, MailSettings options)
    {
        var graph = options.MicrosoftGraph;
        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(graph.TenantId))
            failures.Add("MailSettings:MicrosoftGraph:TenantId is missing.");
        if (string.IsNullOrWhiteSpace(graph.ClientId))
            failures.Add("MailSettings:MicrosoftGraph:ClientId is missing.");
        if (string.IsNullOrWhiteSpace(graph.ClientSecret))
            failures.Add("MailSettings:MicrosoftGraph:ClientSecret is missing (set the MailSettings__MicrosoftGraph__ClientSecret environment variable).");

        var senders = graph.JoiningEmail.CompanySenders;
        if (senders.Count == 0)
            failures.Add("MailSettings:MicrosoftGraph:JoiningEmail:CompanySenders has no company mailbox configured.");

        foreach (var (companyId, sender) in senders)
        {
            if (!Guid.TryParse(companyId, out _))
                failures.Add($"CompanySenders key '{companyId}' is not a valid company Id.");
            if (!MailAddress.TryCreate(sender.Email, out _))
                failures.Add($"CompanySenders '{companyId}' has an invalid Email '{sender.Email}'.");
        }

        return failures.Count == 0 ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(failures);
    }
}
