using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Global.Application.GroupSurveys.Models;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.GroupSurveys.Commands;

#region --- COMMAND ---
public record SaveGroupRecommendationCommand(Guid SurveyId, GroupRecommendationRequest Request, Guid UserId, Guid EmployeeId) : IRequest<Result<string>>;
#endregion

#region --- VALIDATOR ---
public class SaveGroupRecommendationCommandValidator : AbstractValidator<SaveGroupRecommendationCommand>
{
    public SaveGroupRecommendationCommandValidator()
    {
        RuleFor(v => v.SurveyId).NotEmpty().WithMessage("Survey Id is required.");
        RuleFor(v => v.Request).NotNull().SetValidator(new GroupRecommendationRequestValidator());
    }
}
#endregion

#region --- HANDLER ---
// The Head of Branch recommends the survey (it then goes to the approver) or sends it back to the Credit Officers.
internal sealed class SaveGroupRecommendationCommandHandler(
    ISurveyRepository surveyRepository,
    IGroupSurveyRepository groupSurveyRepository,
    ISurveyApprovalRepository approvalRepository,
    INotificationRepository notificationRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<SaveGroupRecommendationCommand, Result<string>>
{
    private const int MaxNotificationLength = 200;

    public async Task<Result<string>> Handle(SaveGroupRecommendationCommand request, CancellationToken cancellationToken)
    {
        var survey = await surveyRepository.GetByIdAsync(request.SurveyId);
        if (survey is null)
            return new RecordNotFoundError("Survey not found.");

        if (survey.SurveyType != SurveyTypes.Group || survey.BranchOrganizationUnitId is null)
            return new ValidationError("Only Group Surveys are recommended here.");

        if (survey.Status != SurveyStatuses.PendingRecommendation)
            return new ValidationError("This survey is not waiting for a recommendation.");

        if (!await surveyRepository.IsHeadOfBranchAsync(request.EmployeeId, survey.BranchOrganizationUnitId.Value))
            return new ForbiddenError("Only the Head of the branch can recommend the survey.");

        var groupSurvey = await groupSurveyRepository.GetBySurveyIdAsync(request.SurveyId);
        if (groupSurvey is null)
            return new ValidationError("The survey form has not been filled in.");

        var comments = (request.Request.Comments ?? string.Empty).Trim();
        var now = DateTime.Now;
        var creditOfficerIds = survey.SurveyAssigneds.Select(a => a.EmployeeId).ToList();

        if (request.Request.Action == GroupRecommendationActions.Recommend)
        {
            // The approver must exist, because he decides next.
            var approverIds = await surveyRepository.GetApproverEmployeeIdsOfBranchAsync(survey.BranchOrganizationUnitId.Value);
            if (approverIds.Count == 0)
                return new ValidationError("No approver (Area Manager) was found above this branch, so the survey cannot be sent for approval.");

            groupSurvey.BranchManagerEmployeeId = request.EmployeeId;
            groupSurvey.BranchManagerRecommendation = request.Request.Recommendation;
            groupSurvey.BranchManagerComments = comments.Length == 0 ? null : comments;
            groupSurvey.BranchManagerActionOn = now;
            groupSurvey.LastModifiedBy = request.UserId;
            groupSurvey.LastModifiedOn = now;
            await surveyRepository.UpdateStatusAsync(request.SurveyId, SurveyStatuses.PendingApproval, request.UserId);

            await Notify(approverIds, "Group Survey Waiting For Approval",
                $"{survey.AssignmentNo} ({survey.ProposedArea}) was recommended ({request.Request.Recommendation}) and is waiting for your approval.",
                "clipboard-check", survey.Id, request.UserId);
        }
        else
        {
            // Recheck: back to the Credit Officers. The Credit Officers can change the form and submit it again.
            await approvalRepository.AddAsync(new TblSurveyApproval
            {
                Id = Guid.NewGuid(),
                SurveyId = request.SurveyId,
                AuthorityEmployeeId = request.EmployeeId,
                Action = ApprovalActions.Recheck,
                Comments = comments,
                ActionOn = now
            });
            ResetForResubmit(groupSurvey, request.UserId, now);
            await surveyRepository.UpdateStatusAsync(request.SurveyId, SurveyStatuses.Draft, request.UserId);

            await Notify(creditOfficerIds, "Group Survey Needs Recheck",
                $"{survey.AssignmentNo} ({survey.ProposedArea}) needs a recheck, said the Head of the branch. {comments}",
                "rotate-ccw", survey.Id, request.UserId);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(request.Request.Action == GroupRecommendationActions.Recommend
            ? "Recommendation saved successfully."
            : "The survey was sent back to the Credit Officers.");
    }

    // The Credit Officers submit again, and then the recommendation is given again.
    internal static void ResetForResubmit(TblGroupSurvey groupSurvey, Guid userId, DateTime now)
    {
        groupSurvey.IsSurveyorSubmit = false;
        groupSurvey.SurveyorSubmitOn = null;
        groupSurvey.BranchManagerEmployeeId = null;
        groupSurvey.BranchManagerRecommendation = null;
        groupSurvey.BranchManagerComments = null;
        groupSurvey.BranchManagerActionOn = null;
        groupSurvey.LastModifiedBy = userId;
        groupSurvey.LastModifiedOn = now;
    }

    private async Task Notify(List<Guid> employeeIds, string title, string message, string icon, Guid surveyId, Guid userId)
    {
        if (message.Length > MaxNotificationLength)
            message = message.Substring(0, MaxNotificationLength - 3) + "...";

        await notificationRepository.AddForEmployeesAsync(
            employeeIds.Distinct().ToList(),
            title: title,
            message: message,
            icon: icon,
            referenceId: surveyId,
            referenceType: "SURVEY",
            actionUrl: "/secure/global/surveys",
            createdBy: userId);
    }
}
#endregion
