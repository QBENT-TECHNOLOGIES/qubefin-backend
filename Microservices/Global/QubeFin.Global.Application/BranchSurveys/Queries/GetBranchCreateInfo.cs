using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using QubeFin.Core.Results;
using QubeFin.Global.Application.BranchSurveys.Models;
using QubeFin.Global.Application.SurveyUnit;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.BranchSurveys.Queries;

#region --- QUERY ---
public record GetBranchCreateInfoQuery(Guid SurveyId, Guid UserId, Guid EmployeeId) : IRequest<Result<BranchCreateInfoResponse>>;
#endregion

#region --- VALIDATOR ---
public class GetBranchCreateInfoQueryValidator : AbstractValidator<GetBranchCreateInfoQuery>
{
    public GetBranchCreateInfoQueryValidator()
    {
        RuleFor(v => v.SurveyId).NotEmpty().WithMessage("Survey Id is required.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class GetBranchCreateInfoQueryHandler(
    QubeFinDataContext context,
    ISurveyCommitteeRepository committeeRepository,
    IOrganizationUnitRepository organizationUnitRepository) : IRequestHandler<GetBranchCreateInfoQuery, Result<BranchCreateInfoResponse>>
{
    public async Task<Result<BranchCreateInfoResponse>> Handle(GetBranchCreateInfoQuery request, CancellationToken cancellationToken)
    {
        var survey = await context.TblSurveys
            .Include(s => s.TblSurveyAssigneds)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.SurveyId, cancellationToken);
        if (survey is null)
            return new RecordNotFoundError("Survey not found.");

        if (!await SurveyAccess.CanViewAsync(context, survey, request.UserId, request.EmployeeId, cancellationToken))
            return new ForbiddenError("You do not have access to this survey.");

        var response = new BranchCreateInfoResponse { SurveyId = survey.Id };

        // The branch already exists: show it.
        if (survey.SurveyType == SurveyTypes.Branch && survey.BranchOrganizationUnitId.HasValue)
        {
            var branch = await context.TblOrganizationUnits.AsNoTracking()
                .Where(o => o.Id == survey.BranchOrganizationUnitId)
                .Select(o => new { o.Id, o.Name, o.CodeVal })
                .FirstOrDefaultAsync(cancellationToken);
            response.BranchId = branch?.Id;
            response.BranchName = branch?.Name;
            response.BranchCode = branch?.CodeVal;
            return response;
        }

        if (survey.SurveyType != SurveyTypes.Branch || survey.Status != SurveyStatuses.Approved)
            return response;

        response.CanCreate = await committeeRepository.IsActiveLeadAsync(request.EmployeeId);
        if (!response.CanCreate)
            return response;

        // Values the Lead starts with in the window.
        response.SuggestedName = survey.ProposedArea;
        response.CompanyId = survey.CompanyId;
        response.CompanyName = survey.CompanyId.HasValue
            ? await context.TblCompanies.AsNoTracking().Where(c => c.Id == survey.CompanyId).Select(c => c.Name).FirstOrDefaultAsync(cancellationToken)
            : null;
        var area = await organizationUnitRepository.GetAreaOfAsync(survey.AdministrativeUnitId, cancellationToken);
        response.StateId = area?.StateId;
        response.StateName = area?.StateName;
        response.DistrictId = area?.DistrictId;
        response.DistrictName = area?.DistrictName;

        var location = await context.TblBranchSurveys.AsNoTracking()
            .Where(b => b.SurveyId == survey.Id)
            .Select(b => b.Location)
            .FirstOrDefaultAsync(cancellationToken);
        if (location is Point point)
        {
            response.Latitude = (decimal)point.Y;
            response.Longitude = (decimal)point.X;
        }

        return response;
    }
}
#endregion
