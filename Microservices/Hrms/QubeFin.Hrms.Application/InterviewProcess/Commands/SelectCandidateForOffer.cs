using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.InterviewProcess.Services;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.InterviewProcess.Commands;

/// <summary>"Is Candidate Selected" after the HR Assessment. Selected opens Candidate Verification; not selected stops
/// the workflow and records "... but not selected" on the recommendation. Either way the decision is final.</summary>
public record SelectCandidateForOfferCommand(Guid CandidateId, bool IsSelected, Guid EmployeeId, Guid ModifiedBy) : IRequest<Result<string>>, ICandidateWorkflowCommand;

public record SelectCandidateForOfferRequest(bool IsSelected);

public class SelectCandidateForOfferCommandValidator : AbstractValidator<SelectCandidateForOfferCommand>
{
    public SelectCandidateForOfferCommandValidator()
    {
        RuleFor(x => x.CandidateId).NotEmpty().WithMessage("Candidate is required.");
    }
}

internal sealed class SelectCandidateForOfferCommandHandler(
    ICandidateRepository candidateRepository,
    QubeFinDataContext context,
    IConfiguration configuration,
    IUnitOfWork unitOfWork) : IRequestHandler<SelectCandidateForOfferCommand, Result<string>>
{
    public async Task<Result<string>> Handle(SelectCandidateForOfferCommand request, CancellationToken cancellationToken)
    {
        if (request.ModifiedBy == Guid.Empty || request.EmployeeId == Guid.Empty)
        {
            return new ValidationError("Authenticated user is required.");
        }

        var hrPostId = Guid.Parse(configuration["HrPost"]!);
        if (!await context.IsHrEmployeeAsync(hrPostId, request.EmployeeId, cancellationToken))
        {
            return new ForbiddenError("Only HR can select a candidate.");
        }

        var candidate = await candidateRepository.GetByIdAsync(request.CandidateId);
        if (candidate is null)
        {
            return new RecordNotFoundError("Candidate not found.");
        }

        if (!candidate.IsHrAssessmentCompleted)
        {
            return new ValidationError("Submit the HR Assessment before deciding on the selection.");
        }

        if (!CandidateWorkflow.IsQualified(candidate.RecommendationStatus))
        {
            return new ValidationError("Only a recommended candidate can be selected or not selected.");
        }

        if (candidate.IsSelectedForOffer)
        {
            return new ValidationError("This candidate has already been selected.");
        }

        if (request.IsSelected)
        {
            candidate.SelectForOffer(request.ModifiedBy);
        }
        else
        {
            candidate.MarkNotSelected(request.ModifiedBy);
        }

        await candidateRepository.UpdateAsync(candidate);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(request.IsSelected
            ? $"Candidate {candidate.FirstName} {candidate.LastName} has been selected. Candidate Verification is now open."
            : $"Candidate {candidate.FirstName} {candidate.LastName} has been marked as not selected.");
    }
}
