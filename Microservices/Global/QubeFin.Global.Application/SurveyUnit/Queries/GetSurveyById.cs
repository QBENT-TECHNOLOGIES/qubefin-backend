using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Global.Application.Grt;
using QubeFin.Global.Application.SurveyUnit.Models;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.SurveyUnit.Queries;

#region --- QUERY ---
public record GetSurveyByIdQuery(Guid Id, Guid EmployeeId, Guid UserId) : IRequest<Result<SurveyResponse>>;
#endregion

#region --- VALIDATOR ---
public class GetSurveyByIdQueryValidator : AbstractValidator<GetSurveyByIdQuery>
{
    public GetSurveyByIdQueryValidator()
    {
        RuleFor(v => v.Id).NotEmpty().WithMessage("Survey Id is required.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class GetSurveyByIdQueryHandler(QubeFinDataContext context, ISurveyRepository surveyRepository) : IRequestHandler<GetSurveyByIdQuery, Result<SurveyResponse>>
{
    public async Task<Result<SurveyResponse>> Handle(GetSurveyByIdQuery request, CancellationToken cancellationToken)
    {
        var surveyEntity = await context.TblSurveys.Include(m => m.TblSurveyAssigneds).ThenInclude(m => m.Employee).
                AsNoTracking().
                FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken: cancellationToken);
        if (surveyEntity is null)
        {
            return new RecordNotFoundError($"Survey not found for the given Id");
        }
        var users = await context.TblUsers.Where(u => u.Id == surveyEntity.CreatedBy || u.Id == surveyEntity.LastModifiedBy).AsNoTracking().ToListAsync(cancellationToken);

        // The assignment can be edited by the Committee Lead who created it, until the Lead Surveyor submits.
        var isEditableStatus = surveyEntity.Status == SurveyStatuses.Assigned || surveyEntity.Status == SurveyStatuses.Draft;
        bool canCreatorEdit;
        string? branchName = null;
        if (surveyEntity.SurveyType == SurveyTypes.Group)
        {
            // A Group Survey is edited by the Head of its branch (who created it).
            canCreatorEdit = surveyEntity.BranchOrganizationUnitId.HasValue
                && await surveyRepository.IsHeadOfBranchAsync(request.EmployeeId, surveyEntity.BranchOrganizationUnitId.Value);
            branchName = surveyEntity.BranchOrganizationUnitId.HasValue
                ? await context.TblOrganizationUnits.AsNoTracking()
                    .Where(o => o.Id == surveyEntity.BranchOrganizationUnitId).Select(o => o.Name).FirstOrDefaultAsync(cancellationToken)
                : null;
        }
        else
        {
            canCreatorEdit = await context.TblSurveyCommittees
                .AnyAsync(c => c.EmployeeId == request.EmployeeId && c.IsActive && c.IsLead, cancellationToken);
        }
        var canEditGeneral = isEditableStatus && canCreatorEdit && surveyEntity.CreatedBy == request.UserId;

        // The survey form is filled in by the Lead Surveyor (Branch Survey) or the Lead Credit Officer (Group Survey).
        var isLeadSurveyor = surveyEntity.TblSurveyAssigneds.Any(s => s.EmployeeId == request.EmployeeId && s.IsLead);
        var canEditForm = isEditableStatus && isLeadSurveyor;

        // Group Survey: once Approved, a group is created from it (Group Master). Show the group, or the right to create it.
        Guid? groupId = null;
        string? groupName = null, groupCode = null;
        var canCreateGroup = false;
        var groupStep = 0;
        var groupStopped = false;
        if (surveyEntity.SurveyType == SurveyTypes.Group)
        {
            var group = await context.TblGroups.AsNoTracking()
                .Where(g => g.SurveyId == surveyEntity.Id)
                .Select(g => new { g.Id, g.Name, g.Code })
                .FirstOrDefaultAsync(cancellationToken);
            groupId = group?.Id;
            groupName = group?.Name;
            groupCode = group?.Code;

            // Only the Credit Officers of the survey create the group (not the Head of the branch),
            // after the GRT was submitted as Pass.
            canCreateGroup = group is null
                && surveyEntity.Status == SurveyStatuses.Approved
                && surveyEntity.TblSurveyAssigneds.Any(s => s.EmployeeId == request.EmployeeId)
                && await context.TblGroupGrts.AsNoTracking().AnyAsync(g => g.SurveyId == surveyEntity.Id
                    && g.Status == GrtStatuses.Completed && g.OverallRecommendation == "Pass", cancellationToken);

            // The 8 steps of a Group Survey: Assigned, Draft, Recommendation, Approval, Mobilization, CGT, GRT, Group Created.
            var cgtStatus = await context.TblGroupCgts.AsNoTracking().Where(c => c.SurveyId == surveyEntity.Id).Select(c => c.Status).FirstOrDefaultAsync(cancellationToken);
            var grt = await context.TblGroupGrts.AsNoTracking().Where(g => g.SurveyId == surveyEntity.Id)
                .Select(g => new { g.Status, g.OverallRecommendation }).FirstOrDefaultAsync(cancellationToken);
            var grtDone = grt is { Status: "Completed" };
            groupStopped = grtDone && grt!.OverallRecommendation == "Fail";

            groupStep = surveyEntity.Status switch
            {
                SurveyStatuses.Assigned => 0,
                SurveyStatuses.Draft => 1,
                SurveyStatuses.PendingRecommendation => 2,
                SurveyStatuses.PendingApproval => 3,
                SurveyStatuses.Approved when groupId.HasValue => 8,
                SurveyStatuses.Approved when grtDone && !groupStopped => 7,
                SurveyStatuses.Approved when cgtStatus == "Completed" => 6,
                SurveyStatuses.Approved when cgtStatus != null => 5,
                SurveyStatuses.Approved => 4,
                _ => 0
            };
        }

        // The location of the survey form (Branch Survey or Group Survey), for the map on the view page.
        NetTopologySuite.Geometries.Geometry? formLocation = surveyEntity.SurveyType == SurveyTypes.Group
            ? await context.TblGroupSurveys.AsNoTracking().Where(g => g.SurveyId == surveyEntity.Id).Select(g => g.Location).FirstOrDefaultAsync(cancellationToken)
            : await context.TblBranchSurveys.AsNoTracking().Where(b => b.SurveyId == surveyEntity.Id).Select(b => b.Location).FirstOrDefaultAsync(cancellationToken);

        var companyName = surveyEntity.CompanyId.HasValue
            ? await context.TblCompanies.AsNoTracking().Where(c => c.Id == surveyEntity.CompanyId).Select(c => c.Name).FirstOrDefaultAsync(cancellationToken)
            : null;

        return new SurveyResponse
        {
            CompanyId = surveyEntity.CompanyId,
            CompanyName = companyName,
            GroupStep = groupStep,
            GroupStopped = groupStopped,
            Latitude = formLocation != null ? (decimal)formLocation.Coordinate.Y : null,
            Longitude = formLocation != null ? (decimal)formLocation.Coordinate.X : null,
            GroupId = groupId,
            GroupName = groupName,
            GroupCode = groupCode,
            CanCreateGroup = canCreateGroup,
            Id = surveyEntity.Id,
            SurveyType = surveyEntity.SurveyType,
            AssignmentNo = surveyEntity.AssignmentNo,
            AssignmentDate = surveyEntity.AssignmentDate,
            ProposedArea = surveyEntity.ProposedArea,
            AdministrativeUnitId = surveyEntity.AdministrativeUnitId,
            TentativeSubmissionDate = surveyEntity.TentativeSubmissionDate,
            Status = surveyEntity.Status,
            SurveyFormAttached = surveyEntity.SurveyFormAttached,
            BranchOrganizationUnitId = surveyEntity.BranchOrganizationUnitId,
            BranchName = branchName,
            PreviousSurveyId = surveyEntity.PreviousSurveyId,
            IsSurveyAccessed = surveyEntity.TblSurveyAssigneds.Any(s => s.EmployeeId == request.EmployeeId),
            CanEditGeneral = canEditGeneral,
            CanEditForm = canEditForm,
            AuditInfo = new AuditInfo
            {
                CreatedBy = users.FirstOrDefault(u => u.Id == surveyEntity.CreatedBy)?.UserName ?? string.Empty,
                CreatedOn = surveyEntity.CreatedOn,
                LastModifiedBy = users.FirstOrDefault(u => u.Id == surveyEntity.LastModifiedBy)?.UserName ?? string.Empty,
                LastModifiedOn = surveyEntity.LastModifiedOn
            },
            SurveyAssigneds = surveyEntity.TblSurveyAssigneds.OrderBy(s => !s.IsLead).ThenBy(s => s.Employee.FullName).Select(s => new SurveyAssignedResponse
            {
                Id = s.Id,
                SurveyId = s.SurveyId,
                EmployeeId = s.EmployeeId,
                EmployeeName = s.Employee.FullName + " (" + s.Employee.Code + ")",
                IsLead = s.IsLead,
                AssignedDate = s.AssignedDate,
                AssignedBy = s.AssignedBy
            }).ToList()
        };
    }
}
#endregion
