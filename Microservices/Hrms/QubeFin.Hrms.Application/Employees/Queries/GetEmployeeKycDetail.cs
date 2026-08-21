using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.Employees.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.Employees.Queries;

#region --- QUERY ---
public record GetEmployeeKycDetailQuery(Guid Id) : IRequest<Result<List<DocumentDetailRequest>>>;
#endregion
#region --- HANDLER ---
internal sealed class GetEmployeeKycDetailQueryHandler(QubeFinDataContext context, IFileStorageRepository fileStorageRepository)
    : IRequestHandler<GetEmployeeKycDetailQuery, Result<List<DocumentDetailRequest>>>
{
    public async Task<Result<List<DocumentDetailRequest>>> Handle(GetEmployeeKycDetailQuery request, CancellationToken cancellationToken)
    {
        var employee = await context.TblEmployees.Include(m => m.TblEmployeeDocuments).Where(m => m.Id == request.Id).FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (employee is null)
        {
            return new RecordNotFoundError($"Employee not found for the given Id");
        }
        var employeeKycDocuments = employee.TblEmployeeDocuments.Where(m => m.DocumentCategory == "KYC").ToList();
        var docs = new List<DocumentDetailRequest>();

        foreach (var d in employeeKycDocuments)
        {

            var fileUrl = !string.IsNullOrEmpty(d.FileNo)
                ? await fileStorageRepository.GetFileUrlAsync(d.FileNo, cancellationToken)
                : null;

            docs.Add(new DocumentDetailRequest
            {
                Id = d.Id,
                DocumentCategory = d.DocumentCategory,
                DocumentName = d.DocumentName,
                DocumentNo = d.DocumentNo,
                ValidFrom = d.ValidFrom?.ToDateTime(TimeOnly.MinValue),
                ValidTill = d.ValidTill?.ToDateTime(TimeOnly.MinValue),
                FileName = d.FileName,
                FileNo = d.FileNo,
                EmployeeId = d.EmployeeId,
                FileUrl = fileUrl
            });
        }

        return Result.Ok(docs.OrderBy(m => m.DocumentName).ToList());
    }
}
#endregion