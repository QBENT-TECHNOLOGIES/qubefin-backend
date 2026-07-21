using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Employees.Queries;

#region --- QUERY ---
public record GetEmployeePersonalByIdQuery(Guid Id) : IRequest<Result<GetPersonalResponse>>;
#endregion
#region --- RESPONSE ---
public record GetPersonalResponse(Guid Id, 
        string Code, 
        string? Salutation, 
        string FirstName, 
        string? MiddleName, 
        string LastName, 
        string? FatherName, 
        string? MotherName,
        DateOnly DateOfBirth, 
        string Gender, 
        string Religion, 
        string? Caste, 
        string Nationality, 
        string BloodGroup, 
        string? DisablityType,
        string? MaritalStatus
    );

#endregion
#region --- HANDLER ---
internal sealed class GetEmployeePersonalByIdQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetEmployeePersonalByIdQuery, Result<GetPersonalResponse>>
{
    public async Task<Result<GetPersonalResponse>> Handle(GetEmployeePersonalByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await context.TblEmployees.Where(m => m.Id == request.Id).FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (employee is null)
        {
            return new RecordNotFoundError($"Employee not found for the given Id");
        }
        return Result.Ok(new GetPersonalResponse(
            employee.Id,
            employee.Code,
            employee.Salutation,
            employee.FirstName,
            employee.MiddleName,
            employee.LastName,
            employee.FatherName,
            employee.MotherName,
            employee.DateOfBirth,
            employee.Gender,
            employee.Religion,
            employee.Caste,
            employee.Nationality,
            employee.BloodGroup,
            employee.DisablityType,
            employee.MaritalStatus
        ));
    }
}
#endregion