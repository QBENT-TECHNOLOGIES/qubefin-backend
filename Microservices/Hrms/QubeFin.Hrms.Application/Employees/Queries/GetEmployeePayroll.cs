using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Employees.Queries;

#region --- QUERY ---
public record GetEmployeePayrollByIdQuery(Guid Id) : IRequest<Result<GetPayrollResponse>>;
#endregion
#region --- RESPONSE ---
public record GetPayrollResponse(Guid Id,
        Guid? BankId, long BankAccountNo, string BankHolderName, string BankBranch, string BankAccountType,
        bool HasEsiEligible, string? EsiIpNumber, string? UniversalAccountNumber, bool IsPayrollActive
    );

#endregion
#region --- HANDLER ---
internal sealed class GetEmployeePayrollByIdQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetEmployeePayrollByIdQuery, Result<GetPayrollResponse>>
{
    public async Task<Result<GetPayrollResponse>> Handle(GetEmployeePayrollByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await context.TblEmployees.Where(m => m.Id == request.Id).FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (employee is null)
        {
            return new RecordNotFoundError($"Employee not found for the given Id");
        }
        return Result.Ok(new GetPayrollResponse(
            employee.Id,
            employee.BankId,
            employee.BankAccountNo.Value,
            employee.BankHolderName,
            employee.BankBranch,
            employee.BankAccountType,
            employee.HasEsiEligible,
            employee.Esiipno,
            employee.UniversalAccountNo,
            employee.IsPayrollActive
        ));
    }
}
#endregion