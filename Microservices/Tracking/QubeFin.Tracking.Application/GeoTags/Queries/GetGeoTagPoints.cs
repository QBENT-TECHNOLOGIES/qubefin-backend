using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Persistence.Models.Tracking;
using QubeFin.Tracking.Application.Enums;
using QubeFin.Tracking.Persistence.Repositories;

namespace QubeFin.Tracking.Application.GeoTags.Queries;

#region --- QUERY ---
public record GetGeoTagPointsQuery(Guid UserId, Guid OrganisationUnitId, string? Kinds) : IRequest<Result<GeoTagPointsResponse>>;
#endregion

#region --- VALIDATOR ---
public class GetGeoTagPointsQueryValidator : AbstractValidator<GetGeoTagPointsQuery>
{
    public GetGeoTagPointsQueryValidator()
    {
        RuleFor(v => v.UserId).NotEmpty().WithMessage("User Id is required.");
        RuleFor(v => v.OrganisationUnitId).NotEmpty().WithMessage("Select a branch.");
        RuleFor(v => v.Kinds)
            .NotEmpty().WithMessage("Select at least one kind of place.");
        RuleFor(v => v.Kinds)
            .Must(k => k!.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).All(v => Enum.TryParse<GeoTagKind>(v, false, out _)))
            .When(v => !string.IsNullOrWhiteSpace(v.Kinds))
            .WithMessage("Kinds must be Branch, GroupCentre, Member, OdMember or EmployeeHome.");
    }
}
#endregion

#region --- RESPONSE ---
public record GeoTagPointResponse(
    Guid Id,
    GeoTagKind Kind,
    Guid RefId,
    string Name,
    string RefCode,
    Guid OrganisationUnitId,
    string BranchName,
    string? Address,
    GeoPoint? Location,
    int? TaskLocationMatchInMeters,
    bool Verified,
    Guid? CreditOfficerId,
    string? CreditOfficerName,
    Guid? GroupId,
    string? GroupName,
    int? Dpd,
    decimal? OverdueAmount,
    string? TaggedByName,
    DateTimeOffset? TaggedAt);

public record GeoTagPointsResponse(List<GeoTagPointResponse> Points, Dictionary<string, int> UntaggedByKind);
#endregion

#region --- HANDLER ---
internal sealed class GetGeoTagPointsQueryHandler(ITrackingScopeRepository scopeRepository, IGeoTagRepository geoTagRepository) : IRequestHandler<GetGeoTagPointsQuery, Result<GeoTagPointsResponse>>
{
    public async Task<Result<GeoTagPointsResponse>> Handle(GetGeoTagPointsQuery request, CancellationToken cancellationToken)
    {
        var viewer = await scopeRepository.GetViewerAsync(request.UserId, cancellationToken);
        if (viewer is null) return new ForbiddenError("You do not have access to Tracking.");
        if (!viewer.BranchIds.Contains(request.OrganisationUnitId)) return new RecordNotFoundError("Branch not found");

        var kinds = request.Kinds!
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToHashSet();
        var setting = await scopeRepository.GetTrackingSettingAsync(cancellationToken);

        var result = await geoTagRepository.GetPointsAsync(request.OrganisationUnitId, kinds, setting?.TaskLocationMatchInMeters, cancellationToken);

        return Result.Ok(new GeoTagPointsResponse(
            result.Points.Select(p => new GeoTagPointResponse(
                p.Id,
                Enum.Parse<GeoTagKind>(p.Kind),
                p.RefId,
                p.Name,
                p.RefCode,
                p.OrganisationUnitId,
                p.BranchName,
                p.Address,
                p.Location,
                p.TaskLocationMatchInMeters,
                p.Verified,
                p.CreditOfficerId,
                p.CreditOfficerName,
                p.GroupId,
                p.GroupName,
                p.Dpd,
                p.OverdueAmount,
                p.TaggedByName,
                TrackingClock.WithIndiaOffset(p.TaggedAt))).ToList(),
            result.UntaggedByKind));
    }
}
#endregion
