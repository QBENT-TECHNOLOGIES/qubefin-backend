using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.BranchSurveys.Commands;

#region --- COMMAND ---
public record SubmitBranchSurveyCommand(Guid SurveyId, Guid UserId, Guid EmployeeId) : IRequest<Result<string>>;
#endregion

#region --- VALIDATOR ---
public class SubmitBranchSurveyCommandValidator : AbstractValidator<SubmitBranchSurveyCommand>
{
    public SubmitBranchSurveyCommandValidator()
    {
        RuleFor(v => v.SurveyId).NotEmpty().WithMessage("Survey Id is required.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class SubmitBranchSurveyCommandHandler(
    ISurveyRepository surveyRepository,
    IBranchSurveyRepository branchSurveyRepository,
    ISurveyDocumentRepository surveyDocumentRepository,
    ISurveyCommitteeRepository surveyCommitteeRepository,
    INotificationRepository notificationRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<SubmitBranchSurveyCommand, Result<string>>
{
    // How many missing fields are named in the error message before it says "and N more".
    private const int MaxMissingFieldsShown = 8;

    public async Task<Result<string>> Handle(SubmitBranchSurveyCommand request, CancellationToken cancellationToken)
    {
        var survey = await surveyRepository.GetByIdAsync(request.SurveyId);
        if (survey is null)
            return new RecordNotFoundError("Survey not found.");

        if (survey.SurveyType != SurveyTypes.Branch)
            return new ValidationError("This survey does not have a branch survey form.");

        if (survey.Status != SurveyStatuses.Assigned && survey.Status != SurveyStatuses.Draft)
            return new ValidationError("This survey has already been submitted.");

        // Only the Lead Surveyor can submit.
        var isLeadSurveyor = survey.SurveyAssigneds.Any(a => a.EmployeeId == request.EmployeeId && a.IsLead);
        if (!isLeadSurveyor)
            return new ForbiddenError("Only the Lead Surveyor can submit the survey.");

        var branchSurvey = await branchSurveyRepository.GetBySurveyIdAsync(request.SurveyId);
        if (branchSurvey is null)
            return new ValidationError("Please fill in and save the survey form before submitting.");

        // Every required field must be filled in.
        var missingFields = BranchSurveySubmitCheck.GetMissingFields(branchSurvey);
        if (missingFields.Count > 0)
            return new ValidationError(BuildMissingFieldsMessage(missingFields));

        // If the surveyor says photographs are attached, there must be at least one.
        if (branchSurvey.PhotographsAttached == "Yes" && await surveyDocumentRepository.CountAsync(request.SurveyId) == 0)
            return new ValidationError("Photographs Attached is Yes, so please upload at least one photograph before submitting.");

        // Submit: mark the form as submitted and move the survey to the committee.
        branchSurvey.IsSurveyorSubmit = true;
        branchSurvey.SurveyorSubmitOn = DateTime.Now;
        branchSurvey.LastModifiedBy = request.UserId;
        branchSurvey.LastModifiedOn = DateTime.Now;
        await surveyRepository.UpdateStatusAsync(request.SurveyId, SurveyStatuses.PendingEvaluation, request.UserId);

        // Tell every active committee member (including the Committee Lead).
        var committeeEmployeeIds = await surveyCommitteeRepository.GetActiveMemberEmployeeIdsAsync();
        await notificationRepository.AddForEmployeesAsync(
            committeeEmployeeIds,
            title: "Branch Survey Submitted",
            message: $"{survey.AssignmentNo} ({survey.ProposedArea}) has been submitted for evaluation.",
            icon: "clipboard-check",
            referenceId: survey.Id,
            referenceType: "SURVEY",
            actionUrl: "/secure/global/surveys",
            createdBy: request.UserId);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok("Survey submitted successfully.");
    }

    private static string BuildMissingFieldsMessage(List<string> missingFields)
    {
        var shown = string.Join(", ", missingFields.Take(MaxMissingFieldsShown));
        var extra = missingFields.Count - MaxMissingFieldsShown;
        return extra > 0
            ? $"Please fill in all required fields before submitting. Missing: {shown} and {extra} more."
            : $"Please fill in all required fields before submitting. Missing: {shown}.";
    }
}
#endregion
