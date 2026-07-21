using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Employees.Queries;

#region --- QUERY ---
public record GetEmployeeOfficialByIdQuery(Guid Id) : IRequest<Result<GetOfficialResponse>>;
#endregion
#region --- RESPONSE ---
public record GetOfficialResponse(
    Guid Id,
    string Code,
    Guid? CompanyId,
    Guid? OrganizationUnitId,
    Guid? DepartmentId,
    string? EmployementType,
    DateOnly? JoiningDate,
    DateOnly? ConfirmationDate,
    DateOnly? SeparationDate,
    Guid? ReferedBy, 
    string? HowYouKnow,
    string? OfficialEmail,
    bool IsActive
    );

#endregion
#region --- HANDLER ---
internal sealed class GetEmployeeOfficialByIdQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetEmployeeOfficialByIdQuery, Result<GetOfficialResponse>>
{
    public async Task<Result<GetOfficialResponse>> Handle(GetEmployeeOfficialByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await context.TblEmployees.Where(m => m.Id == request.Id).FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (employee is null)
        {
            return new RecordNotFoundError($"Employee not found for the given Id");
        }
        return Result.Ok(new GetOfficialResponse(
            Id: employee.Id,
            Code: employee.Code,
            CompanyId: employee.CompanyId,
            OrganizationUnitId: employee.OrganizationUnitId,
            DepartmentId: employee.DepartmentId,
            EmployementType: employee.EmployementType,
            JoiningDate: employee.JoiningDate,
            ConfirmationDate: employee.ConfirmationDate,
            SeparationDate: employee.SeparationDate,
            ReferedBy: employee.ReferedBy,
            HowYouKnow: employee.HowYouKnow,
            OfficialEmail: employee.OfficialEmail,
            IsActive: employee.IsActive
        ));
    }
}
#endregion