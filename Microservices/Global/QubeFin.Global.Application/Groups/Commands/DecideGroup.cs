using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Global.Application.Groups.Models;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Global.Application.Groups.Commands;

#region --- COMMAND ---
public record DecideGroupCommand(Guid Id, GroupDecisionRequest Decision, Guid UserId, Guid EmployeeId) : IRequest<Result<string>>;
#endregion

#region --- VALIDATOR ---
public class DecideGroupCommandValidator : AbstractValidator<DecideGroupCommand>
{
    public DecideGroupCommandValidator()
    {
        RuleFor(v => v.Id).NotEmpty().WithMessage("Group Id is required.");
        RuleFor(v => v.Decision).NotNull();
        RuleFor(v => v.Decision.Decision).Must(d => d == "Approve" || d == "Reject").WithMessage("Decision must be Approve or Reject.");
        RuleFor(v => v.Decision.Comments).NotEmpty().When(v => v.Decision.Decision == "Reject").WithMessage("Comments are required to reject a group.");
        RuleFor(v => v.Decision.Comments).MaximumLength(200).WithMessage("Comments can be 200 characters at most.");
    }
}
#endregion

#region --- HANDLER ---
// The Head of the branch or the approver above the branch approves or rejects a Pending group (never one added by himself).
internal sealed class DecideGroupCommandHandler(
    QubeFinDataContext context,
    ISurveyRepository surveyRepository,
    IGroupRepository groupRepository,
    INotificationRepository notificationRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DecideGroupCommand, Result<string>>
{
    public async Task<Result<string>> Handle(DecideGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetByIdAsync(request.Id);
        if (group is null)
            return new RecordNotFoundError("Group not found.");

        if ((group.CurrentStatus ?? string.Empty).Trim() != GroupStatuses.Pending)
            return new ValidationError("This group is not waiting for approval.");

        var rights = await GroupAccess.GetRightsAsync(context, surveyRepository, groupRepository, group, request.UserId, request.EmployeeId, cancellationToken);
        if (!rights.CanDecide)
            return new ForbiddenError("You cannot decide on this group. The Head of the branch or the Area Manager decides, and nobody decides on a group he added.");

        var approve = request.Decision.Decision == "Approve";
        var comments = (request.Decision.Comments ?? string.Empty).Trim();

        group.IsApproved = approve;
        group.CurrentStatus = approve ? GroupStatuses.Approved : GroupStatuses.Rejected;
        group.VerifiedBy = request.UserId;
        group.VerifiedOn = DateTime.Now;
        group.VerifiedRemarks = comments.Length == 0 ? null : comments;
        group.LastModifiedBy = request.UserId;
        group.LastModifiedOn = DateTime.Now;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        // The Credit Officer of the group and the Head of the branch are told.
        var recipients = new List<Guid>();
        if (group.DesignationId.HasValue)
            recipients.AddRange(await groupRepository.GetCurrentHolderEmployeeIdsAsync(group.DesignationId.Value));
        if (rights.Branch is not null)
            recipients.AddRange(await surveyRepository.GetHeadEmployeeIdsOfBranchAsync(rights.Branch.Id));
        recipients = recipients.Where(id => id != request.EmployeeId).Distinct().ToList();

        if (recipients.Count > 0)
        {
            var message = approve
                ? $"Group {group.Name} was approved."
                : $"Group {group.Name} was rejected. {comments}";
            if (message.Length > 200) message = message.Substring(0, 197) + "...";

            await notificationRepository.AddForEmployeesAsync(
                recipients,
                title: approve ? "Group Approved" : "Group Rejected",
                message: message,
                icon: approve ? "circle-check" : "x-circle",
                referenceId: group.Id,
                referenceType: "GROUP",
                actionUrl: "/secure/global/groups",
                createdBy: request.UserId);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Ok(approve ? "Group approved." : "Group rejected.");
    }
}
#endregion
