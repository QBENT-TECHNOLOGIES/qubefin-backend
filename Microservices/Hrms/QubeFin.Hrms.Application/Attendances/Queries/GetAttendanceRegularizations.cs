using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QubeFin.Hrms.Application.Attendances.Models;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Attendances.Queries;

#region --- QUERY ---
public record GetAttendanceRegularizationsByIdQuery(Guid Id, Guid EmployeeId) : IRequest<Result<GetAttendanceRegularizationsByIdResponse>>;
#endregion

#region --- VALIDATOR ---
public class GetAttendanceRegularizationsByIdQueryValidator : AbstractValidator<GetAttendanceRegularizationsByIdQuery>
{
    public GetAttendanceRegularizationsByIdQueryValidator()
    {
        RuleFor(v => v.Id).NotEmpty().WithMessage("Regularization Id is required.");
    }
}
#endregion

#region --- RESPONSE ---
public record GetAttendanceRegularizationsByIdResponse(RegularizationDetailResponse? response);
#endregion

#region --- HANDLER ---
internal sealed class GetAttendanceRegularizationsByIdQueryHandler(QubeFinDataContext context) : IRequestHandler<GetAttendanceRegularizationsByIdQuery, Result<GetAttendanceRegularizationsByIdResponse>>
{
    public async Task<Result<GetAttendanceRegularizationsByIdResponse>> Handle(GetAttendanceRegularizationsByIdQuery request, CancellationToken cancellationToken)
    {
        var regularizationResponse = await context.Set<RegularizationResponse>()
       .FromSqlRaw("EXEC [Hrms].[USP_GetRegularization] @Id, @EmployeeId",
         new SqlParameter("@Id", request.Id),
         new SqlParameter("@EmployeeId", request.EmployeeId)
        )
       .AsNoTracking()
       .ToListAsync(cancellationToken);

        if (regularizationResponse == null || !regularizationResponse.Any())
            return new GetAttendanceRegularizationsByIdResponse(null);

        var first = regularizationResponse.First();
        var response = new RegularizationDetailResponse
        {
            Id = first.Id,
            EmployeeId = first.EmployeeId,
            RegularizationType = first.RegularizationType,
            RegularizationDates = first.RegularizationDates,
            Reason = first.Reason,
            Attachment = first.Attachment,
            CreatedBy = first.CreatedBy,
            CreatedOn = first.CreatedOn,
            CurrentStatus = first.CurrentStatus,
            IsRecommendVisible = first.IsRecommendVisible,
            IsApprovalVisible = first.IsApprovalVisible,

            Events = regularizationResponse.Select(x => new RegularizationEvent
            {
                ApprovalCategory = x.ApprovalCategory,
                EventDate = x.EventDate,
                Remarks = x.Remarks,
                SenderDesignation = x.SenderDesignation,
                ReceiverDesignation = x.ReceiverDesignation,
                EventCategory = x.EventCategory,
                EventStatus = x.EventStatus,
                EventButtonText = x.EventButtonText
            }).ToList()
        };
        return Result.Ok(new GetAttendanceRegularizationsByIdResponse(response));
    }
}
#endregion
