using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using QubeFin.Hrms.Application.InterviewProcess.Services;
using System.Reflection;

namespace QubeFin.Hrms.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            config.AddOpenBehavior(typeof(CandidateWorkflowGuardBehavior<,>));
        });
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), includeInternalTypes: true);
        services.AddMemoryCache();

        // All HRMS mail goes through Microsoft Graph from the candidate's company HR mailbox. Settings are checked at
        // startup so a missing secret or sender stops the API rather than failing the first mail.
        services.AddOptions<MailSettings>()
            .Bind(configuration.GetSection("MailSettings"))
            .ValidateOnStart();
        services.AddSingleton<IValidateOptions<MailSettings>, MailSettingsValidator>();
        services.AddSingleton(sp => HrMailSender.CreateGraphClient(sp.GetRequiredService<IOptions<MailSettings>>().Value.MicrosoftGraph));
        services.AddScoped<IHrMailSender, HrMailSender>();
        services.AddScoped<ICandidateLetterMailer, CandidateLetterMailer>();
        services.AddScoped<IPanelInvitationMailer, PanelInvitationMailer>();

        return services;
    }
}