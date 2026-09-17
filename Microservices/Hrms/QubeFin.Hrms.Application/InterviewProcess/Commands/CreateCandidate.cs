using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.InterviewProcess.Commands;

public record CreateCandidateCommand(CandidateCreateUpdateDto Candidate, Guid CreatedBy) : IRequest<Result<Guid>>;

public class CreateCandidateCommandValidator : AbstractValidator<CreateCandidateCommand>
{
    public CreateCandidateCommandValidator()
    {
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

internal sealed class CreateCandidateCommandHandler(ICandidateRepository candidateRepository, IUnitOfWork unitOfWork) : IRequestHandler<CreateCandidateCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateCandidateCommand request, CancellationToken cancellationToken)
    {
        if (request.CreatedBy == Guid.Empty)
        {
            return new ValidationError("Authenticated user is required.");
        }

        var candidate = request.Candidate;

        var referenceNo = await GenerateReferenceNoAsync(candidate.CompanyId, cancellationToken);

        var candidateDetails = new CandidateDetails(
            candidate.FirstName,
            candidate.MiddleName,
            candidate.LastName,
            candidate.Gender,
            candidate.FatherName,
            candidate.MobileNo,
            candidate.Email,
            candidate.HouseNo,
            candidate.RoadName,
            candidate.LandMark,
            candidate.AdministrativeUnitId,
            candidate.PoliceStationId,
            candidate.PostOfficeId,
            candidate.PinCode,
            candidate.InterviewDate,
            candidate.InterviewTime,
            candidate.DepartmentId,
            candidate.InterviewPost,
            null,
            candidate.VenueOrganizationUnitId,
            candidate.InterviewMode,
            candidate.ReferedBy,
            candidate.RecruitmentSource,
            candidate.VacancyReference,
            candidate.CurrentSalary,
            candidate.ExpectedSalary,
            candidate.NoticePeriodInDays,
            candidate.EarliestJoiningDate,
            candidate.IsWillingRelocate,
            candidate.PreferredLocation,
            candidate.PostedOrganizationUnitId,
            candidate.DateOfJoining,
            candidate.ReportingTime,
            candidate.MonthlyCostCompany,
            candidate.OverallPerformance,
            candidate.SuitableRoleDepartment,
            candidate.RecommendedGradeId,
            candidate.IsTrainingRequired,
            candidate.RecommendationStatus,
            candidate.AadharNumber,
            candidate.VoterNumber,
            candidate.Pan,
            candidate.Uan
        );

        var entity = Candidate.Create(candidate.CompanyId, referenceNo, request.CreatedBy, candidateDetails);

        await candidateRepository.AddAsync(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(entity.Id);
    }

    private async Task<string> GenerateReferenceNoAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var year = DateTime.UtcNow.Year;
        var countThisYear = await candidateRepository.CountCreatedInYearAsync(companyId, year, cancellationToken);

        return $"WGRW/CAND/{year}/{countThisYear + 1:D4}";
    }
}
