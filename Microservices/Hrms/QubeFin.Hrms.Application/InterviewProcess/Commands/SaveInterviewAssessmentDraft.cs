using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;
using QubeFin.Hrms.Application.InterviewProcess.Services;

namespace QubeFin.Hrms.Application.InterviewProcess.Commands;

public record SaveInterviewAssessmentDraftCommand(Guid CandidateId, Guid EmployeeId, AssessmentSubmitDto Assessment, Guid SavedBy) : IRequest<Result<string>>, ICandidateWorkflowCommand;

public class SaveInterviewAssessmentDraftCommandValidator : AbstractValidator<SaveInterviewAssessmentDraftCommand>
{
    public SaveInterviewAssessmentDraftCommandValidator()
    {
        RuleFor(x => x.CandidateId).NotEmpty().WithMessage("Candidate is required.");
        RuleFor(x => x.EmployeeId).NotEmpty().WithMessage("Panelist is required.");
        RuleFor(x => x.Assessment.AppearanceAttitudeRating).InclusiveBetween(0, 5).When(x => x.Assessment.AppearanceAttitudeRating.HasValue);
        RuleFor(x => x.Assessment.PersonalityRating).InclusiveBetween(0, 5).When(x => x.Assessment.PersonalityRating.HasValue);
        RuleFor(x => x.Assessment.CommunicationRating).InclusiveBetween(0, 5).When(x => x.Assessment.CommunicationRating.HasValue);
        RuleFor(x => x.Assessment.EducationRating).InclusiveBetween(0, 5).When(x => x.Assessment.EducationRating.HasValue);
        RuleFor(x => x.Assessment.WorkExperienceRating).InclusiveBetween(0, 5).When(x => x.Assessment.WorkExperienceRating.HasValue);
        RuleFor(x => x.Assessment.TechnicalCompetenceRating).InclusiveBetween(0, 5).When(x => x.Assessment.TechnicalCompetenceRating.HasValue);
        RuleFor(x => x.Assessment.FlexibilityRating).InclusiveBetween(0, 5).When(x => x.Assessment.FlexibilityRating.HasValue);
        RuleFor(x => x.Assessment.AmbitionRating).InclusiveBetween(0, 5).When(x => x.Assessment.AmbitionRating.HasValue);
        RuleFor(x => x.Assessment.PotentialRating).InclusiveBetween(0, 5).When(x => x.Assessment.PotentialRating.HasValue);
        RuleFor(x => x.Assessment.OthersRating).InclusiveBetween(0, 5).When(x => x.Assessment.OthersRating.HasValue);
    }
}

internal sealed class SaveInterviewAssessmentDraftCommandHandler(IInterviewPanelRepository panelRepository, QubeFinDataContext context, IUnitOfWork unitOfWork) : IRequestHandler<SaveInterviewAssessmentDraftCommand, Result<string>>
{
    public async Task<Result<string>> Handle(SaveInterviewAssessmentDraftCommand request, CancellationToken cancellationToken)
    {
        if (request.SavedBy == Guid.Empty)
        {
            return new ValidationError("Authenticated user is required.");
        }

        var panel = await panelRepository.GetByCandidateAndEmployeeAsync(request.CandidateId, request.EmployeeId);
        if (panel is null)
        {
            return new RecordNotFoundError("Interview panel entry not found for the given candidate and employee.");
        }

        if (!panel.IsAttened)
        {
            return new ValidationError(panel.IsCandidateAbsent
                ? "The candidate was marked absent - there is no assessment to fill in."
                : "Record the candidate's attendance before filling in the assessment.");
        }

        if (!panel.IsSubmitted && await context.GetAssessmentBlockerAsync(panel, cancellationToken) is { } blocker)
        {
            return new ValidationError(blocker);
        }

        var dto = request.Assessment;
        var details = new AssessmentDetails(
            dto.AppearanceAttitudeRating, dto.AppearanceAttitudeRemarks,
            dto.PersonalityRating, dto.PersonalityRemarks,
            dto.CommunicationRating, dto.CommunicationRemarks,
            dto.EducationRating, dto.EducationRemarks,
            dto.WorkExperienceRating, dto.WorkExperienceRemarks,
            dto.TechnicalCompetenceRating, dto.TechnicalCompetenceRemarks,
            dto.FlexibilityRating, dto.FlexibilityRemarks,
            dto.AmbitionRating, dto.AmbitionRemarks,
            dto.PotentialRating, dto.PotentialRemarks,
            dto.OthersRating, dto.OthersRemarks,
            dto.AnyOtherJobsSuitedRemarks,
            dto.IsRecommendedForPosition,
            dto.PositiveRemarks,
            dto.NegativeRemarks);

        var result = panel.SaveAssessmentDraft(details, request.SavedBy);
        if (!result)
        {
            return new ValidationError("This assessment has already been submitted and cannot be changed.");
        }

        await panelRepository.UpdateAsync(panel);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok("Assessment saved as draft successfully.");
    }
}
