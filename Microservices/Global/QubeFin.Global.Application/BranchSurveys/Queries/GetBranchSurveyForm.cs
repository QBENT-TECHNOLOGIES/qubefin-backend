using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Global.Application.BranchSurveys.Models;
using QubeFin.Global.Application.SurveyUnit;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.BranchSurveys.Queries;

#region --- QUERY ---
public record GetBranchSurveyFormQuery(Guid SurveyId, Guid UserId, Guid EmployeeId) : IRequest<Result<BranchSurveyResponse>>;
#endregion

#region --- VALIDATOR ---
public class GetBranchSurveyFormQueryValidator : AbstractValidator<GetBranchSurveyFormQuery>
{
    public GetBranchSurveyFormQueryValidator()
    {
        RuleFor(v => v.SurveyId).NotEmpty().WithMessage("Survey Id is required.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class GetBranchSurveyFormQueryHandler(QubeFinDataContext context) : IRequestHandler<GetBranchSurveyFormQuery, Result<BranchSurveyResponse>>
{
    public async Task<Result<BranchSurveyResponse>> Handle(GetBranchSurveyFormQuery request, CancellationToken cancellationToken)
    {
        var survey = await context.TblSurveys
            .Include(s => s.TblSurveyAssigneds)
            .Include(s => s.TblBranchSurvey!).ThenInclude(b => b.TblBranchSurveyCompetitors)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.SurveyId, cancellationToken);

        if (survey is null)
            return new RecordNotFoundError("Survey not found.");

        if (survey.SurveyType != SurveyTypes.Branch)
            return new ValidationError("This survey does not have a branch survey form.");

        if (!await SurveyAccess.CanViewAsync(context, survey, request.UserId, request.EmployeeId, cancellationToken))
            return new ForbiddenError("You do not have access to this survey.");

        var branchSurvey = survey.TblBranchSurvey;
        var isLeadSurveyor = survey.TblSurveyAssigneds.Any(a => a.EmployeeId == request.EmployeeId && a.IsLead);
        var isEditableStatus = survey.Status == SurveyStatuses.Assigned || survey.Status == SurveyStatuses.Draft;

        return new BranchSurveyResponse
        {
            SurveyId = survey.Id,
            Status = survey.Status,
            CanEditForm = isLeadSurveyor && isEditableStatus,
            IsSaved = branchSurvey != null,
            LastSavedOn = branchSurvey?.LastModifiedOn ?? branchSurvey?.CreatedOn,
            Form = branchSurvey != null ? branchSurvey.ToForm() : CreateEmptyForm(survey)
        };
    }

    // Before the first save, the form starts with what the Committee Lead already entered in the assignment.
    private static BranchSurveyForm CreateEmptyForm(TblSurvey survey)
    {
        return new BranchSurveyForm
        {
            SurveyDate = DateOnly.FromDateTime(DateTime.Today),
            Geographic = new GeographicSection
            {
                AdministrativeUnitId = survey.AdministrativeUnitId,
                ProposedOperationalArea = survey.ProposedArea
            }
        };
    }
}
#endregion
