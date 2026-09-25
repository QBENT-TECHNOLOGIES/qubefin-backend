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
        RuleFor(x => x.Candidate.MonthlyCostCompany)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Candidate.MonthlyCostCompany.HasValue)
            .WithMessage("Monthly cost to company cannot be negative.");
    }
}

internal sealed class UpdateCandidateCommandHandler(ICandidateRepository candidateRepository, IInterviewPanelRepository panelRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateCandidateCommand, Result<string>>
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

        // Only what the candidate form edits is taken from the request: the basic details plus the joining details
        // (posted unit, joining date, reporting time, monthly CTC). Everything filled in elsewhere (HR assessment,
        // salary expectations, KYC numbers) is kept as it is, so an edit cannot wipe it.
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
            details.InterviewDate,
            details.InterviewTime,
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
            candidate.Uan
        );

        var interviewRescheduled = candidate.InterviewDate != details.InterviewDate || candidate.InterviewTime != details.InterviewTime;

        candidate.Update(candidateDetails, request.ModifiedBy);

        await candidateRepository.UpdateAsync(candidate);

        // Keep the panel on the candidate's interview slot. Panelists who already attended keep the slot they
        // interviewed in; the HR Assessment row is not an interview slot and is left alone.
        if (interviewRescheduled && details.InterviewTime is { } interviewTime)
        {
            var panelists = (await panelRepository.GetByCandidateIdAsync(candidate.Id)).Interviewers().Where(p => !p.IsAttened);
            foreach (var panelist in panelists)
            {
                panelist.Reschedule(details.InterviewDate, interviewTime, request.ModifiedBy);
                await panelRepository.UpdateAsync(panelist);
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok("Candidate updated successfully.");
    }
}