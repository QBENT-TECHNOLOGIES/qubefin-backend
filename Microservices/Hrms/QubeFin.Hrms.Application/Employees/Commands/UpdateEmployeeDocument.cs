using Azure.Core;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.Hrms;
using QubeFin.Persistence.Models.App;
using QubeFin.Persistence.Models.Hrms;
using System.Text.RegularExpressions;

namespace QubeFin.Hrms.Application.Employees.Commands
{

    #region --- COMMAND ---
    public record UpdateEmployeeDocumentCommand(
        Guid Id, List<EmployeeDocument> Documents, Guid LastModifiedBy
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
            var documents = request.Documents.FirstOrDefault();
            if (documents == null)
            {
                return new ValidationError("Employee Document not exist.");
            }

            //employeeRepository.DeleteDocuments(request.Id, documents.DocumentCategory);
            //existingEmployee.UpdateDocuments(
            //    request.Documents,
            //    request.LastModifiedBy
            //    );
            //employeeRepository.UpdateEmployee(existingEmployee);

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok(new UpdateEmployeeDocumentResponse(true));


        }
    }
    #endregion
}
