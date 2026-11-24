using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Global.Application.SurveyEvaluations.Models;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.GroupSurveys.Commands;

#region --- COMMAND ---
// Decision + Comments, the same shape as the Managing Director decision of a Branch Survey.
public record SaveGroupApprovalCommand(Guid SurveyId, EvaluationRequest Decision, Guid UserId, Guid EmployeeId) : IRequest<Result<string>>;
#endregion

#region --- VALIDATOR ---
public class SaveGroupApprovalCommandValidator : AbstractValidator<SaveGroupApprovalCommand>
{
    public SaveGroupApprovalCommandValidator()
    {
        RuleFor(v => v.SurveyId).NotEmpty().WithMessage("Survey Id is required.");
        RuleFor(v => v.Decision).NotNull().SetValidator(new EvaluationRequestValidator());
    }
}
#endregion

#region --- HANDLER ---
// The head of the unit above the branch (the Area Manager) approves, rejects or sends the survey back to the Credit Officers.
internal sealed class SaveGroupApprovalCommandHandler(
    ISurveyRepository surveyRepository,
    IGroupSurveyRepository groupSurveyRepository,
    ISurveyApprovalRepository approvalRepository,
    INotificationRepository notificationRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<SaveGroupApprovalCommand, Result<string>>
{
    private const int MaxNotificationLength = 200;

    public async Task<Result<string>> Handle(SaveGroupApprovalCommand request, CancellationToken cancellationToken)
    {
        var survey = await surveyRepository.GetByIdAsync(request.SurveyId);
        if (survey is null)
            return new RecordNotFoundError("Survey not found.");

        if (survey.SurveyType != SurveyTypes.Group || survey.BranchOrganizationUnitId is null)
            return new ValidationError("Only Group Surveys are approved here.");

        if (survey.Status != SurveyStatuses.PendingApproval)
            return new ValidationError("This survey is not waiting for approval.");

        if (!await surveyRepository.IsApproverOfBranchAsync(request.EmployeeId, survey.BranchOrganizationUnitId.Value))
            return new ForbiddenError("Only the Area Manager of this branch can decide on the survey.");

        var groupSurvey = await groupSurveyRepository.GetBySurveyIdAsync(request.SurveyId);
        if (groupSurvey is null)
            return new ValidationError("The survey form has not been filled in.");

        var decision = request.Decision.Decision;
        var comments = (request.Decision.Comments ?? string.Empty).Trim();
        var now = DateTime.Now;

        await approvalRepository.AddAsync(new TblSurveyApproval
        {
            Id = Guid.NewGuid(),
            SurveyId = request.SurveyId,
            AuthorityEmployeeId = request.EmployeeId,
            Action = ApprovalActions.FromDecision(decision),
            Comments = comments.Length == 0 ? null : comments,
            ActionOn = now
        });

        // Credit Officers and the Head of the branch are told about every decision.
        var recipients = survey.SurveyAssigneds.Select(a => a.EmployeeId)
            .Concat(await surveyRepository.GetHeadEmployeeIdsOfBranchAsync(survey.BranchOrganizationUnitId.Value))
            .ToList();

        if (decision == EvaluationDecisions.Approve)
        {
            await surveyRepository.UpdateStatusAsync(request.SurveyId, SurveyStatuses.Approved, request.UserId);
            await Notify(recipients, "Group Survey Approved",
                $"{survey.AssignmentNo} ({survey.ProposedArea}) was approved.", "circle-check", survey.Id, request.UserId);
        }
        else if (decision == EvaluationDecisions.Reject)
        {
            await surveyRepository.UpdateStatusAsync(request.SurveyId, SurveyStatuses.Rejected, request.UserId);
            await Notify(recipients, "Group Survey Rejected",
                $"{survey.AssignmentNo} ({survey.ProposedArea}) was rejected. {comments}", "x-circle", survey.Id, request.UserId);
        }
        else
        {
            // Recheck: back to the Credit Officers, who submit again; the Head recommends again.
            SaveGroupRecommendationCommandHandler.ResetForResubmit(groupSurvey, request.UserId, now);
            await surveyRepository.UpdateStatusAsync(request.SurveyId, SurveyStatuses.Draft, request.UserId);
            await Notify(recipients, "Group Survey Needs Recheck",
                $"{survey.AssignmentNo} ({survey.ProposedArea}) needs a recheck, said the Area Manager. {comments}", "rotate-ccw", survey.Id, request.UserId);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok("Decision saved successfully.");
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
