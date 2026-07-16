using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.Employees.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.Hrms;
using QubeFin.Persistence.Models.App;
using QubeFin.Persistence.Models.Hrms;
using System.Text.RegularExpressions;

namespace QubeFin.Hrms.Application.Employees.Commands
{

    #region --- COMMAND ---
    public record UpdateEmployeeReferenceCommand(
        Guid Id, IReadOnlyList<ReferenceDetailRequest> ReferenceDetail, Guid LastModifiedBy
        ) : IRequest<Result<UpdateEmployeeReferenceResponse>>;
    #endregion
    #region --- VALIDATION ---
    public class UpdateEmployeeReferenceCommandValidator : AbstractValidator<UpdateEmployeeReferenceCommand>
    {
        //public UpdateEmployeeReferenceCommandValidator()
        //{
        //    RuleFor(x => x.FirstName)
        //        .Must(value => !string.IsNullOrWhiteSpace(value)
        //            && Regex.IsMatch(value, @"^[A-Za-z]+$")
        //            && !value.Equals("Select", StringComparison.OrdinalIgnoreCase))
        //        .WithMessage("Please enter a valid First Name name.")
        //        .MinimumLength(3).WithMessage("First Name must be more than 2 characters.")
        //        .MaximumLength(30).WithMessage("First Name cannot exceed 30 characters.");
        //    RuleFor(x => x.LastName)
        //        .NotEmpty()
        //        .Matches("^[A-Za-z]{3,30}$")
        //        .WithMessage("Last name must contain only letters and be between 3 and 30 characters long.");
            
        //}
    }
    #endregion

    #region --- RESPONSE ---
    public record UpdateEmployeeReferenceResponse(bool Created);
    #endregion

    #region --- HANDLER ---
    internal sealed class UpdateEmployeeReferenceCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork, QubeFinDataContext context)
        : IRequestHandler<UpdateEmployeeReferenceCommand, Result<UpdateEmployeeReferenceResponse>>
    {
        public async Task<Result<UpdateEmployeeReferenceResponse>> Handle(UpdateEmployeeReferenceCommand request, CancellationToken cancellationToken)
        {
            var existingEmployee = await employeeRepository.GetByIdAsync(request.Id);
            if (existingEmployee == null)
            {
                return new ValidationError("Employee not exist given id.");
            }
            // 2. Project incoming requests directly into domain entity shapes
            var updatedReferenceList = new List<EmployeeReference>();

            for (int i = 0; i < request.ReferenceDetail.Count; i++)
            {
                var req = request.ReferenceDetail[i];
                //int sequenceValue = i + 1;

                var referenceEntity = new EmployeeReference(
                    Guid.NewGuid(),
                    request.Id,
                    req.PersonName,
                    req.Mobile,
                    req.Email,
                    req.Address,
                    req.Occupation,
                    req.HowDoYouKnow
                );
                updatedReferenceList.Add(referenceEntity);
            }

            // 3. Atomically overwrite old items and explicitly log modifications
            existingEmployee.ReplaceReferences(updatedReferenceList);

            await employeeRepository.UpdateAsync(existingEmployee);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok(new UpdateEmployeeReferenceResponse(true));


        }
    }
    #endregion
}
