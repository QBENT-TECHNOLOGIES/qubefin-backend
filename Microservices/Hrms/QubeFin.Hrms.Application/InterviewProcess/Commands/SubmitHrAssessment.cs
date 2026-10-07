using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Hrms.Application.InterviewProcess.Services;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.InterviewProcess.Commands;

public record SubmitHrAssessmentCommand(Guid CandidateId, Guid HrEmployeeId, HrAssessmentDecisionDto Decision, Guid SubmittedBy) : IRequest<Result<string>>, ICandidateWorkflowCommand;

public class SubmitHrAssessmentCommandValidator : AbstractValidator<SubmitHrAssessmentCommand>
{
    public SubmitHrAssessmentCommandValidator()
    {
        RuleFor(x => x.CandidateId).NotEmpty().WithMessage("Candidate is required.");
        RuleFor(x => x.Decision.RecommendationStatus)
            .NotEmpty().WithMessage("A recommendation (e.g. Recommended / Not Recommended) is required.")
            .NotEqual(CandidateWorkflow.Pending).WithMessage("A recommendation (e.g. Recommended / Not Recommended) is required.")
            .NotEqual(CandidateWorkflow.Rejected).WithMessage("Use Reject to reject the candidate.");
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

        if (candidate.IsHrAssessmentCompleted)
        {
            return new ValidationError("The HR Assessment has already been submitted and cannot be changed.");
        }

        var submitted = (await panelRepository.GetByCandidateIdAsync(request.CandidateId, includeEmployee: false))
            .Where(p => p.IsSubmitted)
            .ToList();

        if (submitted.Count == 0)
        {
            return new ValidationError("No panelist has submitted their assessment yet - HR Assessment cannot be submitted.");
        }

        var averages = HrAssessmentAverageCalculator.Compute(submitted);
        var decision = request.Decision;

        candidate.SubmitHrAssessment(
            decision.OverallPerformance,
            decision.SuitableRoleDepartment,
            decision.RecommendedGradeId,
            decision.IsTrainingRequired,
            decision.RecommendationStatus!,
            averages.Total,
            HrAssessmentAverageCalculator.RatingStatusFor(averages.Total),
            request.SubmittedBy);

        candidate.SaveSalaryAndJoiningDetails(
            decision.CurrentSalary,
            decision.ExpectedSalary,
            decision.NoticePeriodInDays,
            decision.EarliestJoiningDate,
            decision.IsWillingRelocate,
            decision.PreferredLocation,
            request.SubmittedBy);

        await candidateRepository.UpdateAsync(candidate);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(CandidateWorkflow.IsQualified(decision.RecommendationStatus)
            ? "HR Assessment submitted successfully."
            : "HR Assessment submitted. The candidate is marked Not Selected.");
    }
}
