using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Global.Application.Groups.Models;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Global.Application.Groups.Queries;

#region --- QUERY ---
public record GetGroupByIdQuery(Guid Id, Guid UserId, Guid EmployeeId) : IRequest<Result<GroupDetailResponse>>;
#endregion

#region --- VALIDATOR ---
public class GetGroupByIdQueryValidator : AbstractValidator<GetGroupByIdQuery>
{
    public GetGroupByIdQueryValidator()
    {
        RuleFor(v => v.Id).NotEmpty().WithMessage("Group Id is required.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class GetGroupByIdQueryHandler(
    QubeFinDataContext context,
    ISurveyRepository surveyRepository,
    IGroupRepository groupRepository) : IRequestHandler<GetGroupByIdQuery, Result<GroupDetailResponse>>
{
    public async Task<Result<GroupDetailResponse>> Handle(GetGroupByIdQuery request, CancellationToken cancellationToken)
    {
        var group = await context.TblGroups.AsNoTracking()
            .Include(g => g.AdministrativeUnit)
            .FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken);
        if (group is null)
            return new RecordNotFoundError("Group not found.");

        var rights = await GroupAccess.GetRightsAsync(context, surveyRepository, groupRepository, group, request.UserId, request.EmployeeId, cancellationToken);
        if (!rights.CanView)
            return new ForbiddenError("You do not have access to this group.");

        var response = new GroupDetailResponse
        {
            Id = group.Id,
            Code = group.Code ?? string.Empty,
            SurveyId = group.SurveyId,
            BranchId = rights.Branch?.Id,
            BranchName = rights.Branch?.Name,
            DesignationId = group.DesignationId,
            Name = group.Name,
            CollectionDay = group.CollectionDay,
            CollectionTime = group.CollectionTime,
            BiweeklyStartDate = group.BiweeklyStartDate,
            FormationDate = group.FormationDate,
            MobileNo = group.MobileNo,
            AdministrativeUnitId = group.AdministrativeUnitId,
            AreaName = group.AdministrativeUnit?.Name,
            PinCode = group.PinCode?.Trim(),
            Address = group.Address,
            Latitude = group.Location != null ? (decimal)group.Location.Coordinate.Y : null,
            Longitude = group.Location != null ? (decimal)group.Location.Coordinate.X : null,
            LandMark = group.LandMark,
            Status = (group.CurrentStatus ?? string.Empty).Trim(),
            VerifiedOn = group.VerifiedOn,
            VerifiedRemarks = group.VerifiedRemarks,
            CreatedOn = group.CreatedOn,
            CanEdit = rights.CanEdit,
            CanDecide = rights.CanDecide
        };

        if (group.SurveyId.HasValue)
            response.SurveyAssignmentNo = await context.TblSurveys.AsNoTracking()
                .Where(s => s.Id == group.SurveyId).Select(s => s.AssignmentNo).FirstOrDefaultAsync(cancellationToken);

        if (group.DesignationId.HasValue)
            response.CreditOfficerName = await context.TblEmployeeDesignations.AsNoTracking()
                .Where(ed => ed.DesignationId == group.DesignationId && ed.EffectiveTo == null)
                .Select(ed => ed.Employee.FullName).FirstOrDefaultAsync(cancellationToken);

        response.CreatedByName = await context.TblUsers.AsNoTracking()
            .Where(u => u.Id == group.CreatedBy)
            .Select(u => u.Employee.FullName).FirstOrDefaultAsync(cancellationToken);

        if (group.VerifiedBy.HasValue)
            response.VerifiedByName = await context.TblUsers.AsNoTracking()
                .Where(u => u.Id == group.VerifiedBy)
                .Select(u => u.Employee.FullName).FirstOrDefaultAsync(cancellationToken);

        return response;
    }
}
#endregion
