using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.Employees.Models;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.Employees.Commands;

#region --- COMMAND ---
public record UpdateEmployeeReferralCommand(Guid Id, ReferralInfoRequest ReferralInfo, Guid UserId) : IRequest<Result<string>>;
#endregion

#region --- VALIDATION ---
public class UpdateEmployeeReferralCommandValidator : AbstractValidator<UpdateEmployeeReferralCommand>
{
    public UpdateEmployeeReferralCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Employee Id is required.");
        RuleFor(x => x.ReferralInfo).NotNull().WithMessage("Referral Info is required.");
        RuleFor(x => x.UserId).NotEmpty().WithMessage("Login User Id is required.");
        RuleFor(x => x)
            .Must(x => !x.ReferralInfo.ReferedBy.HasValue || x.ReferralInfo.ReferedBy.Value != x.Id)
            .WithMessage("An employee cannot be referred by themselves.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class UpdateEmployeeReferralCommandHandler(QubeFinDataContext context, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateEmployeeReferralCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateEmployeeReferralCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var employeeEntity = await context.TblEmployees
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken: cancellationToken);
            if (employeeEntity is null)
            {
                return new ValidationError("Employee does not exist with the given id.");
            }

            if (request.ReferralInfo.ReferedBy.HasValue)
            {
                var referrerExists = await context.TblEmployees
                    .AsNoTracking()
                    .AnyAsync(m => m.Id == request.ReferralInfo.ReferedBy.Value, cancellationToken: cancellationToken);
                if (!referrerExists)
                {
                    return new ValidationError("Referred-by employee does not exist with the given id.");
                }
            }

            employeeEntity.ReferedBy = request.ReferralInfo.ReferedBy;
            employeeEntity.HowYouKnow = request.ReferralInfo.HowYouKnow;

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok($"Employee referral information updated successfully for Name : {employeeEntity.FullName}");
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error($"{ex.Message}"));
        }
    }
}
#endregion
