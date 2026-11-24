using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Global.Application.SurveyUnit;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.Mobilization.Queries;

#region --- QUERY ---
public record GetMobilizationQuery(Guid SurveyId, Guid UserId, Guid EmployeeId) : IRequest<Result<MobilizationResponse>>;
#endregion

#region --- VALIDATOR ---
public class GetMobilizationQueryValidator : AbstractValidator<GetMobilizationQuery>
{
    public GetMobilizationQueryValidator()
    {
        RuleFor(v => v.SurveyId).NotEmpty().WithMessage("Survey Id is required.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class GetMobilizationQueryHandler(
    QubeFinDataContext context,
    ISurveyRepository surveyRepository) : IRequestHandler<GetMobilizationQuery, Result<MobilizationResponse>>
{
    public async Task<Result<MobilizationResponse>> Handle(GetMobilizationQuery request, CancellationToken cancellationToken)
    {
        var survey = await context.TblSurveys.AsNoTracking()
            .Include(s => s.TblSurveyAssigneds)
            .FirstOrDefaultAsync(s => s.Id == request.SurveyId, cancellationToken);
        if (survey is null)
            return new RecordNotFoundError("Survey not found.");

        if (survey.SurveyType != SurveyTypes.Group)
            return new ValidationError("Community Mobilization is only for Group Surveys.");

        if (!await SurveyAccess.CanViewAsync(context, survey, request.UserId, request.EmployeeId, cancellationToken))
            return new ForbiddenError("You do not have access to this survey.");

        var isOpen = survey.Status == SurveyStatuses.Approved;
        var response = new MobilizationResponse
        {
            SurveyId = survey.Id,
            Status = survey.Status,
            IsOpen = isOpen,
            CanEdit = isOpen && survey.TblSurveyAssigneds.Any(a => a.EmployeeId == request.EmployeeId),
            CanChangeDecision = isOpen && survey.BranchOrganizationUnitId.HasValue
                && await surveyRepository.IsHeadOfBranchAsync(request.EmployeeId, survey.BranchOrganizationUnitId.Value)
        };

        var rows = await context.TblGroupBorrowers.AsNoTracking()
            .Where(b => b.SurveyId == survey.Id)
            .OrderBy(b => b.CreatedOn)
            .ToListAsync(cancellationToken);

        response.Borrowers = rows.Select(b => BorrowerMapper.ToItem(b)).ToList();
        return Result.Ok(response);
    }
}
#endregion
