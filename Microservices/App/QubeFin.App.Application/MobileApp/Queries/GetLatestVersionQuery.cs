using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.App.Application.MobileApp.Models;
using QubeFin.Core.Results;
using QubeFin.Persistence;

namespace QubeFin.App.Application.MobileApp.Queries;

#region --- QUERY ---
public record GetLatestVersionQuery(string version) : IRequest<Result<MobileAppVersion>>;
#endregion

#region --- VALIDATOR ---
public class GetLatestVersionQueryValidator : AbstractValidator<GetLatestVersionQuery>
{
    public GetLatestVersionQueryValidator()
    {
        RuleFor(v => v.version).NotEmpty().WithMessage("Version is required.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class GetLatestVersionQueryHandler(QubeFinDataContext context) : IRequestHandler<GetLatestVersionQuery, Result<MobileAppVersion>>
{
    public async Task<Result<MobileAppVersion>> Handle(GetLatestVersionQuery request, CancellationToken cancellationToken)
    {
        var mobileAppVersion = await context.TblMobileAppVersions.AsNoTracking().FirstOrDefaultAsync(v => v.Version.Trim() == request.version.Trim(), cancellationToken: cancellationToken);
        if (mobileAppVersion is null)
        {
            return new RecordNotFoundError($"Mobile app version not found for the given version");
        }
        var latestMobileAppVersion = await context.TblMobileAppVersions.AsNoTracking().Where(v => v.IsCurrentVersion).FirstOrDefaultAsync(cancellationToken: cancellationToken);
        return new MobileAppVersion
        {
            LatestVersion = mobileAppVersion.IsCurrentVersion ? mobileAppVersion.Version : latestMobileAppVersion.Version,
            IsDiscontinued = mobileAppVersion.IsCurrentVersion ? false : true,
            DownloadUrl = mobileAppVersion.IsCurrentVersion ? null : latestMobileAppVersion.AppUrl
        };
    }
}
#endregion