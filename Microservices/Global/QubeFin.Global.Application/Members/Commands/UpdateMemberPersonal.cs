using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Members;
using System.Text.RegularExpressions;

namespace QubeFin.Global.Application.Members.Commands;

#region --- COMMAND ---
public record UpdateMemberPersonalCommand(
    Guid Id,
    int BranchCode,
    Guid? GroupId,
    string? Salutation,
    string FirstName,
    string? MiddleName,
    string? LastName,
    string? GuardianName,
    string? MotherName,
    string? SpouseName,
    DateOnly DateOfBirth,
    string? Gender,
    string? Religion,
    string? Caste,
    string? Nationality,
    string? MaritalStatus,
    string? EducationalQualification,
    string MobileNo,
    string? AlternateMobileNo,
    Guid UserId
) : IRequest<Result<string>>;
#endregion

#region --- VALIDATION ---
public class UpdateMemberPersonalCommandValidator : AbstractValidator<UpdateMemberPersonalCommand>
{
    public UpdateMemberPersonalCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Member Id is required.");

        RuleFor(x => x.BranchCode)
            .GreaterThan(0)
            .WithMessage("Branch Code is required.");

        RuleFor(x => x.FirstName)
            .Must(value => !string.IsNullOrWhiteSpace(value)
                && Regex.IsMatch(value, @"^[A-Za-z]+$")
                && !value.Equals("Select", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Please enter a valid First Name.")
            .MinimumLength(3).WithMessage("First Name must be more than 2 characters.")
            .MaximumLength(30).WithMessage("First Name cannot exceed 30 characters.");

        RuleFor(x => x.LastName)
            .Matches("^[A-Za-z]{1,30}$")
            .When(x => !string.IsNullOrWhiteSpace(x.LastName))
            .WithMessage("Last Name must contain only letters and cannot exceed 30 characters.");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .LessThan(DateOnly.FromDateTime(DateTime.Now))
            .WithMessage("Date of Birth must be a valid date in the past.");

        RuleFor(x => x.Gender)
            .NotEmpty()
            .Must(value => value == "Male" || value == "Female" || value == "Other")
            .WithMessage("Gender must be either 'Male', 'Female', or 'Other'.");

        RuleFor(x => x.MobileNo)
            .NotEmpty()
            .Matches("^[0-9]{10}$")
            .WithMessage("Mobile Number must be a valid 10-digit number.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class UpdateMemberPersonalCommandHandler(
    IMemberRepository memberRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateMemberPersonalCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateMemberPersonalCommand request, CancellationToken cancellationToken)
    {
        var member = await memberRepository.GetByIdAsync(request.Id);
        if (member is null)
        {
            return new ValidationError("Member does not exist with the given id.");
        }

        member.UpdatePersonalInfo(
            new PersonalInfo(
                request.BranchCode,
                request.GroupId,
                member.Code,
                request.Salutation,
                request.FirstName,
                request.MiddleName,
                request.LastName,
                request.GuardianName,
                request.MotherName,
                request.SpouseName,
                request.DateOfBirth,
                request.Gender,
                request.Religion,
                request.Caste,
                request.Nationality,
                request.MaritalStatus,
                request.EducationalQualification,
                request.MobileNo,
                request.AlternateMobileNo),
            request.UserId);

        await memberRepository.UpdateAsync(member);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var fullName = $"{request.FirstName} {request.LastName}".Trim();
        return Result.Ok($"Member personal information updated successfully for Name : {fullName}");
    }
}
#endregion