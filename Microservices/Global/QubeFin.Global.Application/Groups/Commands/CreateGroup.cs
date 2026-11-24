using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using QubeFin.Core.Results;
using QubeFin.Global.Application.Groups.Models;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.Groups.Commands;

#region --- COMMAND ---
public record CreateGroupCommand(GroupRequest Group, Guid UserId, Guid EmployeeId) : IRequest<Result<string>>;
#endregion

#region --- VALIDATOR ---
public class CreateGroupCommandValidator : AbstractValidator<CreateGroupCommand>
{
    public CreateGroupCommandValidator()
    {
        RuleFor(v => v.Group).NotNull().SetValidator(new GroupRequestValidator());
    }
}
#endregion

#region --- HANDLER ---
// The Credit Officer (for himself) or the Head of the branch (for a Credit Officer of the branch) adds a group manually.
// The group starts as Pending and waits for approval. A group made from a survey is created by the Group Creation command.
internal sealed class CreateGroupCommandHandler(
    QubeFinDataContext context,
    ISurveyRepository surveyRepository,
    IGroupRepository groupRepository,
    INotificationRepository notificationRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateGroupCommand, Result<string>>
{
    private const int GpsSrid = 4326;

    public async Task<Result<string>> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        var input = request.Group;

        var adder = await GroupAccess.GetAdderAsync(surveyRepository, groupRepository, request.EmployeeId);
        if (adder is null)
            return new ForbiddenError("Only a Credit Officer or the Head of a branch can add a group.");

        if (!adder.CreditOfficers.Any(c => c.DesignationId == input.DesignationId))
            return new ValidationError(adder.IsHead
                ? "Select a Credit Officer of your branch."
                : "A Credit Officer can add a group only for himself.");

        var branchCode = await groupRepository.GetBranchCodeAsync(adder.BranchId);
        if (branchCode is null)
            return new ValidationError("The branch has no code.");

        if (!await context.TblAdministrativeUnits.AnyAsync(a => a.Id == input.AdministrativeUnitId, cancellationToken))
            return new ValidationError("The selected area was not found.");

        var name = input.Name.Trim();
        if (await groupRepository.NameExistsAsync(branchCode.Value, name, null))
            return new ValidationError("A group with this name already exists in the branch.");

        var now = DateTime.Now;
        var group = new TblGroup
        {
            Id = Guid.NewGuid(),
            BranchCode = branchCode.Value,
            // The sequence (1, 2, 3 ...) restarts for each branch; the database builds Code from BranchCode and CodeVal.
            CodeVal = await groupRepository.GetNextCodeValAsync(branchCode.Value),
            Name = name,
            CollectionDay = input.CollectionDay,
            CollectionTime = input.CollectionTime,
            BiweeklyStartDate = input.BiweeklyStartDate,
            FormationDate = input.FormationDate,
            MobileNo = input.MobileNo.Trim(),
            AdministrativeUnitId = input.AdministrativeUnitId,
            PinCode = string.IsNullOrWhiteSpace(input.PinCode) ? null : input.PinCode.Trim(),
            Address = string.IsNullOrWhiteSpace(input.Address) ? null : input.Address.Trim(),
            Location = input.Latitude.HasValue && input.Longitude.HasValue
                ? new Point((double)input.Longitude.Value, (double)input.Latitude.Value) { SRID = GpsSrid }
                : null,
            LandMark = string.IsNullOrWhiteSpace(input.LandMark) ? null : input.LandMark.Trim(),
            SittingPlace = string.IsNullOrWhiteSpace(input.SittingPlace) ? null : input.SittingPlace.Trim(),
            PoliceStation = string.IsNullOrWhiteSpace(input.PoliceStation) ? null : input.PoliceStation.Trim(),
            PostOffice = string.IsNullOrWhiteSpace(input.PostOffice) ? null : input.PostOffice.Trim(),
            // A manually added group waits for approval. A group made from a survey is created in the Group Creation window.
            IsApproved = false,
            CurrentStatus = GroupStatuses.Pending,
            DesignationId = input.DesignationId,
            CreatedBy = request.UserId,
            CreatedOn = now
        };

        await groupRepository.AddAsync(group);

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            // Two people took the same next number, or the same survey, at the same moment.
            return new ValidationError("The group could not be saved. Please try again.");
        }

        // The Head and the approver of the branch are told (not the person who added it).
        var recipients = (await surveyRepository.GetHeadEmployeeIdsOfBranchAsync(adder.BranchId))
            .Concat(await surveyRepository.GetApproverEmployeeIdsOfBranchAsync(adder.BranchId))
            .Where(id => id != request.EmployeeId)
            .Distinct()
            .ToList();
        if (recipients.Count > 0)
        {
            await notificationRepository.AddForEmployeesAsync(
                recipients,
                title: "Group Waiting For Approval",
                message: $"Group {name} ({adder.BranchName}) was added and needs approval.",
                icon: "users",
                referenceId: group.Id,
                referenceType: "GROUP",
                actionUrl: "/secure/global/groups",
                createdBy: request.UserId);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Ok("Group added successfully. It is waiting for approval.");
    }
}
#endregion
