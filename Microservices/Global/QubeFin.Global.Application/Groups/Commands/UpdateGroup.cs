using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using QubeFin.Core.Results;
using QubeFin.Global.Application.Groups.Models;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Global.Application.Groups.Commands;

#region --- COMMAND ---
public record UpdateGroupCommand(Guid Id, GroupRequest Group, Guid UserId, Guid EmployeeId) : IRequest<Result<string>>;
#endregion

#region --- VALIDATOR ---
public class UpdateGroupCommandValidator : AbstractValidator<UpdateGroupCommand>
{
    public UpdateGroupCommandValidator()
    {
        RuleFor(v => v.Id).NotEmpty().WithMessage("Group Id is required.");
        RuleFor(v => v.Group).NotNull().SetValidator(new GroupRequestValidator());
    }
}
#endregion

#region --- HANDLER ---
// The owner Credit Officer or the Head of the branch can edit a group at any time. Code, branch, survey, Credit Officer
// and Formation Date stay as they are. Editing an approved group needs no new approval; a Rejected group goes back to Pending.
internal sealed class UpdateGroupCommandHandler(
    QubeFinDataContext context,
    ISurveyRepository surveyRepository,
    IGroupRepository groupRepository,
    INotificationRepository notificationRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateGroupCommand, Result<string>>
{
    private const int GpsSrid = 4326;

    public async Task<Result<string>> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
    {
        var input = request.Group;

        var group = await groupRepository.GetByIdAsync(request.Id);
        if (group is null)
            return new RecordNotFoundError("Group not found.");

        var rights = await GroupAccess.GetRightsAsync(context, surveyRepository, groupRepository, group, request.UserId, request.EmployeeId, cancellationToken);
        if (!rights.CanEdit)
            return new ForbiddenError("Only the Credit Officer of the group or the Head of the branch can edit it.");

        if (!await context.TblAdministrativeUnits.AnyAsync(a => a.Id == input.AdministrativeUnitId, cancellationToken))
            return new ValidationError("The selected area was not found.");

        var name = input.Name.Trim();
        if (group.BranchCode.HasValue && await groupRepository.NameExistsAsync(group.BranchCode.Value, name, group.Id))
            return new ValidationError("A group with this name already exists in the branch.");

        group.Name = name;
        group.CollectionDay = input.CollectionDay;
        group.CollectionTime = input.CollectionTime;
        group.BiweeklyStartDate = input.BiweeklyStartDate;
        group.MobileNo = input.MobileNo.Trim();
        group.AdministrativeUnitId = input.AdministrativeUnitId;
        group.PinCode = string.IsNullOrWhiteSpace(input.PinCode) ? null : input.PinCode.Trim();
        group.Address = string.IsNullOrWhiteSpace(input.Address) ? null : input.Address.Trim();
        group.Location = input.Latitude.HasValue && input.Longitude.HasValue
            ? new Point((double)input.Longitude.Value, (double)input.Latitude.Value) { SRID = GpsSrid }
            : null;
        group.LandMark = string.IsNullOrWhiteSpace(input.LandMark) ? null : input.LandMark.Trim();
        group.LastModifiedBy = request.UserId;
        group.LastModifiedOn = DateTime.Now;

        var resubmitted = (group.CurrentStatus ?? string.Empty).Trim() == GroupStatuses.Rejected;
        if (resubmitted)
        {
            group.CurrentStatus = GroupStatuses.Pending;
            group.IsApproved = false;
            group.VerifiedBy = null;
            group.VerifiedOn = null;
            group.VerifiedRemarks = null;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (resubmitted && rights.Branch is not null)
        {
            var recipients = (await surveyRepository.GetHeadEmployeeIdsOfBranchAsync(rights.Branch.Id))
                .Concat(await surveyRepository.GetApproverEmployeeIdsOfBranchAsync(rights.Branch.Id))
                .Where(id => id != request.EmployeeId)
                .Distinct()
                .ToList();
            if (recipients.Count > 0)
            {
                await notificationRepository.AddForEmployeesAsync(
                    recipients,
                    title: "Group Waiting For Approval",
                    message: $"Group {name} ({rights.Branch.Name}) was corrected and needs approval again.",
                    icon: "users",
                    referenceId: group.Id,
                    referenceType: "GROUP",
                    actionUrl: "/secure/global/groups",
                    createdBy: request.UserId);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }

        return Result.Ok(resubmitted ? "Group updated and sent for approval again." : "Group updated successfully.");
    }
}
#endregion
