using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Global.Application.SurveyUnit.Models;
using QubeFin.Persistence;

namespace QubeFin.Global.Application.SurveyUnit.Queries;

#region --- QUERY ---
public record GetSurveyByIdQuery(Guid Id) : IRequest<Result<GetByIdResponse>>;
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
#region --- RESPONSE ---
public record GetByIdResponse(SurveyResponse SurveyResponse);
#endregion

#region --- HANDLER ---
internal sealed class GetSurveyByIdQueryHandler(QubeFinDataContext context) : IRequestHandler<GetSurveyByIdQuery, Result<GetByIdResponse>>
{
    public async Task<Result<GetByIdResponse>> Handle(GetSurveyByIdQuery request, CancellationToken cancellationToken)
    {
        var surveyEntity = await context.TblSurveys.Include(m => m.TblSurveyAssigneds).ThenInclude(m => m.Employee).AsNoTracking().FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken: cancellationToken);

        if (surveyEntity is null)
        {
            return new RecordNotFoundError($"Survey not found for the given Id");
        }
        return new GetByIdResponse(new SurveyResponse
        {
            Id = surveyEntity.Id,
            SurveyType = surveyEntity.SurveyType,
            AssignmentNo = surveyEntity.AssignmentNo,
            AssignmentDate = surveyEntity.AssignmentDate,
            ProposedArea = surveyEntity.ProposedArea,
            AdministrativeUnitId = surveyEntity.AdministrativeUnitId,
            TentativeSubmissionDate = surveyEntity.TentativeSubmissionDate,
            CreatedBy = surveyEntity.CreatedBy,
            CreatedOn = surveyEntity.CreatedOn,
            LastModifiedBy = surveyEntity.LastModifiedBy,
            LastModifiedOn = surveyEntity.LastModifiedOn,
            SurveyAssigneds = surveyEntity.TblSurveyAssigneds.OrderBy(s => s.AssignedDate).Select(s => new SurveyAssignedResponse
            {
                Id = s.Id,
                SurveyId = s.SurveyId,
                EmployeeId = s.EmployeeId,
                EmployeeName = s.Employee.FullName + " (" + s.Employee.Code + ")",
                IsLead = s.IsLead,
                AssignedDate = s.AssignedDate,
                AssignedBy = s.AssignedBy
            }).ToList()
        });
    }
}
#endregion
