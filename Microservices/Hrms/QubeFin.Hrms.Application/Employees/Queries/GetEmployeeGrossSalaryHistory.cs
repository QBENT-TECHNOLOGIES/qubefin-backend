using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.Employees.Commands;
using QubeFin.Hrms.Application.Employees.Models;
using QubeFin.Persistence;
namespace QubeFin.Hrms.Application.Employees.Queries;

#region --- QUERY ---
public record GetEmployeeGrossSalaryHistoryQuery(Guid Id) : IRequest<Result<GetEmployeeGrossSalaryHistoryResponse>>;
#endregion


#region --- VALIDATION ---
public class GetEmployeeGrossSalaryHistoryQueryValidator : AbstractValidator<GetEmployeeGrossSalaryHistoryQuery>
{
    public GetEmployeeGrossSalaryHistoryQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Employee is required.");
    }
}
#endregion

#region --- RESPONSE ---
public record GetEmployeeGrossSalaryHistoryResponse(List<EmployeeGrossSalaryHistoryResponse> EmployeeGrossSalaryHistory, EmployeeCurrentGrossSalaryResponse CurrentGrossSalary);

#endregion

#region --- HANDLER ---
internal sealed class GetEmployeeGrossSalaryHistoryQueryHandler(QubeFinDataContext context) : IRequestHandler<GetEmployeeGrossSalaryHistoryQuery, Result<GetEmployeeGrossSalaryHistoryResponse>>
{
    public async Task<Result<GetEmployeeGrossSalaryHistoryResponse>> Handle(GetEmployeeGrossSalaryHistoryQuery request, CancellationToken cancellationToken)
    {
        List<EmployeeGrossSalaryHistoryResponse> EmployeeGrossSalaryHistory = new List<EmployeeGrossSalaryHistoryResponse>();
        EmployeeCurrentGrossSalaryResponse CurrentGrossSalary = new EmployeeCurrentGrossSalaryResponse();

        var employee = await context.TblEmployees
            .Include(e => e.TblEmployeeGrossSalaries)
            .Include(e => e.TblEmployeeDesignations)
            .Where(m => m.Id == request.Id)
            .AsNoTracking().FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (employee is null)
        {
            return new RecordNotFoundError($"Employee not found for the given Id");
        }

        // Employee last designation -> active designation grade mapping
        Guid? designationId = !employee.TblEmployeeDesignations.Any() ? null :
            employee.TblEmployeeDesignations.Any(ed => ed.EffectiveTo == null) ?
            employee.TblEmployeeDesignations.Where(ed => ed.EffectiveTo == null).First()?.DesignationId :
            employee.TblEmployeeDesignations.OrderByDescending(ed => ed.EffectiveFrom).First()?.DesignationId;

        var employeeDesignationGrade = designationId == null ? null : await context
            .TblDesignationGradeMappings
            .Where(m => m.DesignationId == designationId)
            .AsNoTracking()
            .ToListAsync(cancellationToken: cancellationToken);

        Guid? salaryGradeId = employeeDesignationGrade == null ? null :
            employeeDesignationGrade.Any(dg => dg.IsActive) ?
            employeeDesignationGrade.First(dg => dg.IsActive).GradeId :
            employeeDesignationGrade.FirstOrDefault()?.GradeId;

        var grossSalaries = employee.TblEmployeeGrossSalaries.ToList();

        var currentGrossSalary = !grossSalaries.Any() ? null :
            grossSalaries.Any(gs => gs.EffectiveTill == null) ?
            grossSalaries.Where(gs => gs.EffectiveTill == null).First() :
            grossSalaries.OrderByDescending(gs => gs.EffectiveFrom).First();

        CurrentGrossSalary = new EmployeeCurrentGrossSalaryResponse
        {
            Id = currentGrossSalary?.Id,
            SalaryGradeId = salaryGradeId,
            GrossSalary = currentGrossSalary?.GrossSalary,
            //PfAmount = currentGrossSalary?.PfAmount,
            EffectiveFrom = currentGrossSalary?.EffectiveFrom,
            EffectiveTill = currentGrossSalary?.EffectiveTill
        };

        // Tbl_EmployeeGrossSalary holds no grade per row, so every row carries the current salary grade.
        EmployeeGrossSalaryHistory = grossSalaries.Select(m => new EmployeeGrossSalaryHistoryResponse
        {
            Id = m.Id,
            SalaryGradeId = salaryGradeId,
            GrossSalary = m.GrossSalary,
            EffectiveFrom = m.EffectiveFrom,
            EffectiveTill = m.EffectiveTill
        }).OrderByDescending(m => m.EffectiveFrom).ToList();

        return Result.Ok(new GetEmployeeGrossSalaryHistoryResponse(EmployeeGrossSalaryHistory, CurrentGrossSalary));
    }
}
#endregion
