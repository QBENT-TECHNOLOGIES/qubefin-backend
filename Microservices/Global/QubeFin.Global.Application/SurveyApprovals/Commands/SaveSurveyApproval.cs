using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Global.Application.SurveyEvaluations.Models;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.SurveyApprovals.Commands;

#region --- COMMAND ---
public record SaveSurveyApprovalCommand(Guid SurveyId, EvaluationRequest Decision, Guid UserId, Guid EmployeeId) : IRequest<Result<string>>;
#endregion

#region --- VALIDATOR ---
public class SaveSurveyApprovalCommandValidator : AbstractValidator<SaveSurveyApprovalCommand>
{
    public SaveSurveyApprovalCommandValidator()
    {
        RuleFor(v => v.SurveyId).NotEmpty().WithMessage("Survey Id is required.");
        RuleFor(v => v.Decision).NotNull().SetValidator(new EvaluationRequestValidator());
    }
}
#endregion

#region --- HANDLER ---
// The Managing Director approves, rejects or sends the survey back for a recheck.
internal sealed class SaveSurveyApprovalCommandHandler(
    ISurveyRepository surveyRepository,
    IBranchSurveyRepository branchSurveyRepository,
    ISurveyEvaluationRepository evaluationRepository,
    ISurveyApprovalRepository approvalRepository,
    ISurveyCommitteeRepository committeeRepository,
    INotificationRepository notificationRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<SaveSurveyApprovalCommand, Result<string>>
{
    private const int MaxNotificationLength = 200;

    public async Task<Result<string>> Handle(SaveSurveyApprovalCommand request, CancellationToken cancellationToken)
    {
        var survey = await surveyRepository.GetByIdAsync(request.SurveyId);
        if (survey is null)
            return new RecordNotFoundError("Survey not found.");

        if (survey.SurveyType != SurveyTypes.Branch)
            return new ValidationError("Only Branch Surveys are approved here.");

        if (survey.Status != SurveyStatuses.PendingApproval)
            return new ValidationError("This survey is not waiting for approval.");

        if (!await approvalRepository.IsManagingDirectorAsync(request.EmployeeId))
            return new ForbiddenError("Only the Managing Director can approve the survey.");

        var branchSurvey = await branchSurveyRepository.GetBySurveyIdAsync(request.SurveyId);
        if (branchSurvey is null)
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

        branchSurvey.ActionBy = request.UserId;
        branchSurvey.ActionOn = now;
        branchSurvey.LastModifiedBy = request.UserId;
        branchSurvey.LastModifiedOn = now;

        var surveyorEmployeeIds = survey.SurveyAssigneds.Select(a => a.EmployeeId).ToList();

        if (decision == EvaluationDecisions.Approve)
        {
            branchSurvey.IsApproved = true;
            await surveyRepository.UpdateStatusAsync(request.SurveyId, SurveyStatuses.Approved, request.UserId);

            // The Committee Lead creates the branch next, so he and the surveyors are told.
            var recipients = surveyorEmployeeIds.Concat(await committeeRepository.GetActiveLeadEmployeeIdsAsync()).ToList();
            await Notify(recipients, "Branch Survey Approved",
                $"{survey.AssignmentNo} ({survey.ProposedArea}) was approved by the Managing Director.", "circle-check", survey.Id, request.UserId);
        }
        else if (decision == EvaluationDecisions.Reject)
        {
            branchSurvey.IsRejected = true;
            await surveyRepository.UpdateStatusAsync(request.SurveyId, SurveyStatuses.Rejected, request.UserId);

            var recipients = surveyorEmployeeIds.Concat(await committeeRepository.GetActiveMemberEmployeeIdsAsync()).ToList();
            var creatorEmployeeId = await surveyRepository.GetCreatorEmployeeIdAsync(request.SurveyId);
            if (creatorEmployeeId.HasValue)
                recipients.Add(creatorEmployeeId.Value);
            await Notify(recipients, "Branch Survey Rejected",
                $"{survey.AssignmentNo} ({survey.ProposedArea}) was rejected by the Managing Director. {comments}", "x-circle", survey.Id, request.UserId);
        }
        else
        {
            // Recheck: back to the surveyors. Old evaluations are kept as history and everyone evaluates again.
            await evaluationRepository.ArchiveActiveAsync(request.SurveyId);
            branchSurvey.IsSurveyorSubmit = false;
            branchSurvey.IsCommiteeSubmit = false;
            await surveyRepository.UpdateStatusAsync(request.SurveyId, SurveyStatuses.Draft, request.UserId);

            var recipients = surveyorEmployeeIds.Concat(await committeeRepository.GetActiveMemberEmployeeIdsAsync()).ToList();
            await Notify(recipients, "Branch Survey Needs Recheck",
                $"{survey.AssignmentNo} ({survey.ProposedArea}) needs a recheck, said the Managing Director. {comments}", "rotate-ccw", survey.Id, request.UserId);
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
