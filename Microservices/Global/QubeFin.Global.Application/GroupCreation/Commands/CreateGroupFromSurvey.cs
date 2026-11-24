using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using QubeFin.Core.Results;
using QubeFin.Global.Application.Grt;
using QubeFin.Global.Application.Groups.Models;
using QubeFin.Global.Application.Mobilization;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.GroupCreation.Commands;

#region --- COMMAND ---
public record CreateGroupFromSurveyCommand(Guid SurveyId, CreateGroupFromSurveyRequest Group, Guid UserId, Guid EmployeeId) : IRequest<Result<string>>;
#endregion

#region --- VALIDATOR ---
public class CreateGroupFromSurveyCommandValidator : AbstractValidator<CreateGroupFromSurveyCommand>
{
    public CreateGroupFromSurveyCommandValidator()
    {
        RuleFor(v => v.SurveyId).NotEmpty().WithMessage("Survey Id is required.");
        RuleFor(v => v.Group).NotNull().SetValidator(new CreateGroupFromSurveyRequestValidator());
    }
}
#endregion

#region --- HANDLER ---
// After the GRT is submitted as Pass, a Credit Officer of the survey creates the group in one step: the group,
// its eligible borrowers (linked to it, with the proposed loan, purpose and remarks) and its three leaders.
// The group belongs to the Credit Officer's designation and is Approved at once (no approval for a survey group).
internal sealed class CreateGroupFromSurveyCommandHandler(
    QubeFinDataContext context,
    IGroupRepository groupRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateGroupFromSurveyCommand, Result<string>>
{
    private const int GpsSrid = 4326;

    public async Task<Result<string>> Handle(CreateGroupFromSurveyCommand request, CancellationToken cancellationToken)
    {
        var input = request.Group;

        var survey = await context.TblSurveys.AsNoTracking()
            .Include(s => s.TblSurveyAssigneds)
            .FirstOrDefaultAsync(s => s.Id == request.SurveyId, cancellationToken);
        if (survey is null)
            return new RecordNotFoundError("Survey not found.");
        if (survey.SurveyType != SurveyTypes.Group || survey.Status != SurveyStatuses.Approved || survey.BranchOrganizationUnitId is null)
            return new ValidationError("A group is created only from an Approved Group Survey.");
        if (!survey.TblSurveyAssigneds.Any(a => a.EmployeeId == request.EmployeeId))
            return new ForbiddenError("Only the Credit Officers of this survey can create the group.");

        var grt = await context.TblGroupGrts.AsNoTracking().FirstOrDefaultAsync(g => g.SurveyId == survey.Id, cancellationToken);
        if (grt is null || grt.Status != GrtStatuses.Completed)
            return new ValidationError("The group can be created after the GRT is submitted.");
        if (grt.OverallRecommendation != "Pass")
            return new ValidationError("The GRT is failed. A group cannot be created from this survey.");

        if (await context.TblGroups.AnyAsync(g => g.SurveyId == survey.Id, cancellationToken))
            return new ValidationError("A group is already created for this survey.");

        // The Responsible Credit Officer: the creator, at this branch.
        var post = await groupRepository.GetCreditOfficerPostOfEmployeeAsync(request.EmployeeId);
        if (post is null || post.BranchId != survey.BranchOrganizationUnitId)
            return new ForbiddenError("You must be a Credit Officer of this branch to create the group.");

        var branchCode = await groupRepository.GetBranchCodeAsync(post.BranchId);
        if (branchCode is null)
            return new ValidationError("The branch has no code.");

        var name = input.Name.Trim();
        if (await groupRepository.NameExistsAsync(branchCode.Value, name, null))
            return new ValidationError("A group with this name already exists in the branch.");

        // The chosen borrowers must be Pending borrowers of this survey that are not in a group yet.
        var borrowerIds = input.Members.Select(m => m.BorrowerId).ToList();
        var borrowers = await context.TblGroupBorrowers
            .Where(b => b.SurveyId == survey.Id && borrowerIds.Contains(b.Id))
            .ToListAsync(cancellationToken);
        if (borrowers.Count != borrowerIds.Count)
            return new ValidationError("A selected borrower was not found in this survey.");
        if (borrowers.Any(b => b.Status != BorrowerStatuses.Pending || b.GroupId != null))
            return new ValidationError("A selected borrower is blocked or is already in a group. Reload and select again.");

        var now = DateTime.Now;
        var group = new TblGroup
        {
            Id = Guid.NewGuid(),
            SurveyId = survey.Id,
            BranchCode = branchCode.Value,
            // The sequence (1, 2, 3 ...) restarts for each branch; the database builds Code from BranchCode and CodeVal.
            CodeVal = await groupRepository.GetNextCodeValAsync(branchCode.Value),
            Name = name,
            CollectionDay = input.CollectionDay,
            CollectionTime = input.CollectionTime,
            FormationDate = input.FormationDate,
            MobileNo = input.MobileNo.Trim(),
            // The area is the area of the survey.
            AdministrativeUnitId = survey.AdministrativeUnitId,
            PinCode = string.IsNullOrWhiteSpace(input.PinCode) ? null : input.PinCode.Trim(),
            Address = string.IsNullOrWhiteSpace(input.Address) ? null : input.Address.Trim(),
            Location = input.Latitude.HasValue && input.Longitude.HasValue
                ? new Point((double)input.Longitude.Value, (double)input.Latitude.Value) { SRID = GpsSrid }
                : null,
            LandMark = string.IsNullOrWhiteSpace(input.LandMark) ? null : input.LandMark.Trim(),
            SittingPlace = input.SittingPlace.Trim(),
            PoliceStation = string.IsNullOrWhiteSpace(input.PoliceStation) ? null : input.PoliceStation.Trim(),
            PostOffice = string.IsNullOrWhiteSpace(input.PostOffice) ? null : input.PostOffice.Trim(),
            // A group made from a survey needs no approval (Phase 13 decision).
            IsApproved = true,
            CurrentStatus = GroupStatuses.Approved,
            DesignationId = post.DesignationId,
            CreatedBy = request.UserId,
            CreatedOn = now
        };
        await groupRepository.AddAsync(group);

        // The eligible borrowers join the group: loan details and, for three of them, the leader role.
        foreach (var member in input.Members)
        {
            var borrower = borrowers.First(b => b.Id == member.BorrowerId);
            borrower.GroupId = group.Id;
            borrower.ProposedLoanAmount = member.ProposedLoanAmount;
            borrower.LoanPurpose = member.LoanPurpose.Trim();
            borrower.EligibleRemarks = string.IsNullOrWhiteSpace(member.Remarks) ? null : member.Remarks.Trim();
            borrower.GroupRole = member.Role;
            borrower.AppointmentDate = member.Role is null ? null : member.AppointmentDate;
            borrower.LastModifiedBy = request.UserId;
            borrower.LastModifiedOn = now;
        }

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            // Two people took the same next number, or created a group for the same survey, at the same moment.
            return new ValidationError("The group could not be saved. Please try again.");
        }

        return Result.Ok("Group created successfully.");
    }
}
#endregion
