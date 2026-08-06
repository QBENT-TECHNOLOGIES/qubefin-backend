using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QubeFin.Hrms.Application.Leaves.Models;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Leaves.Queries;

#region --- QUERY ---
public record GetRequestByIdQuery(Guid Id, Guid EmployeeId) : IRequest<Result<GetRequestByIdResponse>>;
#endregion

#region --- VALIDATOR ---
public class GetRequestByIdQueryValidator : AbstractValidator<GetRequestByIdQuery>
{
    public GetRequestByIdQueryValidator()
    {
        RuleFor(v => v.Id).NotEmpty().WithMessage("Leave Request Id is required.");
    }
}
#endregion

#region --- RESPONSE ---
public record GetRequestByIdResponse(LeaveRequestDetailResponse? response);
#endregion

#region --- HANDLER ---
internal sealed class GetRequestByIdQueryHandler(QubeFinDataContext context) : IRequestHandler<GetRequestByIdQuery, Result<GetRequestByIdResponse>>
{
    public async Task<Result<GetRequestByIdResponse>> Handle(GetRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var leaveRequestResponse = await context.Set<LeaveRequestResponse>().FromSqlRaw("EXEC [Hrms].[USP_GetLeaveRequestById] @Id, @EmployeeId",
         new SqlParameter("@Id", request.Id),
         new SqlParameter("@EmployeeId", request.EmployeeId)
        )
       .AsNoTracking()
       .ToListAsync(cancellationToken);

        if (leaveRequestResponse == null || !leaveRequestResponse.Any())
            return new GetRequestByIdResponse(null);

        var first = leaveRequestResponse.First();
        var response = new LeaveRequestDetailResponse
        {
            Id = first.Id,
            EmployeeName = first.EmployeeName,
            LeaveType = first.LeaveType,
            LeaveTypeId = first.LeaveTypeId,
            FromDate = first.FromDate,
            ToDate = first.ToDate,
            TotalDays = first.TotalDays,
            CurrentStatus = first.CurrentStatus,
            Reason = first.Reason,
            Address = first.Address,
            EnclosedDocName = first.EnclosedDocName,
            EnclosedDocNo = first.EnclosedDocNo,
            IsSubmitted = first.IsSubmitted,
            ApprovalCategory = first.ApprovalCategory,
            EventButtonText = first.EventButtonText,
            IsRecommendEvent = first.IsRecommendEvent,
            IsApprovalEvent = first.IsApprovalEvent,
            IsCancellable = first.IsCancellable,
            RejectedReason = first.RejectedReason,

            Events = leaveRequestResponse.Select(x => new LeaveRequestEvent
            {
                Event = x.EventStatus,
                EventOn = x.EventDate,
                EventRemarks = x.Remarks,
                SenderDesignation = x.SenderDesignation,
                ReceiverDesignation = x.ReceiverDesignation
            }).ToList()
        };
        return Result.Ok(new GetRequestByIdResponse(response));
    }
}
#endregion
