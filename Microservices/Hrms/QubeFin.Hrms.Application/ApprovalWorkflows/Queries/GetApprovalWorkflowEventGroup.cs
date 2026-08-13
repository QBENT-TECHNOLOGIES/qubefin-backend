using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.ApprovalWorkflows.Queries;

#region --- QUERY ---
public record GetApprovalWorkflowEventGroupQuery(string Category, Guid? OrganizationUnitTypeId, Guid? LeaveTypeId, Guid? SalaryGradeId, Guid? PostId, int MinimumDays = 0, int MaximumDays = 0) 
    : IRequest<Result<GetApprovalWorkflowEventGroupResponse>>;
#endregion

#region --- RESPONSE ---
public record GetApprovalWorkflowEventGroupResponse();
#endregion

#region --- HANDLER ---
internal sealed class GetApprovalWorkflowEventGroupQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetApprovalWorkflowEventGroupQuery, Result<GetApprovalWorkflowEventGroupResponse>>
{
    public async Task<Result<GetApprovalWorkflowEventGroupResponse>> Handle(GetApprovalWorkflowEventGroupQuery request, CancellationToken cancellationToken)
    {
        var workflowEvents = request.Category == "LEAVE"
            ? await context.TblApprovalWorkflowEvents.Where(m => m.Category == request.Category && m.OrganizationUnitTypeId == request.OrganizationUnitTypeId &&
                m.LeaveTypeId == request.LeaveTypeId && m.SalaryGradeId == request.SalaryGradeId && m.MinimumDays == request.MinimumDays && m.MaximumDays == request.MaximumDays).ToListAsync(cancellationToken)
            : await context.TblApprovalWorkflowEvents.Where(m => m.Category == request.Category && m.OrganizationUnitTypeId == request.OrganizationUnitTypeId &&
                m.PostId == request.PostId).ToListAsync(cancellationToken);

        return new Result<GetApprovalWorkflowEventGroupResponse>();
    }
}
#endregion
