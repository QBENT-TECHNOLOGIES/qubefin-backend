using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
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
        if (surveyEntity.SurveyType == SurveyTypes.Group)
        {
            var group = await context.TblGroups.AsNoTracking()
                .Where(g => g.SurveyId == surveyEntity.Id)
                .Select(g => new { g.Id, g.Name, g.Code })
                .FirstOrDefaultAsync(cancellationToken);
            groupId = group?.Id;
            groupName = group?.Name;
            groupCode = group?.Code;

            // Only the Credit Officers of the survey create the group (not the Head of the branch).
            canCreateGroup = group is null
                && surveyEntity.Status == SurveyStatuses.Approved
                && surveyEntity.TblSurveyAssigneds.Any(s => s.EmployeeId == request.EmployeeId);
        }

        return new SurveyResponse
        {
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
