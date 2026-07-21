using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Global.Application.SurveyCommittees.Commands;
using QubeFin.Global.Application.SurveyCommittees.Models;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.SurveyCommittees.Queries;

#region --- QUERY ---
public record GetByIdQuery(Guid Id) : IRequest<Result<GetByIdResponse>>;
#endregion

#region --- VALIDATOR ---
public class GetByIdQueryValidator : AbstractValidator<GetByIdQuery>
{
    public GetByIdQueryValidator()
    {
        RuleFor(v => v.Id).NotEmpty().WithMessage("Committee Id is required.");
    }
}
#endregion
#region --- RESPONSE ---
public record GetByIdResponse(SurveyCommitteeMemberResponse SurveyCommitteeMember);
#endregion

#region --- HANDLER ---
internal sealed class GetByIdQueryHandler(QubeFinDataContext context) : IRequestHandler<GetByIdQuery, Result<GetByIdResponse>>
{
    public async Task<Result<GetByIdResponse>> Handle(GetByIdQuery request, CancellationToken cancellationToken)
    {
        var SurveyCommitteeMember = await context.TblSurveyCommittees.Include(m => m.Employee).AsNoTracking().FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken: cancellationToken);

        if (SurveyCommitteeMember is null)
        {
            return new RecordNotFoundError($"Survey committee member not found for the given Id");
        }
        return new GetByIdResponse(new SurveyCommitteeMemberResponse
        {
            Id = SurveyCommitteeMember.Id,
            EmployeeId = SurveyCommitteeMember.EmployeeId,
            EmployeeName = SurveyCommitteeMember.Employee.FullName + "(" + SurveyCommitteeMember.Employee.Code + ")",
            IsLead = SurveyCommitteeMember.IsLead,
            IsActive = SurveyCommitteeMember.IsActive,
            AssignedFrom = SurveyCommitteeMember.AssignedFrom,
            AssignedTo = SurveyCommitteeMember.AssignedTo,
            AuditInfo = new AuditInfo
            {
                CreatedBy = SurveyCommitteeMember.CreatedBy.ToString(),
                CreatedOn = SurveyCommitteeMember.CreatedOn,
                LastModifiedBy = SurveyCommitteeMember.LastModifiedBy != null ? SurveyCommitteeMember.LastModifiedBy.ToString() : null,
                LastModifiedOn = SurveyCommitteeMember.LastModifiedOn
            }
        });
    }
}
#endregion