using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.InterviewProcess.Commands;

public record UpdateCandidateCommand(Guid Id, CandidateCreateUpdateDto Candidate, Guid ModifiedBy) : IRequest<Result<string>>;

public class UpdateCandidateCommandValidator : AbstractValidator<UpdateCandidateCommand>
{
    public UpdateCandidateCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Candidate Id is required.");

        RuleFor(x => x.Candidate.CompanyId)
            .NotEmpty()
            .WithMessage("Company is required.");

        RuleFor(x => x.Candidate.FirstName)
            .NotEmpty()
            .WithMessage("First name is required.")
            .MaximumLength(50)
            .WithMessage("First name cannot exceed 50 characters.");

        RuleFor(x => x.Candidate.LastName)
            .NotEmpty()
            .WithMessage("Last name is required.")
            .MaximumLength(50)
            .WithMessage("Last name cannot exceed 50 characters.");

        RuleFor(x => x.Candidate.Gender)
            .NotEmpty()
            .WithMessage("Gender is required.")
            .MaximumLength(10)
            .WithMessage("Gender cannot exceed 10 characters.");

        RuleFor(x => x.Candidate.MobileNo)
            .NotEmpty()
            .WithMessage("Mobile number is required.")
            .Length(10)
            .WithMessage("Mobile number must be exactly 10 digits.")
            .Matches(@"^\d{10}$")
            .WithMessage("Mobile number must contain digits only.");

        RuleFor(x => x.Candidate.InterviewDate)
            .NotEmpty()
            .WithMessage("Interview date is required.");

        RuleFor(x => x.Candidate.InterviewPost)
            .NotEmpty()
            .WithMessage("Interview post is required.");
    }
}

internal sealed class UpdateCandidateCommandHandler(ICandidateRepository candidateRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateCandidateCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateCandidateCommand request, CancellationToken cancellationToken)
    {
        if (request.ModifiedBy == Guid.Empty)
        {
            return new ValidationError("Authenticated user is required.");
        }

        var candidate = await candidateRepository.GetByIdAsync(request.Id);

        if (candidate is null)
        {
            return new RecordNotFoundError("Candidate not found for the given Id.");
        }

        var candidateDetails = new CandidateDetails(
            request.Candidate.FirstName,
            request.Candidate.MiddleName,
            request.Candidate.LastName,
            request.Candidate.Gender,
            request.Candidate.FatherName,
            request.Candidate.MobileNo,
            request.Candidate.Email,
            request.Candidate.HouseNo,
            request.Candidate.RoadName,
            request.Candidate.LandMark,
            request.Candidate.AdministrativeUnitId,
            request.Candidate.PoliceStationId,
            request.Candidate.PostOfficeId,
            request.Candidate.PinCode,
            request.Candidate.InterviewDate,
            request.Candidate.InterviewTime,
            request.Candidate.DepartmentId,
            request.Candidate.InterviewPost,
            null,
            request.Candidate.VenueOrganizationUnitId,
            request.Candidate.InterviewMode,
            request.Candidate.ReferedBy,
            request.Candidate.RecruitmentSource,
            request.Candidate.VacancyReference,
            request.Candidate.CurrentSalary,
            request.Candidate.ExpectedSalary,
            request.Candidate.NoticePeriodInDays,
            request.Candidate.EarliestJoiningDate,
            request.Candidate.IsWillingRelocate,
            request.Candidate.PreferredLocation,
            request.Candidate.PostedOrganizationUnitId,
            request.Candidate.DateOfJoining,
            request.Candidate.ReportingTime,
            request.Candidate.MonthlyCostCompany,
            request.Candidate.OverallPerformance,
            request.Candidate.SuitableRoleDepartment,
            request.Candidate.RecommendedGradeId,
            request.Candidate.IsTrainingRequired,
            request.Candidate.RecommendationStatus,
            request.Candidate.AadharNumber,
            request.Candidate.VoterNumber,
            request.Candidate.Pan,
            request.Candidate.Uan
        );

        candidate.Update(candidateDetails, request.ModifiedBy);

        await candidateRepository.UpdateAsync(candidate);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok("Candidate updated successfully.");
    }
}