using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Tracking;
using QubeFin.Tracking.Application.GeoTags.Queries;
using QubeFin.Tracking.Persistence.Repositories;

namespace QubeFin.Tracking.Application.GeoTags.Commands;

#region --- COMMAND ---
// radiusMeters is checked but not stored (radii come from Tbl_TrackingSetting); the reason is required but there is no table to keep it yet
public record UpdateGeoTagRequest(GeoPoint? Location, int? RadiusMeters, string? Reason);

public record UpdateGeoTagCommand(Guid UserId, Guid Id, UpdateGeoTagRequest Request) : IRequest<Result<GeoTagResponse>>;
#endregion

#region --- VALIDATOR ---
public class UpdateGeoTagCommandValidator : AbstractValidator<UpdateGeoTagCommand>
{
    public UpdateGeoTagCommandValidator()
    {
        RuleFor(v => v.UserId).NotEmpty().WithMessage("User Id is required.");
        RuleFor(v => v.Id).NotEmpty().WithMessage("Geo-tag Id is required.");
        RuleFor(v => v.Request.Location).NotNull().WithMessage("Pick the location on the map.");
        RuleFor(v => v.Request.Location!.Lat).InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.")
            .When(v => v.Request.Location is not null);
        RuleFor(v => v.Request.Location!.Lng).InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.")
            .When(v => v.Request.Location is not null);
        RuleFor(v => v.Request.RadiusMeters).NotNull().GreaterThan(0).WithMessage("The radius must be greater than 0.");
        RuleFor(v => v.Request.Reason).NotEmpty().WithMessage("A reason is required when changing a location.");
    }
}
#endregion

#region --- HANDLER ---
// Branches and employee homes. Groups and members have their own APIs (location + photo): groups/{id}/location and members/{id}/location.
internal sealed class UpdateGeoTagCommandHandler(
    ITrackingScopeRepository scopeRepository,
    IGeoTagRepository geoTagRepository,
    IFileStorageRepository fileStorage,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateGeoTagCommand, Result<GeoTagResponse>>
{
    public async Task<Result<GeoTagResponse>> Handle(UpdateGeoTagCommand command, CancellationToken cancellationToken)
    {
        var viewer = await scopeRepository.GetViewerAsync(command.UserId, cancellationToken);
        if (viewer is null) return new ForbiddenError("You do not have access to Tracking.");

        var target = await geoTagRepository.FindGeoTagAsync(command.Id, viewer.EmployeeId, cancellationToken);
        if (target is null) return new RecordNotFoundError("Geo-tag not found");
        if (GeoTagRules.IsGroupOrMember(target))
        {
            return new ValidationError("Group and member locations are updated with their photo through groups/{id}/location and members/{id}/location.");
        }
        if (!GeoTagRules.CanEdit(viewer, target)) return new ForbiddenError("You cannot change this location.");

        await geoTagRepository.UpdateLocationAsync(target, command.Request.Location!, null, viewer.UserId, DateTime.Now, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(GeoTagResponse.From(await GeoTagRules.ReadBackAsync(scopeRepository, geoTagRepository, target, cancellationToken), fileStorage));
    }
}
#endregion
