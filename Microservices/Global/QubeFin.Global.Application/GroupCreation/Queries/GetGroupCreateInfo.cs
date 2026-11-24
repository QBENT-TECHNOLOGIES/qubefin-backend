using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Global.Application.Cgt;
using QubeFin.Global.Application.Grt;
using QubeFin.Global.Application.Mobilization;
using QubeFin.Global.Application.SurveyUnit;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.GroupCreation.Queries;

#region --- QUERY ---
public record GetGroupCreateInfoQuery(Guid SurveyId, Guid UserId, Guid EmployeeId) : IRequest<Result<GroupCreateInfoResponse>>;
#endregion

#region --- VALIDATOR ---
public class GetGroupCreateInfoQueryValidator : AbstractValidator<GetGroupCreateInfoQuery>
{
    public GetGroupCreateInfoQueryValidator()
    {
        RuleFor(v => v.SurveyId).NotEmpty().WithMessage("Survey Id is required.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class GetGroupCreateInfoQueryHandler(
    QubeFinDataContext context,
    IGroupRepository groupRepository,
    IGroupSurveyRepository groupSurveyRepository) : IRequestHandler<GetGroupCreateInfoQuery, Result<GroupCreateInfoResponse>>
{
    public async Task<Result<GroupCreateInfoResponse>> Handle(GetGroupCreateInfoQuery request, CancellationToken cancellationToken)
    {
        var survey = await context.TblSurveys.AsNoTracking()
            .Include(s => s.TblSurveyAssigneds)
            .Include(s => s.AdministrativeUnit)
            .FirstOrDefaultAsync(s => s.Id == request.SurveyId, cancellationToken);
        if (survey is null)
            return new RecordNotFoundError("Survey not found.");
        if (survey.SurveyType != SurveyTypes.Group)
            return new ValidationError("Group Creation is only for Group Surveys.");
        if (!await SurveyAccess.CanViewAsync(context, survey, request.UserId, request.EmployeeId, cancellationToken))
            return new ForbiddenError("You do not have access to this survey.");

        var response = new GroupCreateInfoResponse
        {
            SurveyId = survey.Id,
            ProposedArea = survey.ProposedArea,
            AdministrativeUnitId = survey.AdministrativeUnitId,
            AreaName = survey.AdministrativeUnit?.Name,
            BranchName = survey.BranchOrganizationUnitId.HasValue
                ? await context.TblOrganizationUnits.AsNoTracking()
                    .Where(o => o.Id == survey.BranchOrganizationUnitId).Select(o => o.Name).FirstOrDefaultAsync(cancellationToken)
                : null
        };

        // A group already made from this survey.
        var group = await context.TblGroups.AsNoTracking()
            .Where(g => g.SurveyId == survey.Id)
            .Select(g => new { g.Id, g.Name, g.Code })
            .FirstOrDefaultAsync(cancellationToken);
        if (group is not null)
        {
            response.GroupId = group.Id;
            response.GroupName = group.Name;
            response.GroupCode = group.Code;
            response.Reason = "The group is already created.";
            return Result.Ok(response);
        }

        // The group is created only after the GRT is submitted as Pass.
        var grt = await context.TblGroupGrts.AsNoTracking()
            .FirstOrDefaultAsync(g => g.SurveyId == survey.Id, cancellationToken);
        var cgtCompleted = await context.TblGroupCgts.AsNoTracking()
            .AnyAsync(c => c.SurveyId == survey.Id && c.Status == CgtStatuses.Completed, cancellationToken);
        var grtPassed = grt is { Status: GrtStatuses.Completed, OverallRecommendation: "Pass" };
        response.CgtStatus = cgtCompleted ? "Complete" : "Pending";
        response.GrtStatus = grt is { Status: GrtStatuses.Completed } ? "Complete" : "Pending";

        if (survey.Status != SurveyStatuses.Approved)
            response.Reason = "The survey is not approved.";
        else if (grt is null || grt.Status != GrtStatuses.Completed)
            response.Reason = "The group can be created after the GRT is submitted.";
        else if (!grtPassed)
            response.Reason = "The GRT is failed. The process stops here.";

        // The Responsible Credit Officer is the survey's Credit Officer who creates the group.
        var post = await groupRepository.GetCreditOfficerPostOfEmployeeAsync(request.EmployeeId);
        var isAssigned = survey.TblSurveyAssigneds.Any(a => a.EmployeeId == request.EmployeeId);
        response.CanCreate = response.Reason is null
            && isAssigned
            && post is not null
            && post.BranchId == survey.BranchOrganizationUnitId;

        if (response.Reason is null && !response.CanCreate)
            response.Reason = "Only the Credit Officers of the survey can create the group.";

        // Details shown or prefilled in the window.
        if (grtPassed && grt is not null)
        {
            var conductor = await groupSurveyRepository.GetSignOffAsync(grt.ConductedByEmployeeId);
            if (conductor is not null)
                response.ConductedBy = new CreateInfoPerson { Name = conductor.EmployeeName, Designation = conductor.Designation, Code = conductor.Pin };
        }

        if (response.CanCreate && post is not null)
        {
            var me = await groupSurveyRepository.GetSignOffAsync(request.EmployeeId);
            response.CreditOfficer = new CreateInfoPerson { Name = me?.EmployeeName ?? post.EmployeeName, Designation = me?.Designation, Code = me?.Pin };

            // Date of joining = the start of the current designation.
            var from = await context.TblEmployeeDesignations.AsNoTracking()
                .Where(ed => ed.EmployeeId == request.EmployeeId && ed.DesignationId == post.DesignationId && ed.EffectiveTo == null)
                .Select(ed => (DateTime?)ed.EffectiveFrom)
                .FirstOrDefaultAsync(cancellationToken);
            response.CreditOfficerJoinedOn = from.HasValue ? DateOnly.FromDateTime(from.Value) : null;

            var groupSurvey = await context.TblGroupSurveys.AsNoTracking()
                .Where(g => g.SurveyId == survey.Id)
                .Select(g => new { g.Location, g.NearestLandmark })
                .FirstOrDefaultAsync(cancellationToken);
            if (groupSurvey?.Location is not null)
            {
                response.Latitude = (decimal)groupSurvey.Location.Coordinate.Y;
                response.Longitude = (decimal)groupSurvey.Location.Coordinate.X;
            }
            response.LandMark = groupSurvey?.NearestLandmark;

            // Only the Pending borrowers (not Blocked ones, not those who already joined a group) can be chosen.
            response.Borrowers = await context.TblGroupBorrowers.AsNoTracking()
                .Where(b => b.SurveyId == survey.Id && b.Status == BorrowerStatuses.Pending && b.GroupId == null)
                .OrderBy(b => b.CreatedOn)
                .Select(b => new CreateInfoBorrower
                {
                    Id = b.Id,
                    Name = b.Name,
                    GuardianName = b.GuardianName,
                    MobileNo = b.MobileNo,
                    Age = b.Age,
                    MaritalStatus = b.MaritalStatus
                })
                .ToListAsync(cancellationToken);
        }

        return Result.Ok(response);
    }
}
#endregion
