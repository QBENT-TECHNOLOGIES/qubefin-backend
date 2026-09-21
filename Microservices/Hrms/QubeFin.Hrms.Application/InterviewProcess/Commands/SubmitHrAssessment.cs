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

/// <summary>Finalizes HR's assessment of the candidate. Requires every panelist on the interview panel to
/// have already submitted their own assessment (mirrors the "IsShowHrAssessmentButton" gate computed by
/// USP_GetInterviewCandidateById). TotalRatingPoint/RatingStatus are computed here from the average of the
/// panelists' ratings - HR does not type these in.</summary>
public record SubmitHrAssessmentCommand(Guid CandidateId, Guid HrEmployeeId, HrAssessmentDecisionDto Decision, Guid SubmittedBy) : IRequest<Result<string>>;

public class SubmitHrAssessmentCommandValidator : AbstractValidator<SubmitHrAssessmentCommand>
{
    public SubmitHrAssessmentCommandValidator()
    {
        RuleFor(x => x.CandidateId).NotEmpty().WithMessage("Candidate is required.");
        RuleFor(x => x.Decision.RecommendationStatus).NotEmpty().WithMessage("A recommendation (e.g. Recommended / Not Recommended) is required.");
    }
}

internal sealed class SubmitHrAssessmentCommandHandler(
    ICandidateRepository candidateRepository,
    IInterviewPanelRepository panelRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<SubmitHrAssessmentCommand, Result<string>>
{
    public async Task<Result<string>> Handle(SubmitHrAssessmentCommand request, CancellationToken cancellationToken)
    {
        if (request.SubmittedBy == Guid.Empty || request.HrEmployeeId == Guid.Empty)
        {
            return new ValidationError("Authenticated user is required.");
        }

        var candidate = await candidateRepository.GetByIdAsync(request.CandidateId);
        if (candidate is null)
        {
            return new RecordNotFoundError("Candidate not found.");
        }

        var allPanelEntries = await panelRepository.GetByCandidateIdAsync(request.CandidateId);
        var hrEntry = allPanelEntries.FirstOrDefault(p => p.EmployeeId == request.HrEmployeeId);
        var hrIsInterviewer = hrEntry.IsGenuineInterviewer();

        var interviewers = allPanelEntries
            .Where(p => p.EmployeeId != request.HrEmployeeId && !p.IsHrRow())
            .ToList();
        if (hrIsInterviewer)
        {
            interviewers.Add(hrEntry!);
        }

        if (interviewers.Count == 0)
        {
            return new ValidationError("This candidate has no interview panel yet.");
        }

        if (interviewers.Any(p => !p.IsSubmitted))
        {
            return new ValidationError(hrIsInterviewer
                ? "Every panelist, including your own interview assessment, must be submitted before HR Assessment can be submitted."
                : "Every panelist must submit their assessment before HR Assessment can be submitted.");
        }

        var averages = HrAssessmentAverageCalculator.Compute(interviewers);
        var decision = request.Decision;

        if (!hrIsInterviewer)
        {
            // hrEntry is the administrative row (or none yet) - not HR's own interview submission, since HR
            // isn't genuinely on the panel. Snapshot the average into it as before.
            var details = new AssessmentDetails(
                averages.AppearanceAttitudeRating, null,
                averages.PersonalityRating, null,
                averages.CommunicationRating, null,
                averages.EducationRating, null,
                averages.WorkExperienceRating, null,
                averages.TechnicalCompetenceRating, null,
                averages.FlexibilityRating, null,
                averages.AmbitionRating, null,
                averages.PotentialRating, null,
                averages.OthersRating, null,
                decision.AnyOtherJobsSuitedRemarks,
                decision.IsRecommendedForPosition,
                decision.PositiveRemarks,
                decision.NegativeRemarks);

            if (hrEntry is null)
            {
                // Stamp HR's row with the candidate's own interview slot rather than "whenever HR happened to
                // open the form", so the row lines up with the interviewers' rows on the same candidate.
                var scheduledDate = candidate.InterviewDate;
                var scheduledTime = candidate.InterviewTime ?? TimeOnly.FromDateTime(DateTime.UtcNow);
                hrEntry = InterviewPanel.Schedule(request.CandidateId, request.HrEmployeeId, scheduledDate, scheduledTime, request.SubmittedBy);

                if (!hrEntry.SubmitAssessment(details, request.SubmittedBy))
                {
                    return new ValidationError("The HR Assessment has already been submitted and cannot be changed.");
                }

                await panelRepository.AddRangeAsync(new[] { hrEntry }, cancellationToken);
            }
            else
            {
                if (!hrEntry.SubmitAssessment(details, request.SubmittedBy))
                {
                    return new ValidationError("The HR Assessment has already been submitted and cannot be changed.");
                }

                await panelRepository.UpdateAsync(hrEntry);
            }
        }
        // When HR is also a genuine interviewer, hrEntry already holds their own locked, submitted score -
        // it's included in `averages` above via `interviewers`. Nothing further to write to the panel table;
        // only the Candidate-level decision below changes, so submitting the HR decision is never blocked by
        // HR's own interviewer submission, and can be resubmitted freely (e.g. to revise the recommendation).

        candidate.SubmitHrAssessment(
            decision.OverallPerformance,
            decision.SuitableRoleDepartment,
            decision.RecommendedGradeId,
            decision.IsTrainingRequired,
            decision.RecommendationStatus!,
            averages.Total,
            HrAssessmentAverageCalculator.RatingStatusFor(averages.Total),
            request.SubmittedBy);

        await candidateRepository.UpdateAsync(candidate);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok("HR Assessment submitted successfully.");
    }
}
