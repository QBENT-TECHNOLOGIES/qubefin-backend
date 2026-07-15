using MediatR;
using FluentResults;
using QubeFin.Persistence;
using QubeFin.Core.Results;
using Microsoft.EntityFrameworkCore;
using QubeFin.Global.Application.SurveyCommittees.Models;

namespace QubeFin.Global.Application.SurveyCommittees.Queries;

#region --- QUERY ---
public record GetByIdQuery(Guid Id) : IRequest<Result<GetByIdResponse>>;
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
            EmployeeName = SurveyCommitteeMember.Employee.FullName,
            IsLead = SurveyCommitteeMember.IsLead,
            IsActive = SurveyCommitteeMember.IsActive,
            AssignedFrom = SurveyCommitteeMember.AssignedFrom,
            AssignedTo = SurveyCommitteeMember.AssignedTo
        });
    }
}
#endregion