using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Employees.Queries;

#region --- QUERY ---
public record GetEmployeeContactByIdQuery(Guid Id) : IRequest<Result<GetContactResponse>>;
#endregion
#region --- RESPONSE ---
public record GetContactResponse(Guid Id,
        string MobileNo, string? PersonalEmail, string? PrimaryEmergencyRelation, string? PrimaryEmergencyName, string? PrimaryEmergencyMobile,
        string? SecondaryEmergencyRelation, string? SecondaryEmergencyName, string? SecondaryEmergencyMobile
    );

#endregion
#region --- HANDLER ---
internal sealed class GetEmployeeContactByIdQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetEmployeeContactByIdQuery, Result<GetContactResponse>>
{
    public async Task<Result<GetContactResponse>> Handle(GetEmployeeContactByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await context.TblEmployees.Where(m => m.Id == request.Id).FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (employee is null)
        {
            return new RecordNotFoundError($"Employee not found for the given Id");
        }
        return Result.Ok(new GetContactResponse(
            employee.Id,
            employee.MobileNo,
            employee.PersonalEmail,
            employee.EmergencyContactRelation1,
            employee.EmergencyContactName1,
            employee.EmergencyContactMobile1,
            employee.EmergencyContactRelation2,
            employee.EmergencyContactName2,
            employee.EmergencyContactMobile2
        ));
    }
}
#endregion