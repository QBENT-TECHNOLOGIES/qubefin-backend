using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Persistence;

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
        var employee = await context.TblEmployees.FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);
        if (employee is null)
        {
            return new RecordNotFoundError("Employee not found for the given Id");
        }

        return Result.Ok(new GetContactResponse(
            employee.Id,
            EmptyIfWhiteSpace(employee.MobileNo),
            EmptyIfWhiteSpace(employee.PersonalEmail),
            EmptyIfWhiteSpace(employee.EmergencyContactRelation1),
            EmptyIfWhiteSpace(employee.EmergencyContactName1),
            EmptyIfWhiteSpace(employee.EmergencyContactMobile1),
            EmptyIfWhiteSpace(employee.EmergencyContactRelation2),
            EmptyIfWhiteSpace(employee.EmergencyContactName2),
            EmptyIfWhiteSpace(employee.EmergencyContactMobile2)
        ));
    }
    private static string EmptyIfWhiteSpace(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
    }
}
#endregion