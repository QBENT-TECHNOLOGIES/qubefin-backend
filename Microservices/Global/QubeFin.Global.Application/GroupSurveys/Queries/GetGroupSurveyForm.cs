using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Global.Application.GroupSurveys.Models;
using QubeFin.Global.Application.SurveyUnit;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.GroupSurveys.Queries;

#region --- QUERY ---
public record GetGroupSurveyFormQuery(Guid SurveyId, Guid UserId, Guid EmployeeId) : IRequest<Result<GroupSurveyResponse>>;
#endregion

#region --- VALIDATOR ---
public class GetGroupSurveyFormQueryValidator : AbstractValidator<GetGroupSurveyFormQuery>
{
    public GetGroupSurveyFormQueryValidator()
    {
        RuleFor(v => v.SurveyId).NotEmpty().WithMessage("Survey Id is required.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class GetGroupSurveyFormQueryHandler(
    QubeFinDataContext context,
    IGroupSurveyRepository groupSurveyRepository,
    IFileStorageRepository fileStorageRepository) : IRequestHandler<GetGroupSurveyFormQuery, Result<GroupSurveyResponse>>
{
    public async Task<Result<GroupSurveyResponse>> Handle(GetGroupSurveyFormQuery request, CancellationToken cancellationToken)
    {
        var survey = await context.TblSurveys
            .Include(s => s.TblSurveyAssigneds)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.SurveyId, cancellationToken);

        if (survey is null)
            return new RecordNotFoundError("Survey not found.");

        if (survey.SurveyType != SurveyTypes.Group)
            return new ValidationError("This survey does not have a group survey form.");

        if (!await SurveyAccess.CanViewAsync(context, survey, request.UserId, request.EmployeeId, cancellationToken))
            return new ForbiddenError("You do not have access to this survey.");

        var groupSurvey = await context.TblGroupSurveys.AsNoTracking().FirstOrDefaultAsync(g => g.SurveyId == survey.Id, cancellationToken);
        var isLeadCreditOfficer = survey.TblSurveyAssigneds.Any(a => a.EmployeeId == request.EmployeeId && a.IsLead);
        var isEditableStatus = survey.Status == SurveyStatuses.Assigned || survey.Status == SurveyStatuses.Draft;

        var response = new GroupSurveyResponse
        {
            SurveyId = survey.Id,
            Status = survey.Status,
            CanEditForm = isLeadCreditOfficer && isEditableStatus,
            IsSaved = groupSurvey != null,
            LastSavedOn = groupSurvey?.LastModifiedOn ?? groupSurvey?.CreatedOn,
            Form = groupSurvey != null
                ? groupSurvey.ToForm(survey.ProposedArea)
                : await CreateEmptyFormAsync(survey)
        };

        // Distance from the branch: worked out from the saved location and the branch location (not typed).
        var branchPoint = survey.BranchOrganizationUnitId.HasValue
            ? await groupSurveyRepository.GetBranchPointAsync(survey.BranchOrganizationUnitId.Value)
            : null;
        response.BranchHasLocation = branchPoint?.Latitude != null && branchPoint?.Longitude != null;
        response.Form.Market.DistanceFromBranchKm = GroupSurveyDistance.Between(groupSurvey?.Location, branchPoint?.Latitude, branchPoint?.Longitude);

        // The "Surveyed By" block is filled once the Lead Credit Officer has submitted the form.
        if (groupSurvey is { IsSurveyorSubmit: true })
        {
            var leadEmployeeId = survey.TblSurveyAssigneds.FirstOrDefault(a => a.IsLead)?.EmployeeId;
            if (leadEmployeeId.HasValue)
                response.SurveyedBy = await BuildSignOffAsync(leadEmployeeId.Value, groupSurvey.SurveyorSubmitOn);
        }

        // "Recommended By": the Head of Branch, once he has recommended.
        if (groupSurvey is { BranchManagerEmployeeId: not null, BranchManagerActionOn: not null })
            response.RecommendedBy = await BuildSignOffAsync(groupSurvey.BranchManagerEmployeeId.Value, groupSurvey.BranchManagerActionOn);

        // "Approved By": the Area Manager, once he has approved.
        if (survey.Status == SurveyStatuses.Approved)
        {
            var approval = await context.TblSurveyApprovals.AsNoTracking()
                .Where(a => a.SurveyId == survey.Id && a.Action == ApprovalActions.Approved)
                .OrderByDescending(a => a.ActionOn)
                .FirstOrDefaultAsync(cancellationToken);
            if (approval is not null)
                response.ApprovedBy = await BuildSignOffAsync(approval.AuthorityEmployeeId, approval.ActionOn);
        }

        return response;
    }

    // Before the first save, the form starts with what the Branch Head entered in the assignment.
    private async Task<GroupSurveyForm> CreateEmptyFormAsync(TblSurvey survey)
    {
        var places = await groupSurveyRepository.GetPlaceNamesAsync(survey.AdministrativeUnitId);
        return new GroupSurveyForm
        {
            SurveyDate = DateOnly.FromDateTime(DateTime.Today),
            Geographic = new GroupGeographicSection
            {
                ProposedOperationalArea = survey.ProposedArea,
                VillageLocality = places.VillageLocality,
                GramPanchayatMunicipality = places.GramPanchayatMunicipality,
                Block = places.Block
            }
        };
    }

    private async Task<SignOffResponse?> BuildSignOffAsync(Guid employeeId, DateTime? date)
    {
        var info = await groupSurveyRepository.GetSignOffAsync(employeeId);
        if (info is null) return null;

        // A problem with the file storage must not stop the form from opening.
        string? signatureUrl = null;
        if (!string.IsNullOrEmpty(info.SignatureFileNo))
        {
            try { signatureUrl = await fileStorageRepository.GetRootFileUrlAsync(info.SignatureFileNo); }
            catch { signatureUrl = null; }
        }

        return new SignOffResponse
        {
            Name = info.EmployeeName,
            Designation = info.Designation,
            Pin = info.Pin,
            Date = date,
            SignatureUrl = signatureUrl
        };
    }
}
#endregion
