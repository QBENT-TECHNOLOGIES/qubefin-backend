using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.Employees.Queries;

#region --- QUERY ---
public record GetEmployeeOfficialByIdQuery(Guid Id) : IRequest<Result<GetOfficialResponse>>;
#endregion
#region --- RESPONSE ---
public record GetOfficialResponse(
    Guid Id,
    string Code,
    Guid? OrganizationUnitTypeId,
    Guid? OrganizationUnitId,
    Guid? CompanyId,
    string? CompanyName,
    Guid? DesignationId,
    string? SalaryGrade,
    decimal? GrossSalary,
    Guid? DepartmentId,
    string? DepartmentName,
    string? EmployementType,
    DateOnly? JoiningDate,
    DateOnly? ConfirmationDate,
    DateOnly? SeparationDate,
    Guid? ReferedBy,
    string? HowYouKnow,
    string? OfficialEmail,
    bool IsActive,
    bool IsDesignationEditable = false
    );

#endregion
#region --- HANDLER ---
internal sealed class GetEmployeeOfficialByIdQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetEmployeeOfficialByIdQuery, Result<GetOfficialResponse>>
{
    public async Task<Result<GetOfficialResponse>> Handle(GetEmployeeOfficialByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await context
            .TblEmployees
            .Include(e => e.OrganizationUnit)
            .Include(e => e.Company)
            //.Include(e => e.Department)
            .Include(e => e.TblEmployeeGrossSalaries)
            .Include(e => e.TblEmployeeDesignations)
            .Where(m => m.Id == request.Id)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (employee is null)
        {
            return new RecordNotFoundError($"Employee not found for the given Id");
        }

        Guid designationId = employee.TblEmployeeDesignations.Any(ed => ed.EffectiveTo == null) ?
            employee.TblEmployeeDesignations.Where(ed => ed.EffectiveTo == null).First().DesignationId :
            employee.TblEmployeeDesignations.OrderByDescending(ed => ed.EffectiveFrom).First().DesignationId;

        var employeeDesignationGrade = await context
            .TblDesignationGradeMappings.Include(e => e.Grade)
            .Where(m => m.DesignationId == designationId)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        decimal? grossSalary = employee.TblEmployeeGrossSalaries.Any(eg => eg.EffectiveTill == null) ?
            employee.TblEmployeeGrossSalaries.Where(eg => eg.EffectiveTill == null).First().GrossSalary :
            employee.TblEmployeeGrossSalaries.OrderByDescending(g => g.EffectiveFrom).FirstOrDefault()?.GrossSalary;

        return Result.Ok(new GetOfficialResponse(
            Id: employee.Id,
            Code: employee.Code,
            OrganizationUnitTypeId: employee.OrganizationUnit?.OrganizationUnitTypeId,
            OrganizationUnitId: employee.OrganizationUnitId,
            CompanyId: employee.CompanyId,
            CompanyName: employee.Company?.Name,
            DesignationId: designationId,
            //OrganizationUnitName: employee.OrganizationUnit?.Name,
            SalaryGrade: employeeDesignationGrade?.Grade?.Name,
            GrossSalary: grossSalary,
            DepartmentId: employee.DepartmentId,
            DepartmentName: employee.Department?.Name,
            EmployementType: employee.EmployementType,
            JoiningDate: employee.JoiningDate,
            ConfirmationDate: employee.ConfirmationDate,
            SeparationDate: employee.SeparationDate,
            ReferedBy: employee.ReferedBy,
            HowYouKnow: employee.HowYouKnow,
            OfficialEmail: employee.OfficialEmail,
            IsActive: employee.IsActive,
            IsDesignationEditable: !employee.TblEmployeeDesignations.Any()
        ));
    }
}
#endregion