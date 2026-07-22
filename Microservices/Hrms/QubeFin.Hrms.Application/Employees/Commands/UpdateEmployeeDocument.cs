using Azure.Core;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.Employees.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Mappers.Hrms;
using QubeFin.Persistence.Models.App;
using QubeFin.Persistence.Models.Hrms;
using System.Text.RegularExpressions;

namespace QubeFin.Hrms.Application.Employees.Commands
{

    #region --- COMMAND ---
    public record UpdateEmployeeDocumentCommand(
        Guid Id, List<DocumentDetailRequest> Documents, Guid LastModifiedBy
        ) : IRequest<Result<UpdateEmployeeDocumentResponse>>;
    #endregion
    #region --- VALIDATION ---
    //public class UpdateEmployeeDocumentCommandValidator : AbstractValidator<UpdateEmployeeDocumentCommand>
    //{
    //    public UpdateEmployeeDocumentCommandValidator()
    //    {
    //        RuleFor(x => x.FirstName)
    //            .Must(value => !string.IsNullOrWhiteSpace(value)
    //                && Regex.IsMatch(value, @"^[A-Za-z]+$")
    //                && !value.Equals("Select", StringComparison.OrdinalIgnoreCase))
    //            .WithMessage("Please enter a valid First Name name.")
    //            .MinimumLength(3).WithMessage("First Name must be more than 2 characters.")
    //            .MaximumLength(30).WithMessage("First Name cannot exceed 30 characters.");
    //        RuleFor(x => x.LastName)
    //            .NotEmpty()
    //            .Matches("^[A-Za-z]{3,30}$")
    //            .WithMessage("Last name must contain only letters and be between 3 and 30 characters long.");
            
    //    }
    //}
    #endregion

    #region --- RESPONSE ---
    public record UpdateEmployeeDocumentResponse(bool Created);
    #endregion

    #region --- HANDLER ---
    internal sealed class UpdateEmployeeDocumentCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork, QubeFinDataContext context)
        : IRequestHandler<UpdateEmployeeDocumentCommand, Result<UpdateEmployeeDocumentResponse>>
    {
        public async Task<Result<UpdateEmployeeDocumentResponse>> Handle(UpdateEmployeeDocumentCommand request, CancellationToken cancellationToken)
        {
            var existingEmployee = await employeeRepository.GetByIdAsync(request.Id);
            if (existingEmployee == null)
            {
                return new ValidationError("Employee not exist with given id.");
            }
            // 2. Project incoming requests directly into domain entity shapes
            var updatedDocumentEntityList = new List<TblEmployeeDocument>();

            for (int i = 0; i < request.Documents.Count; i++)
            {
                var req = request.Documents[i];
                //int sequenceValue = i + 1;

                var documentEntity = new TblEmployeeDocument()
                {
                    Id = Guid.NewGuid(),
                    DocumentCategory = "KYC",
                    DocumentName = req.DocumentName,
                    DocumentNo = req.DocumentNo,
                    ValidFrom = req.ValidFrom != null ? DateOnly.FromDateTime(req.ValidFrom.Value) : null,
                    ValidTill = req.ValidTill != null ? DateOnly.FromDateTime(req.ValidTill.Value) : null,
                    FileName = req.FileName,
                    FileNo = req.FileNo,
                    EmployeeId = request.Id,
                    UploadedBy = request.LastModifiedBy,
                    UploadedOn = DateTime.Now
                };
                    
                
                updatedDocumentEntityList.Add(documentEntity);
            }

            var docs = await context.TblEmployeeDocuments.Where(m => m.EmployeeId == request.Id && m.DocumentCategory == "KYC").ToListAsync(cancellationToken: cancellationToken);
            if (docs != null && docs.Count() > 0)
            {
                 context.TblEmployeeDocuments.RemoveRange(docs);
            }
            context.TblEmployeeDocuments.AddRange(updatedDocumentEntityList);
            existingEmployee.SetModified(request.LastModifiedBy);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok(new UpdateEmployeeDocumentResponse(true));


        }
    }
    #endregion
}
