using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.Employees.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Employees.Queries;

#region --- QUERY ---
public record GetEmployeeEmploymentQuery(Guid Id) : IRequest<Result<GetEmploymentResponse>>;
#endregion
#region --- RESPONSE ---
public record GetEmploymentResponse(
    List<EmploymentDetailRequest> Employments
    );

#endregion
#region --- HANDLER ---
internal sealed class GetEmployeeEmploymentQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetEmployeeEmploymentQuery, Result<GetEmploymentResponse>>
{
    public async Task<Result<GetEmploymentResponse>> Handle(GetEmployeeEmploymentQuery request, CancellationToken cancellationToken)
    {
        var employee = await context.TblEmployees.Include(m => m.TblEmployeeEmployments).Where(m => m.Id == request.Id).FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (employee is null)
        {
            return new RecordNotFoundError($"Employee not found for the given Id");
        }
        var employeeEmployment = employee.TblEmployeeEmployments.ToList();
        var docs = employeeEmployment.Any() ? [.. employeeEmployment.Select(d => new EmploymentDetailRequest()
        {
            Id = d.Id,
            EmployeeId = d.EmployeeId,
            EmployerName = d.EmployerName,
            Designation = d.Designation,
            FromDate = d.FromDate.ToDateTime(TimeOnly.MinValue),
            ToDate = d.ToDate.ToDateTime(TimeOnly.MinValue),
            LastDrawnSalary = d.LastDrawnSalary,
            JobTitle = d.JobTitle,
            NocFileName = d.NocFileName,
            NocFileNo = d.NocFileNo,
            ExpCertFileName = d.ExpCertFileName,
            ExpCertFileNo = d.ExpCertFileNo,
            Sequence = d.Sequence
        })]

                    : new List<EmploymentDetailRequest>();
        return Result.Ok(new GetEmploymentResponse(docs.OrderBy(m => m.Sequence).ToList()));
    }
}
#endregion