using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Global.Application.SurveyUnit;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.Cgt.Queries;

#region --- QUERY ---
public record GetCgtQuery(Guid SurveyId, Guid UserId, Guid EmployeeId) : IRequest<Result<CgtResponse>>;
#endregion

#region --- VALIDATOR ---
public class GetCgtQueryValidator : AbstractValidator<GetCgtQuery>
{
    public GetCgtQueryValidator()
    {
        RuleFor(v => v.SurveyId).NotEmpty().WithMessage("Survey Id is required.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class GetCgtQueryHandler(
    QubeFinDataContext context,
    IGroupSurveyRepository groupSurveyRepository) : IRequestHandler<GetCgtQuery, Result<CgtResponse>>
{
    public async Task<Result<CgtResponse>> Handle(GetCgtQuery request, CancellationToken cancellationToken)
    {
        var survey = await context.TblSurveys.AsNoTracking()
            .Include(s => s.TblSurveyAssigneds)
            .FirstOrDefaultAsync(s => s.Id == request.SurveyId, cancellationToken);
        if (survey is null)
            return new RecordNotFoundError("Survey not found.");
        if (survey.SurveyType != SurveyTypes.Group)
            return new ValidationError("CGT is only for Group Surveys.");
        if (!await SurveyAccess.CanViewAsync(context, survey, request.UserId, request.EmployeeId, cancellationToken))
            return new ForbiddenError("You do not have access to this survey.");

        var cgt = await context.TblGroupCgts.AsNoTracking()
            .Include(c => c.TblGroupCgtTopics)
            .FirstOrDefaultAsync(c => c.SurveyId == survey.Id, cancellationToken);

        var isOpen = survey.Status == SurveyStatuses.Approved;
        var response = new CgtResponse
        {
            SurveyId = survey.Id,
            IsOpen = isOpen,
            IsSaved = cgt is not null,
            Status = cgt?.Status ?? CgtStatuses.InProgress,
            ProposedArea = survey.ProposedArea,
            BranchName = survey.BranchOrganizationUnitId.HasValue
                ? await context.TblOrganizationUnits.AsNoTracking()
                    .Where(o => o.Id == survey.BranchOrganizationUnitId).Select(o => o.Name).FirstOrDefaultAsync(cancellationToken)
                : null,
            CanEdit = isOpen
                && survey.TblSurveyAssigneds.Any(a => a.EmployeeId == request.EmployeeId)
                && (cgt is null || cgt.Status != CgtStatuses.Completed),
            StartDate = cgt?.StartDate,
            EndDate = cgt?.EndDate,
            CompletedOn = cgt?.CompletedOn
        };

        // All 10 topics are always listed; answers come from what was saved.
        response.Topics = CgtTopics.All.Select(t =>
        {
            var saved = cgt?.TblGroupCgtTopics.FirstOrDefault(x => x.TopicNo == t.No);
            return new CgtTopicItem { TopicNo = t.No, Title = t.Title, Points = t.Points, Covered = saved?.Covered, Remarks = saved?.Remarks };
        }).ToList();

        if (cgt is not null)
        {
            var info = await groupSurveyRepository.GetSignOffAsync(cgt.ConductedByEmployeeId);
            if (info is not null)
                response.ConductedBy = new CgtConductedBy { Name = info.EmployeeName, Designation = info.Designation, EmployeeCode = info.Pin };
        }

        return Result.Ok(response);
    }
}
#endregion
