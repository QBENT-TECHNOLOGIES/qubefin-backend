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
public record GetEmployeeTransferHistoryQuery(Guid Id) : IRequest<Result<GetEmployeeTransferHistoryResponse>>;
#endregion


#region --- VALIDATION ---
public class GetEmployeeTransferHistoryQueryValidator : AbstractValidator<GetEmployeeTransferHistoryQuery>
{
    public GetEmployeeTransferHistoryQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Employee is required.");
    }
}
#endregion
#region --- RESPONSE ---
public record GetEmployeeTransferHistoryResponse(List<EmployeeTransferHistoryResponse> EmployeeTransferHistory, EmployeeCurrentOfficialInfoResponse CurrentOfficialInfo);

#endregion
#region --- HANDLER ---
internal sealed class GetEmployeeTransferHistoryQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetEmployeeTransferHistoryQuery, Result<GetEmployeeTransferHistoryResponse>>
{
    public async Task<Result<GetEmployeeTransferHistoryResponse>> Handle(GetEmployeeTransferHistoryQuery request, CancellationToken cancellationToken)
    {
        List<EmployeeTransferHistoryResponse> EmployeeTransferHistory = new List<EmployeeTransferHistoryResponse>();
        EmployeeCurrentOfficialInfoResponse CurrentOfficialInfo = new EmployeeCurrentOfficialInfoResponse();

        var employee = await context.TblEmployees
            .Include(e => e.OrganizationUnit).ThenInclude(e => e.OrganizationUnitType)
            .Include(e => e.TblEmployeeGrossSalaries)
            .Include(e => e.TblEmployeeDesignations)
            .Where(m => m.Id == request.Id)
            .AsNoTracking().FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (employee is null)
        {
            return new RecordNotFoundError($"Employee not found for the given Id");
        }

        Guid? designationId = !employee.TblEmployeeDesignations.Any() ? null :
            employee.TblEmployeeDesignations.Any(ed => ed.EffectiveTo == null) ?
            employee.TblEmployeeDesignations.Where(ed => ed.EffectiveTo == null).First()?.DesignationId :
            employee.TblEmployeeDesignations.OrderByDescending(ed => ed.EffectiveFrom).First()?.DesignationId;

        decimal? grossSalary = !employee.TblEmployeeGrossSalaries.Any() ? null :
            employee.TblEmployeeGrossSalaries.Any(eg => eg.EffectiveTill == null) ?
            employee.TblEmployeeGrossSalaries.Where(eg => eg.EffectiveTill == null).First()?.GrossSalary :
            employee.TblEmployeeGrossSalaries.OrderByDescending(g => g.EffectiveFrom).FirstOrDefault()?.GrossSalary;


        CurrentOfficialInfo = new EmployeeCurrentOfficialInfoResponse
        {
            OrganisationUnitTypeId = employee.OrganizationUnit?.OrganizationUnitTypeId,
            OrganisationUnitId = employee.OrganizationUnitId,
            DesignationId = designationId,
            GrossSalary = grossSalary
        };

        var transferHistory = await context.TblEmployeeTransfers.Include(m => m.OrganisationUnit).ThenInclude(m => m.OrganizationUnitType)
                            .Include(m => m.Designation).ThenInclude(d => d.TblDesignationGradeMappings).ThenInclude(m => m.Grade).AsNoTracking().Where(m => m.EmployeeId == request.Id).ToListAsync(cancellationToken);



        EmployeeTransferHistory = transferHistory.Select(m => new EmployeeTransferHistoryResponse
        {
            Id = m.Id,
            OrganisationUnit = m.OrganisationUnit.Name,
            OrganisationUnitType = m.OrganisationUnit.OrganizationUnitType.Name,
            Designation = m.Designation.Name,
            SalaryGrade = m.Designation.TblDesignationGradeMappings.FirstOrDefault()?.Grade.Name,
            GrossSalary = m.GrossSalary,
        }).ToList();
        
        return Result.Ok(new GetEmployeeTransferHistoryResponse(EmployeeTransferHistory, CurrentOfficialInfo));
    }
}
#endregion
