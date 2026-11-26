using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Members;
using System.Text.RegularExpressions;

namespace QubeFin.Global.Application.Members.Commands;

#region --- COMMAND ---
public record CreateMemberCommand(
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
    Guid CreatedBy,
    Guid EmployeeId
) : IRequest<Result<CreateMemberResponse?>>;
#endregion


#region --- VALIDATION ---
public class CreateMemberCommandValidator : AbstractValidator<CreateMemberCommand>
{
    public CreateMemberCommandValidator()
    {
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

#region --- RESPONSE ---
public record CreateMemberResponse(Guid Id, string? Message);
#endregion


#region --- HANDLER ---
internal sealed class CreateMemberCommandHandler(
    IMemberRepository memberRepository,
    IUnitOfWork unitOfWork,
    QubeFinDataContext context)
    : IRequestHandler<CreateMemberCommand, Result<CreateMemberResponse?>>
{
    public async Task<Result<CreateMemberResponse?>> Handle(CreateMemberCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var designationId = await context.TblEmployeeDesignations
                .AsNoTracking()
                .Where(ed => ed.EmployeeId == request.EmployeeId)
                .OrderByDescending(ed => ed.EffectiveTo == null)
                .ThenByDescending(ed => ed.EffectiveFrom)
                .Select(ed => (Guid?)ed.DesignationId)
                .FirstOrDefaultAsync(cancellationToken);

            Guid memberId = Guid.NewGuid();

            var personalInfo = new PersonalInfo(
                request.BranchCode,
                request.GroupId,
                null,
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
                request.AlternateMobileNo);

            var member = Member.Create(
                memberId,
                null,
                personalInfo,
                request.CreatedBy,
                designationId);

            await memberRepository.AddAsync(member);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var fullName = $"{request.FirstName} {request.LastName}".Trim();
            return Result.Ok<CreateMemberResponse?>(
                new CreateMemberResponse(memberId, $"Member created successfully with Name : {fullName}"));
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error($"An error occurred while creating the member: {ex.Message}"));
        }
    }
}
#endregion