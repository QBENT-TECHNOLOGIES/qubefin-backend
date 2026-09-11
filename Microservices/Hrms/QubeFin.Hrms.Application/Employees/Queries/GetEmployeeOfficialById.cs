using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Hrms.Application.Employees.Queries;

#region --- QUERY ---
public record GetEmployeeOfficialByIdQuery(Guid Id) : IRequest<Result<GetOfficialResponse>>;
#endregion
#region --- RESPONSE ---
public record GetOfficialResponse(
    Guid Id,
    string Code,
    Guid? OrganizationUnitTypeId,
    string? OrganizationUnitTypeName,
    Guid? OrganizationUnitId,
    string? OrganizationUnitName,
    Guid? CompanyId,
    string? CompanyName,
    Guid? DesignationId,
    string? DesignationName,
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
            .Include(e => e.OrganizationUnit).ThenInclude(e => e.OrganizationUnitType)
            .Include(e => e.Company)
            //.Include(e => e.Department)
            .Include(e => e.TblEmployeeGrossSalaries)
            .Include(e => e.TblEmployeeDesignations).ThenInclude(e => e.Designation)
            .Where(m => m.Id == request.Id)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (employee is null)
        {
            return new RecordNotFoundError($"Employee not found for the given Id");
        }

        var designation = !employee.TblEmployeeDesignations.Any() ? null :
            employee.TblEmployeeDesignations.Any(ed => ed.EffectiveTo == null) ?
            employee.TblEmployeeDesignations.Where(ed => ed.EffectiveTo == null).First() :
            employee.TblEmployeeDesignations.OrderByDescending(ed => ed.EffectiveFrom).First();

        var employeeDesignationGrade = designation == null ? null : await context
            .TblDesignationGradeMappings.Include(e => e.Grade)
            .Where(m => m.DesignationId == designation.DesignationId)
            .AsNoTracking()
            .ToListAsync(cancellationToken: cancellationToken);

        string? salaryGrade = employeeDesignationGrade == null ? null :
            employeeDesignationGrade.Any(dg => dg.IsActive) ?
            employeeDesignationGrade.First(dg => dg.IsActive).Grade?.Name :
            employeeDesignationGrade.FirstOrDefault()?.Grade?.Name;

        decimal? grossSalary = !employee.TblEmployeeGrossSalaries.Any() ? null :
            employee.TblEmployeeGrossSalaries.Any(eg => eg.EffectiveTill == null) ?
            employee.TblEmployeeGrossSalaries.Where(eg => eg.EffectiveTill == null).First()?.GrossSalary :
            employee.TblEmployeeGrossSalaries.OrderByDescending(g => g.EffectiveFrom).FirstOrDefault()?.GrossSalary;

        return Result.Ok(new GetOfficialResponse(
            Id: employee.Id,
            Code: employee.Code,
            OrganizationUnitTypeId: employee.OrganizationUnit?.OrganizationUnitTypeId,
            OrganizationUnitTypeName: employee.OrganizationUnit?.OrganizationUnitType?.Name,
            OrganizationUnitId: employee.OrganizationUnitId,
            OrganizationUnitName: employee.OrganizationUnit?.Name,
            CompanyId: employee.CompanyId,
            CompanyName: employee.Company?.Name,
            DesignationId: designation?.DesignationId,
            DesignationName: designation?.Designation?.Name,
            SalaryGrade: salaryGrade,
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