using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.Employees.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.Employees.Queries;

#region --- QUERY ---
public record GetEmployeeEmploymentQuery(Guid Id) : IRequest<Result<List<EmploymentDetailResponse>>>;
#endregion
#region --- HANDLER ---
internal sealed class GetEmployeeEmploymentQueryHandler(QubeFinDataContext context, IFileStorageRepository fileStorageRepository)
    : IRequestHandler<GetEmployeeEmploymentQuery, Result<List<EmploymentDetailResponse>>>
{
    public async Task<Result<List<EmploymentDetailResponse>>> Handle(GetEmployeeEmploymentQuery request, CancellationToken cancellationToken)
    {
        var employee = await context.TblEmployees.Include(m => m.TblEmployeeEmployments).Where(m => m.Id == request.Id).FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (employee is null)
        {
            return new RecordNotFoundError($"Employee not found for the given Id");
        }
        var employeeEmployment = employee.TblEmployeeEmployments.ToList();
        var employments = (await Task.WhenAll(employeeEmployment.Select(async d => new EmploymentDetailResponse
        {
            Id = d.Id,
            EmployeeId = d.EmployeeId,
            EmployerName = d.EmployerName,
            Designation = d.Designation,
            FromDate = d.FromDate,
            ToDate = d.ToDate,
            LastDrawnSalary = d.LastDrawnSalary,
            JobTitle = d.JobTitle,
            NocFileName = d.NocFileName,
            NocFileUrl = !string.IsNullOrEmpty(d.NocFileNo)
                ? await fileStorageRepository.GetFileUrlAsync(d.NocFileNo, cancellationToken)
                : null,
            ExpCertFileName = d.ExpCertFileName,
            ExpCertFileUrl = !string.IsNullOrEmpty(d.ExpCertFileNo)
                ? await fileStorageRepository.GetFileUrlAsync(d.ExpCertFileNo, cancellationToken)
                : null,
            Sequence = d.Sequence
        }))).ToList();

        return Result.Ok(employments.OrderBy(m => m.Sequence).ToList());
    }
}
#endregion