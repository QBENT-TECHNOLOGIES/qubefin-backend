using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.Employees.Models;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.Employees.Queries;

#region --- QUERY ---
public record GetEmployeeDependentNomineeQuery(Guid Id) : IRequest<Result<List<DependentNomineeDetailResponse>>>;
#endregion
#region --- HANDLER ---
internal sealed class GetEmployeeDependentNomineeQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetEmployeeDependentNomineeQuery, Result<List<DependentNomineeDetailResponse>>>
{
    public async Task<Result<List<DependentNomineeDetailResponse>>> Handle(GetEmployeeDependentNomineeQuery request, CancellationToken cancellationToken)
    {
        var employee = await context.TblEmployees
            .Include(m => m.TblEmployeeDependentNominees)
            .Where(m => m.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (employee is null)
        {
            return new RecordNotFoundError($"Employee not found for the given Id");
        }

        var nominees = employee.TblEmployeeDependentNominees.Select(d => new DependentNomineeDetailResponse
        {
            Id = d.Id,
            EmployeeId = d.EmployeeId,
            NomineeName = d.NomineeName,
            RelationWithInsuredPerson = d.RelationWithInsuredPerson,
            DateOfBirth = d.DateOfBirth,
            Age = d.Age,
            UhidAbhaNumber = d.UhidAbhaNumber,
            AbhaAddress = d.AbhaAddress,
            Uan = d.Uan,
            AadharNumber = d.AadharNumber,
            VoterIdnumber = d.VoterIdnumber,
            IsResidingWithIp = d.IsResidingWithIp
        }).ToList();

        return Result.Ok(nominees);
    }
}
#endregion
