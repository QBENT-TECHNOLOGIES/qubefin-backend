using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.GroupSurveys.Commands;

#region --- COMMAND ---
public record SubmitGroupSurveyCommand(Guid SurveyId, Guid UserId, Guid EmployeeId) : IRequest<Result<string>>;
#endregion

#region --- VALIDATOR ---
public class SubmitGroupSurveyCommandValidator : AbstractValidator<SubmitGroupSurveyCommand>
{
    public SubmitGroupSurveyCommandValidator()
    {
        RuleFor(v => v.SurveyId).NotEmpty().WithMessage("Survey Id is required.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class SubmitGroupSurveyCommandHandler(
    ISurveyRepository surveyRepository,
    IGroupSurveyRepository groupSurveyRepository,
    INotificationRepository notificationRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<SubmitGroupSurveyCommand, Result<string>>
{
    // How many missing fields are named in the error message before it says "and N more".
    private const int MaxMissingFieldsShown = 8;

    public async Task<Result<string>> Handle(SubmitGroupSurveyCommand request, CancellationToken cancellationToken)
    {
        var survey = await surveyRepository.GetByIdAsync(request.SurveyId);
        if (survey is null)
            return new RecordNotFoundError("Survey not found.");

        if (survey.SurveyType != SurveyTypes.Group)
            return new ValidationError("This survey does not have a group survey form.");

        if (survey.Status != SurveyStatuses.Assigned && survey.Status != SurveyStatuses.Draft)
            return new ValidationError("This survey has already been submitted.");

        // Only the Lead Credit Officer can submit.
        var isLeadCreditOfficer = survey.SurveyAssigneds.Any(a => a.EmployeeId == request.EmployeeId && a.IsLead);
        if (!isLeadCreditOfficer)
            return new ForbiddenError("Only the Lead Credit Officer can submit the survey.");

        var groupSurvey = await groupSurveyRepository.GetBySurveyIdAsync(request.SurveyId);
        if (groupSurvey is null)
            return new ValidationError("Please fill in and save the survey form before submitting.");

        // Every required field must be filled in.
        var missingFields = GroupSurveySubmitCheck.GetMissingFields(groupSurvey);
        if (missingFields.Count > 0)
            return new ValidationError(BuildMissingFieldsMessage(missingFields));

        // The Head of the branch must exist, because he recommends the survey next.
        if (survey.BranchOrganizationUnitId is null)
            return new ValidationError("The survey has no branch.");
        var headEmployeeIds = await surveyRepository.GetHeadEmployeeIdsOfBranchAsync(survey.BranchOrganizationUnitId.Value);
        if (headEmployeeIds.Count == 0)
            return new ValidationError("This branch has no Head (person in charge or Branch Manager), so the survey cannot be sent for recommendation.");

        // Submit: mark the form as submitted and move the survey to the Head of the branch.
        groupSurvey.IsSurveyorSubmit = true;
        groupSurvey.SurveyorSubmitOn = DateTime.Now;
        groupSurvey.LastModifiedBy = request.UserId;
        groupSurvey.LastModifiedOn = DateTime.Now;
        await surveyRepository.UpdateStatusAsync(request.SurveyId, SurveyStatuses.PendingRecommendation, request.UserId);

        await notificationRepository.AddForEmployeesAsync(
            headEmployeeIds,
            title: "Group Survey Submitted",
            message: $"{survey.AssignmentNo} ({survey.ProposedArea}) has been submitted and is waiting for your recommendation.",
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
