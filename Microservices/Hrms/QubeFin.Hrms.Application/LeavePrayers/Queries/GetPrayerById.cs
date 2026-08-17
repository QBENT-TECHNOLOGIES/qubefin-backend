using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QubeFin.Hrms.Application.Attendances.Models;
using QubeFin.Hrms.Application.LeavePrayers.Models;
using QubeFin.Hrms.Application.Leaves.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Hrms.Application.LeavePrayers.Queries;


#region --- QUERY ---
public record GetPrayerByIdQuery(Guid Id, Guid EmployeeId) : IRequest<Result<LeavePrayerDetailResponse?>>;
#endregion

#region --- VALIDATOR ---
public class GetPrayerByIdQueryValidator : AbstractValidator<GetPrayerByIdQuery>
{
    public GetPrayerByIdQueryValidator()
    {
        RuleFor(v => v.Id).NotEmpty().WithMessage("Leave Request Id is required.");
    }
}
#endregion

#region --- RESPONSE ---
#endregion

#region --- HANDLER ---
internal sealed class GetPrayerByIdQueryHandler(QubeFinDataContext context, IFileStorageRepository fileStorageRepository) : IRequestHandler<GetPrayerByIdQuery, Result<LeavePrayerDetailResponse?>>
{
    public async Task<Result<LeavePrayerDetailResponse?>> Handle(GetPrayerByIdQuery request, CancellationToken cancellationToken)
    {
        var leaveRequestResponse = await context.Set<LeavePrayerResponse>().FromSqlRaw("EXEC [Hrms].[USP_GetLeavePrayerById] @Id, @EmployeeId",
         new SqlParameter("@Id", request.Id),
         new SqlParameter("@EmployeeId", request.EmployeeId)
        ).AsNoTracking().ToListAsync(cancellationToken);

        if (leaveRequestResponse == null || !leaveRequestResponse.Any())
            return Result.Fail("Something went wrong. Please try again later.");

        var first = leaveRequestResponse.OrderBy(g => g.EventDate).First();
        var response = new LeavePrayerDetailResponse
        {
            Id = first.Id,
            EmployeeName = first.EmployeeName,
            LeaveType = first.LeaveType,
            LeaveTypeId = first.LeaveTypeId,
            PrayerDays = first.PrayerDays,
            CurrentStatus = first.CurrentStatus,
            LeavePrayerRemarks = first.LeavePrayerRemarks,
            AppliedOn = first.AppliedOn,
            Attachment = first.Attachment,
            AttachmentUrl = !string.IsNullOrEmpty(first.Attachment) ? await fileStorageRepository.GetFileUrlAsync(first.Attachment, cancellationToken) : null,
            ApprovalCategory = first.ApprovalCategory,
            EventButtonText = first.EventButtonText,
            IsRecommendEvent = first.IsRecommendEvent,
            IsApprovalEvent = first.IsApprovalEvent,

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
        return Result.Ok((LeavePrayerDetailResponse?)response);
    }
}
#endregion