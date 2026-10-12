using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.Employees.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Hrms.Application.Employees.Queries;

#region --- QUERY ---
public record GetEmployeeDocumentsByIdQuery(Guid EmployeeId) : IRequest<Result<List<EmployeeDocumentResponse>>>;
#endregion

#region --- VALIDATION ---
public class GetEmployeeDocumentByIdQueryValidator : AbstractValidator<GetEmployeeDocumentsByIdQuery>
{
    public GetEmployeeDocumentByIdQueryValidator()
    {
        RuleFor(x => x.EmployeeId).NotEmpty().WithMessage("Employee is required.");
    }
}
#endregion


#region --- HANDLER ---

internal sealed class GetEmployeeDocumentsByIdQueryHandler(QubeFinDataContext context, IFileStorageRepository fileStorageRepository) : IRequestHandler<GetEmployeeDocumentsByIdQuery, Result<List<EmployeeDocumentResponse>>>
{
    public async Task<Result<List<EmployeeDocumentResponse>>> Handle(GetEmployeeDocumentsByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await context.TblEmployees.Where(m => m.Id == request.EmployeeId).FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (employee == null)
        {
            return new RecordNotFoundError($"Employee Not Found With The Given Id");
        }

        var docs = await context.TblEmployeeDocuments.Where( m => m.EmployeeId == request.EmployeeId).ToListAsync();
        var employeeDocuments = new List<EmployeeDocumentResponse>();

        foreach (var m in docs)
        {
            
               var fileUrl = !string.IsNullOrEmpty(m.FileNo)
              ? await fileStorageRepository.GetFileUrlAsync(m.FileNo, cancellationToken)
              : null;
     
            employeeDocuments.Add(new EmployeeDocumentResponse
            {
                Id = m.Id,
                DocumentName = m.DocumentName,
                FileName = m.FileName,
                FileUrl = fileUrl
            });
        }

        return Result.Ok(employeeDocuments);
    }
}
#endregion