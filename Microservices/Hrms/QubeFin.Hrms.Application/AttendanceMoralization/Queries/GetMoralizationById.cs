using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Hrms.Application.AttendanceMoralization.Models;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.AttendanceMoralization.Queries;
#region --- QUERY ---
public record GetMoralizationByIdQuery(Guid Id) : IRequest<Result<List<EmployeeLosDetails>>>;
#endregion

#region --- VALIDATOR ---
public class GetMoralizationByIdQueryValidator : AbstractValidator<GetMoralizationByIdQuery>
{
    public GetMoralizationByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEqual(Guid.Empty).WithMessage("Id is required.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class GetMoralizationByIdQueryHandler(QubeFinDataContext context) :
        IRequestHandler<GetMoralizationByIdQuery, Result<List<EmployeeLosDetails>>>
{
    public async Task<Result<List<EmployeeLosDetails>>> Handle(GetMoralizationByIdQuery request, CancellationToken cancellationToken)
    {
        var employeeLosDetails = await context.TblEmployeeLopDetails
            .Where(m => m.EmployeeLopId == request.Id)
            .OrderBy(m => m.AbsentDate)
            .Select(m => new EmployeeLosDetails
            {
                Id = m.Id,
                EmployeeLopId = m.EmployeeLopId,
                AbsentDate = m.AbsentDate,
                LeaveTypeId = m.LeaveTypeId
            })
            .ToListAsync(cancellationToken);
        return Result.Ok(employeeLosDetails);
    }
}
#endregion