using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.Employees.Queries;

#region --- QUERY ---
public record GetEmployeePayrollByIdQuery(Guid Id) : IRequest<Result<GetPayrollResponse>>;
#endregion
#region --- RESPONSE ---
public record GetPayrollResponse
    (Guid Id, Guid? BankId, string? BankHolderName, long? BankAccountNo, string? IfscCode, string? BankBranch, string? BankAccountType,
    string? UniversalAccountNumber, string? PFAccountNo, bool HasEsiEligible, string? EsiIpNumber, bool IsPayrollActive);

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
            employee.BankHolderName,
            employee.BankAccountNo,
            employee.IfscCode,
            employee.BankBranch,
            employee.BankAccountType,
            employee.UniversalAccountNo,
            employee.PfaccountNo,
            employee.HasEsiEligible,
            employee.Esiipno,
            employee.IsPayrollActive
        ));
    }
}
#endregion