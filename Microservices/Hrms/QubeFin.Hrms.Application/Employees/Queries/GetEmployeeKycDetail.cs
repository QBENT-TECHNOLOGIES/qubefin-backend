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
public record GetEmployeeKycDetailQuery(Guid Id) : IRequest<Result<GetKycResponse>>;
#endregion
#region --- RESPONSE ---
public record GetKycResponse(
    List<DocumentDetailRequest> Documents
    );

#endregion
#region --- HANDLER ---
internal sealed class GetEmployeeKycDetailQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetEmployeeKycDetailQuery, Result<GetKycResponse>>
{
    public async Task<Result<GetKycResponse>> Handle(GetEmployeeKycDetailQuery request, CancellationToken cancellationToken)
    {
        var employee = await context.TblEmployees.Include(m => m.TblEmployeeDocuments).Where(m => m.Id == request.Id).FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (employee is null)
        {
            return new RecordNotFoundError($"Employee not found for the given Id");
        }
        var employeeKycDocuments = employee.TblEmployeeDocuments.Where(m => m.DocumentCategory == "KYC").ToList();
        var docs = employeeKycDocuments.Any() ? [.. employeeKycDocuments.Select(d => new DocumentDetailRequest()
        {
            Id = d.Id,
            DocumentCategory = d.DocumentCategory,
            DocumentName = d.DocumentName,
            DocumentNo = d.DocumentNo,
            ValidFrom = d.ValidFrom == null ? null :d.ValidFrom?.ToDateTime(TimeOnly.MinValue),
            ValidTill = d.ValidTill == null ? null :d.ValidTill?.ToDateTime(TimeOnly.MinValue),
            FileName = d.FileName,
            FileNo = d.FileNo,
            EmployeeId = d.EmployeeId
        }).OrderBy(m => m.DocumentName)]

                    : new List<DocumentDetailRequest>();
        return Result.Ok(new GetKycResponse(docs));
    }
}
#endregion