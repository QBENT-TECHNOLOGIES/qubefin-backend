using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.Employees.Models;
using QubeFin.Persistence;
namespace QubeFin.Hrms.Application.Employees.Queries;

#region --- QUERY ---
public record GetJoiningInfoQuery(Guid EmployeeId) : IRequest<Result<JoiningInfoResponse>>;
#endregion

#region --- VALIDATION ---
public class GetJoiningInfoQueryValidator : AbstractValidator<GetJoiningInfoQuery>
{
    public GetJoiningInfoQueryValidator()
    {
        RuleFor(x => x.EmployeeId).NotEmpty().WithMessage("Employee is required.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class GetJoiningInfoQueryHandler(QubeFinDataContext context) : IRequestHandler<GetJoiningInfoQuery, Result<JoiningInfoResponse>>
{
    public async Task<Result<JoiningInfoResponse>> Handle(GetJoiningInfoQuery request, CancellationToken cancellationToken)
    {
        var employeeExists = await context.TblEmployees.AsNoTracking().AnyAsync(e => e.Id == request.EmployeeId, cancellationToken);

        if (!employeeExists)
        {
            return new RecordNotFoundError($"Employee not found for the given Id");
        }

        var joiningInfo = await context.TblJoiningInfos
            .Where(j => j.EmployeeId == request.EmployeeId)
            .AsNoTracking()
            .Select(j => new JoiningInfoResponse
            {
                Id = j.Id,
                EmployeeId = j.EmployeeId,
                ReferenceNo = j.ReferenceNo,
                OfferLetterDate = j.OfferLetterDate,
                MonthlyCtc = j.MonthlyCtc,
                ApprovalStatus = j.ApprovalStatus
            })
            .FirstOrDefaultAsync(cancellationToken);

        // No joining info saved yet: return an empty record so the form opens in create mode.
        return Result.Ok(joiningInfo ?? new JoiningInfoResponse { EmployeeId = request.EmployeeId });
    }
}
#endregion
