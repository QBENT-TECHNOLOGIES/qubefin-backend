using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QubeFin.Hrms.Application.Attendances.Models;
using QubeFin.Hrms.Application.Leaves.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Leaves.Queries;

#region --- QUERY ---
public record GetRequestByIdQuery(Guid Id, Guid EmployeeId) : IRequest<Result<LeaveRequestDetailResponse?>>;
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

#region --- HANDLER ---
internal sealed class GetRequestByIdQueryHandler(QubeFinDataContext context, IFileStorageRepository fileStorageRepository) :
    IRequestHandler<GetRequestByIdQuery, Result<LeaveRequestDetailResponse?>>
{
    public async Task<Result<LeaveRequestDetailResponse?>> Handle(GetRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var leaveRequestResponse = await context.Set<LeaveRequestResponse>().FromSqlRaw("EXEC [Hrms].[USP_GetLeaveRequestById] @Id, @EmployeeId",
         new SqlParameter("@Id", request.Id),
         new SqlParameter("@EmployeeId", request.EmployeeId)
        )
       .AsNoTracking()
       .ToListAsync(cancellationToken);

        if (leaveRequestResponse == null || !leaveRequestResponse.Any())
            return Result.Ok((LeaveRequestDetailResponse?)null);

        var first = leaveRequestResponse.OrderBy(g => g.EventDate).First();
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
            EnclosedDocUrl = !string.IsNullOrEmpty(first.EnclosedDocNo) ? await fileStorageRepository.GetFileUrlAsync(first.EnclosedDocNo, cancellationToken) : null,
            IsSubmitted = first.IsSubmitted,
            ApprovalCategory = first.ApprovalCategory,
            EventButtonText = first.EventButtonText,
            IsRecommendEvent = first.IsRecommendEvent,
            IsApprovalEvent = first.IsApprovalEvent,
            IsCancellable = first.IsCancellable,
            RejectedReason = first.RejectedReason,

            Events = leaveRequestResponse.Select(x => new LeaveRequestEvent
            {
                EventStatus = x.EventStatus,
                EventDate = x.EventDate,
                Designation = x.ReceiverDesignation,
                Remarks = x.Remarks
            }).ToList()
        };
        if (response != null)
        {
            response.Events.Insert(0, new LeaveRequestEvent
            {
                EventStatus = "Requested",
                EventDate = first.EventDate,
                Designation = first.SenderDesignation
            });
        }
        return Result.Ok((LeaveRequestDetailResponse?)response);
    }
}
#endregion
