using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QubeFin.Hrms.Application.Attendances.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Attendances.Queries;

#region --- QUERY ---
public record GetAttendanceRegularizationsByIdQuery(Guid Id, Guid EmployeeId) : IRequest<Result<RegularizationDetailResponse?>>;
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

#region --- HANDLER ---
internal sealed class GetAttendanceRegularizationsByIdQueryHandler(QubeFinDataContext context, IFileStorageRepository fileStorageRepository) :
    IRequestHandler<GetAttendanceRegularizationsByIdQuery, Result<RegularizationDetailResponse?>>
{
    public async Task<Result<RegularizationDetailResponse?>> Handle(GetAttendanceRegularizationsByIdQuery request, CancellationToken cancellationToken)
    {
        var regularizationResponse = await context.Set<RegularizationResponse>()
       .FromSqlRaw("EXEC [Hrms].[USP_RegularizationGet] @Id, @EmployeeId",
         new SqlParameter("@Id", request.Id),
         new SqlParameter("@EmployeeId", request.EmployeeId)
        )
       .AsNoTracking()
       .ToListAsync(cancellationToken);

        if (regularizationResponse == null || !regularizationResponse.Any())
            return Result.Ok((RegularizationDetailResponse?)null);

        var first = regularizationResponse.OrderBy(g => g.EventDate).First();
        var response = new RegularizationDetailResponse
        {
            Id = first.Id,
            EmployeeId = first.EmployeeId,
            RegularizationType = first.RegularizationType,
            RegularizationDates = first.RegularizationDates,
            Reason = first.Reason,
            Attachment = first.Attachment,
            AttachmentUrl = !string.IsNullOrEmpty(first.Attachment) ? await fileStorageRepository.GetFileUrlAsync(first.Attachment, cancellationToken) : null,
            Remarks = first.Remarks,
            CreatedBy = first.CreatedBy,
            CreatedOn = first.CreatedOn,
            CurrentStatus = first.CurrentStatus,
            IsRecommendEvent = first.IsRecommendEvent,
            IsApprovalEvent = first.IsApprovalEvent,

            Events = regularizationResponse.Select(x => new RegularizationEvent
            {
                EventStatus = x.EventStatus,
                EventDate = x.EventDate,
                Designation = x.ReceiverDesignation,
                Remarks = x.EventRemarks
            }).ToList()
        };
        if (response != null)
        {
            response.Events.Insert(0, new RegularizationEvent
            {
                EventStatus = "Requested",
                EventDate = response.CreatedOn,
                Designation = first.SenderDesignation
            });
        }
        return Result.Ok((RegularizationDetailResponse?)response);
    }
}
#endregion
