using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.Employees.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;

namespace QubeFin.Hrms.Application.Employees.Commands
{

    #region --- COMMAND ---
    public record UpdateEmployeeReferenceCommand(
        Guid Id, IReadOnlyList<ReferenceDetailRequest> ReferenceDetail, Guid LastModifiedBy
        ) : IRequest<Result<string>>;
    #endregion

    #region --- VALIDATION ---
    public class UpdateEmployeeReferenceCommandValidator : AbstractValidator<UpdateEmployeeReferenceCommand>
    {

        public UpdateEmployeeReferenceCommandValidator()
        {
            RuleForEach(x => x.ReferenceDetail).ChildRules(reference =>
            {
                reference.RuleFor(r => r.Mobile)
                    .NotEmpty().WithMessage("Mobile number is required.")
                    .Matches(@"^[6-9]\d{9}$").WithMessage("Enter a valid 10-digit mobile number.");
                reference.RuleFor(r => r.Email)
                    .NotEmpty().WithMessage("Email is required.")
                    .Matches(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")
            .When(r => !string.IsNullOrWhiteSpace(r.Email))
            .WithMessage("Enter a valid email address.");
            });

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

        }
    }
    #endregion

    #region --- HANDLER ---
    internal sealed class UpdateEmployeeReferenceCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork, QubeFinDataContext context)
        : IRequestHandler<UpdateEmployeeReferenceCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(UpdateEmployeeReferenceCommand request, CancellationToken cancellationToken)
        {
            var existingEmployee = await employeeRepository.GetByIdAsync(request.Id);
            if (existingEmployee == null)
            {
                return new ValidationError("Employee not exist given id.");
            }
            // 2. Project incoming requests directly into domain entity shapes
            var referenceEntityList = new List<TblEmployeeReference>();

            for (int i = 0; i < request.ReferenceDetail.Count; i++)
            {
                var req = request.ReferenceDetail[i];
                //int sequenceValue = i + 1;

                var referenceEntity = new TblEmployeeReference()
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = request.Id,
                    PersonName = req.PersonName,
                    Mobile = req.Mobile,
                    Email = req.Email,
                    Address = req.Address,
                    Occupation = req.Occupation,
                    HowDoYouKnow = req.HowDoYouKnow,
                };


                referenceEntityList.Add(referenceEntity);
            }

            var referers = await context.TblEmployeeReferences.Where(m => m.EmployeeId == request.Id).ToListAsync(cancellationToken: cancellationToken);
            if (referers != null && referers.Count() > 0)
            {
                context.TblEmployeeReferences.RemoveRange(referers);
            }
            context.TblEmployeeReferences.AddRange(referenceEntityList);
            existingEmployee.SetModified(request.LastModifiedBy);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok($"Employee reference information updated successfully for Name : {existingEmployee.PersonalInfo.FirstName} {existingEmployee.PersonalInfo.LastName}");
        }
    }
    #endregion
}
