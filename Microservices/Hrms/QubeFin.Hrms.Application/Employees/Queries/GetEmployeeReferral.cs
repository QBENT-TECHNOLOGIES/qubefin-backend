using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Hrms.Application.Employees.Queries;

#region --- QUERY ---
public record GetEmployeeReferralQuery(Guid Id) : IRequest<Result<GetReferralResponse>>;
#endregion
#region --- RESPONSE ---
public record GetReferralResponse(
    Guid Id,
    Guid? ReferedBy,
    string? HowYouKnow,
    string? EmployeeName,
    string? Designation,
    string? Code
    );
#endregion
#region --- HANDLER ---
internal sealed class GetEmployeeReferralQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetEmployeeReferralQuery, Result<GetReferralResponse>>
{
    public async Task<Result<GetReferralResponse>> Handle(GetEmployeeReferralQuery request, CancellationToken cancellationToken)
    {
        var employee = await context
            .TblEmployees
            .Where(m => m.Id == request.Id)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (employee is null)
        {
            return new RecordNotFoundError($"Employee not found for the given Id");
        }

        string? referrerName = null;
        string? referrerDesignationName = null;
        string? referrerCode = null;

        // ReferedBy holds another employee's Id (not a plain label), so the referrer's
        // Name/Designation/Code are resolved with a second lookup here rather than a
        // static Include -- there is no self-referencing navigation property for it.
        if (employee.ReferedBy is not null)
        {
            var referrer = await context
                .TblEmployees
                .Include(e => e.TblEmployeeDesignations).ThenInclude(e => e.Designation)
                .Where(m => m.Id == employee.ReferedBy)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (referrer is not null)
            {
                var referrerDesignation = !referrer.TblEmployeeDesignations.Any() ? null :
                    referrer.TblEmployeeDesignations.Any(ed => ed.EffectiveTo == null) ?
                    referrer.TblEmployeeDesignations.Where(ed => ed.EffectiveTo == null).First() :
                    referrer.TblEmployeeDesignations.OrderByDescending(ed => ed.EffectiveFrom).First();
                // Same "current designation" resolution used in GetEmployeeOfficialById:
                // prefer the open-ended (EffectiveTo == null) row, else the most recent one.

                referrerName = referrer.FullName;
                referrerDesignationName = referrerDesignation?.Designation?.Name;
                referrerCode = referrer.Code;
            }
        }

        return Result.Ok(new GetReferralResponse(
            Id: employee.Id,
            ReferedBy: employee.ReferedBy,
            HowYouKnow: employee.HowYouKnow,
            EmployeeName: referrerName,
            Designation: referrerDesignationName,
            Code: referrerCode
        ));
    }
}
#endregion
