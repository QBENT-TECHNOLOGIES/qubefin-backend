using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Attendances.Queries;

#region --- QUERY ---
public record GetAttendanceRegularizationsByIdQuery(Guid Id) : IRequest<Result<GetAttendanceRegularizationsByIdResponse>>;
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
public record GetAttendanceRegularizationsByIdResponse(AttendanceRegularization? response);
#endregion

#region --- HANDLER ---
internal sealed class GetAttendanceRegularizationsByIdQueryHandler(IAttendanceRepository attendanceRepository) : IRequestHandler<GetAttendanceRegularizationsByIdQuery, Result<GetAttendanceRegularizationsByIdResponse>>
{
    public async Task<Result<GetAttendanceRegularizationsByIdResponse>> Handle(GetAttendanceRegularizationsByIdQuery request, CancellationToken cancellationToken)
    {
        var response = await attendanceRepository.GetRegularization(request.Id);
        return Result.Ok(new GetAttendanceRegularizationsByIdResponse(response));
    }
}
#endregion
