using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using QubeFin.Core.Results;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Tracking;
using QubeFin.Tracking.Application.GeoTags.Queries;
using QubeFin.Tracking.Persistence.Repositories;

namespace QubeFin.Tracking.Application.GeoTags.Commands;

#region --- COMMAND ---
// Sent as multipart/form-data: the photo taken at the place and its GPS position
public class UpdateBcLocationRequest
{
    public IFormFile? Photo { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}

public enum BcPlace
{
    Group,
    Member
}

public record UpdateBcLocationCommand(Guid UserId, BcPlace Place, Guid Id, UpdateBcLocationRequest Request) : IRequest<Result<GeoTagResponse>>;
#endregion

#region --- VALIDATOR ---
public class UpdateBcLocationCommandValidator : AbstractValidator<UpdateBcLocationCommand>
{
    private const long MaxPhotoBytes = 5 * 1024 * 1024;

    public UpdateBcLocationCommandValidator()
    {
        RuleFor(v => v.UserId).NotEmpty().WithMessage("User Id is required.");
        RuleFor(v => v.Id).NotEmpty().WithMessage("Id is required.");
        RuleFor(v => v.Request.Photo).NotNull().WithMessage("Take a photo of the place.");
        RuleFor(v => v.Request.Photo!)
            .Must(f => f.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)).WithMessage("The photo must be an image.")
            .Must(f => f.Length > 0 && f.Length <= MaxPhotoBytes).WithMessage("The photo must be at most 5 MB.")
            .When(v => v.Request.Photo is not null);
        RuleFor(v => v.Request.Latitude).NotNull().InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.");
        RuleFor(v => v.Request.Longitude).NotNull().InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.");
    }
}
#endregion

#region --- HANDLER ---
// Sets a group's or member's location with the photo taken there, and records who did it (TaggedBy / TaggedOn).
// Allowed for whoever holds the group's mapped designation (the CO) and the Branch, Area or Divisional Manager above it.
internal sealed class UpdateBcLocationCommandHandler(
    ITrackingScopeRepository scopeRepository,
    IGeoTagRepository geoTagRepository,
    IFileStorageRepository fileStorage,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateBcLocationCommand, Result<GeoTagResponse>>
{
    public async Task<Result<GeoTagResponse>> Handle(UpdateBcLocationCommand command, CancellationToken cancellationToken)
    {
        var viewer = await scopeRepository.GetViewerAsync(command.UserId, cancellationToken);
        if (viewer is null) return new ForbiddenError("You do not have access to Tracking.");

        var target = await geoTagRepository.FindGeoTagAsync(command.Id, viewer.EmployeeId, cancellationToken);
        var isExpectedPlace = target is not null && (command.Place == BcPlace.Group
            ? target.Kind == "GroupCentre"
            : target.Kind is "Member" or "OdMember");
        if (!isExpectedPlace) return new RecordNotFoundError(command.Place == BcPlace.Group ? "Group not found" : "Member not found");
        if (!GeoTagRules.CanEdit(viewer, target!)) return new ForbiddenError("You cannot change this location.");

        var photo = command.Request.Photo!;
        string photoKey;
        await using (var stream = photo.OpenReadStream())
        {
            var folder = command.Place == BcPlace.Group ? "Tracking/Groups" : "Tracking/Members";
            photoKey = await fileStorage.UploadFileAsync(stream, folder, photo.FileName, photo.ContentType, cancellationToken);
        }

        var location = new GeoPoint(command.Request.Latitude!.Value, command.Request.Longitude!.Value);
        await geoTagRepository.UpdateLocationAsync(target!, location, photoKey, viewer.UserId, DateTime.Now, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var updated = await GeoTagRules.ReadBackAsync(scopeRepository, geoTagRepository, target!, cancellationToken);
        return Result.Ok(GeoTagResponse.From(updated, fileStorage));
    }
}
#endregion
