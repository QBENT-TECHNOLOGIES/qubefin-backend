using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Hrms.Application.InterviewProcess.Services;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.InterviewProcess.Commands;

public record UpdateCandidateCommand(Guid Id, CandidateCreateUpdateDto Candidate, Guid ModifiedBy) : IRequest<Result<string>>, ICandidateWorkflowCommand
{
    Guid ICandidateWorkflowCommand.CandidateId => Id;
}

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

        RuleFor(x => x.Candidate.Address)
            .MaximumLength(200)
            .WithMessage("Address cannot exceed 200 characters.");

        RuleFor(x => x.Candidate.CvFile).CandidateDocument("CV", required: false);
        RuleFor(x => x.Candidate.JobApplicationFile).CandidateDocument("Job Application", required: false);

        RuleFor(x => x.Candidate.InterviewPost)
            .NotEmpty()
            .WithMessage("Interview post is required.");
        RuleFor(x => x.Candidate.MonthlyCostCompany)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Candidate.MonthlyCostCompany.HasValue)
            .WithMessage("Monthly cost to company cannot be negative.");
    }
}

internal sealed class UpdateCandidateCommandHandler(ICandidateRepository candidateRepository, IFileStorageRepository fileStorageRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateCandidateCommand, Result<string>>
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

        // Details are editable only until the offer letter is received - the same point the Edit button hides.
        if (candidate.IsOfferLetterReceived)
        {
            return new ValidationError("Candidate details cannot be edited once the offer letter has been received.");
        }

        // A replaced CV / job application is uploaded first; files not sent are kept.
        string? cvFile, jobApplicationFile;
        try
        {
            cvFile = await fileStorageRepository.UploadIfPresentAsync(request.Candidate.CvFile, cancellationToken);
            jobApplicationFile = await fileStorageRepository.UploadIfPresentAsync(request.Candidate.JobApplicationFile, cancellationToken);
        }
        catch (Exception)
        {
            return new ValidationError("Unable to upload the CV / Job Application. Please try again.");
        }

        // Only what the candidate form edits is taken from the request: the basic details plus the joining details
        // (posted unit, joining date, reporting time, monthly CTC). Everything filled in elsewhere (HR assessment,
        // salary expectations, KYC numbers, the interview slot set by Schedule) is kept as it is, so an edit cannot wipe it.
        var details = request.Candidate;
        var candidateDetails = new CandidateDetails(
            details.FirstName,
            details.MiddleName,
            details.LastName,
            details.Gender,
            details.FatherName,
            details.MobileNo,
            details.Email,
            details.HouseNo,
            details.RoadName,
            details.LandMark,
            details.AdministrativeUnitId,
            details.PoliceStationId,
            details.PostOfficeId,
            details.PinCode,
            candidate.InterviewDate,
            candidate.InterviewTime,
            details.DepartmentId,
            details.InterviewPost,
            candidate.InterviewPostName,
            details.VenueOrganizationUnitId,
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
            details.PostedOrganizationUnitId,
            details.DateOfJoining,
            details.ReportingTime,
            details.MonthlyCostCompany,
            candidate.OverallPerformance,
            candidate.SuitableRoleDepartment,
            candidate.RecommendedGradeId,
            candidate.IsTrainingRequired,
            candidate.RecommendationStatus,
            candidate.AadharNumber,
            candidate.VoterNumber,
            candidate.Pan,
            candidate.Uan,
            details.Address
        );

        candidate.Update(candidateDetails, request.ModifiedBy);

        if (cvFile is not null || jobApplicationFile is not null)
        {
            candidate.SetDocuments(cvFile, jobApplicationFile, request.ModifiedBy);
        }

        await candidateRepository.UpdateAsync(candidate);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok("Candidate updated successfully.");
    }
}