using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Hrms.Application.AttendanceMoralization.Models;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.AttendanceMoralization.Queries;

#region --- QUERY ---
public record GetAllByMonthAndYearQuery(MoralizationSearch SearchParam) : IRequest<GetAllByMonthAndYearQueryResponse>;
#endregion

#region --- RESPONSE ---
public record GetAllByMonthAndYearQueryResponse(IReadOnlyList<EmployeeWiseCalculationResponse> Employees, int TotalRecords);
#endregion
#region --- VALIDATOR ---
public class GetAllByMonthAndYearQueryValidator : AbstractValidator<GetAllByMonthAndYearQuery>
{
    public GetAllByMonthAndYearQueryValidator()
    {
        RuleFor(x => x.SearchParam.Month).NotEmpty().WithMessage("Month is required.").InclusiveBetween(1, 12).WithMessage("Month must be between 1 and 12.");
        RuleFor(x => x.SearchParam.Year).NotEmpty().WithMessage("Year is required.").GreaterThanOrEqualTo(2000).WithMessage("Year must be greater than or equal to 2000.");
        RuleFor(x => x.SearchParam.PageIndex).GreaterThanOrEqualTo(0).WithMessage("PageIndex must be greater than or equal to 0.");
        RuleFor(x => x.SearchParam.PageSize).GreaterThan(0).WithMessage("PageSize must be greater than 0.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class GetAllByMonthAndYearQueryHandler(QubeFinDataContext context) :
        IRequestHandler<GetAllByMonthAndYearQuery, GetAllByMonthAndYearQueryResponse>
{
    public async Task<GetAllByMonthAndYearQueryResponse> Handle(GetAllByMonthAndYearQuery request, CancellationToken cancellationToken)
    {
        var skipRecordCount = request.SearchParam.PageIndex * request.SearchParam.PageSize;
        var filterEntitiesQuery = context.TblEmployeeLops
            .Include(e => e.TblEmployeeLopDetails)
            .Include(e => e.OrganizationUnit)
            .ThenInclude(e => e.Company)
            .Include(e => e.Employee)
        .AsNoTracking().AsQueryable();

        if (request.SearchParam.CompanyId != null && request.SearchParam.CompanyId != Guid.Empty)
        {
            filterEntitiesQuery = filterEntitiesQuery.Where(m => m.OrganizationUnit.CompanyId == request.SearchParam.CompanyId);
        }
        if (request.SearchParam.SearchOrganizationUnitId != null && request.SearchParam.SearchOrganizationUnitId != Guid.Empty)
        {
            filterEntitiesQuery = filterEntitiesQuery.Where(m => m.OrganizationUnitId == request.SearchParam.SearchOrganizationUnitId);
        }
        if (request.SearchParam.EmployeeId != null && request.SearchParam.EmployeeId != Guid.Empty)
        {
            filterEntitiesQuery = filterEntitiesQuery.Where(m => m.EmployeeId == request.SearchParam.EmployeeId);
        }
        if (!string.IsNullOrEmpty(request.SearchParam.SearchText))
        {
            filterEntitiesQuery = filterEntitiesQuery.Where(m => m.Employee.Code!.Contains(request.SearchParam.SearchText.Trim())
            || m.Employee.FullName.Contains(request.SearchParam.SearchText.Trim())
            || m.Employee.OfficialEmail!.Contains(request.SearchParam.SearchText.Trim()));
        }
        if (request.SearchParam.Status > 0)
        {
            filterEntitiesQuery = request.SearchParam.Status switch
            {
                1 => filterEntitiesQuery.Where(m => m.AttendanceIrregularDays > 0),
                2 => filterEntitiesQuery.Where(m => (m.IrregularLopDays + (m.TblEmployeeLopDetails.Where(l => l.PayrollStatus == "LOP").Count())) > 0),
                _ => filterEntitiesQuery
            };
        }

        if (request.SearchParam.SortOn is not null && request.SearchParam.SortDirection is not null)
        {
            filterEntitiesQuery = request.SearchParam.SortOn switch
            {
                "code" => request.SearchParam.SortDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? filterEntitiesQuery.OrderByDescending(m => m.Employee.Code) : filterEntitiesQuery.OrderBy(m => m.Employee.Code),
                "name" => request.SearchParam.SortDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? filterEntitiesQuery.OrderByDescending(m => m.Employee.FullName) : filterEntitiesQuery.OrderBy(m => m.Employee.FullName),
                "company" => request.SearchParam.SortDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? filterEntitiesQuery.OrderByDescending(m => m.OrganizationUnit.Company.Name) : filterEntitiesQuery.OrderBy(m => m.OrganizationUnit.Company.Name),
                "organizationUnit" => request.SearchParam.SortDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? filterEntitiesQuery.OrderByDescending(m => m.OrganizationUnit.Name) : filterEntitiesQuery.OrderBy(m => m.OrganizationUnit.Name),
                _ => request.SearchParam.SortDirection == "DESC" ? filterEntitiesQuery.OrderByDescending(m => m.Employee.Code) : filterEntitiesQuery.OrderBy(m => m.Employee.Code),
            };
        }

        var totalCount = await filterEntitiesQuery.CountAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        var employees = await filterEntitiesQuery.Skip(skipRecordCount).Take(request.SearchParam.PageSize)
            .Select(m => new EmployeeWiseCalculationResponse
            {
                Id = m.Id,
                EmployeeId = m.EmployeeId,
                EmployeeCode = m.Employee.Code,
                EmployeeName = m.Employee.FullName,
                OrganizationUnitId = m.OrganizationUnitId,
                OrganizationUnitName = m.OrganizationUnit.Name,
                CompanyName = m.OrganizationUnit.Company != null ? m.OrganizationUnit.Company.Name : null,
                HoliDays = m.HoliDays,
                WorkingDays = m.WorkingDays,
                LeaveDays = m.LeaveDays,
                AttendanceDays = m.AttendanceDays,
                AbsentDays = m.AbsentDays,
                AttendanceIrregularDays = m.AttendanceIrregularDays,
                IrregularLopDays = m.IrregularLopDays + (m.TblEmployeeLopDetails.Where(l => l.PayrollStatus == "LOP").Count()),
                IsLocked = m.IsLocked,
                Remarks = m.Remarks
            }).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        return new GetAllByMonthAndYearQueryResponse(employees, totalCount);
    }
}
#endregion

