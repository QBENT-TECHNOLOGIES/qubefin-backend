using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Global.Application.BranchSurveys.Models;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.BranchSurveys.Commands;

#region --- COMMAND ---
public record CreateBranchFromSurveyCommand(Guid SurveyId, CreateBranchRequest Branch, Guid UserId, Guid EmployeeId) : IRequest<Result<string>>;
#endregion

#region --- VALIDATOR ---
public class CreateBranchFromSurveyCommandValidator : AbstractValidator<CreateBranchFromSurveyCommand>
{
    public CreateBranchFromSurveyCommandValidator()
    {
        RuleFor(v => v.SurveyId).NotEmpty().WithMessage("Survey Id is required.");
        RuleFor(v => v.Branch).NotNull().SetValidator(new CreateBranchRequestValidator());
    }
}
#endregion

#region --- HANDLER ---
// Creates the Branch (an Organization Unit of type Branch) from an approved Branch Survey, in one save.
internal sealed class CreateBranchFromSurveyCommandHandler(
    ISurveyRepository surveyRepository,
    IBranchSurveyRepository branchSurveyRepository,
    ISurveyCommitteeRepository committeeRepository,
    IOrganizationUnitRepository organizationUnitRepository,
    INotificationRepository notificationRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateBranchFromSurveyCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CreateBranchFromSurveyCommand request, CancellationToken cancellationToken)
    {
        var survey = await surveyRepository.GetByIdAsync(request.SurveyId);
        if (survey is null)
            return new RecordNotFoundError("Survey not found.");

        if (survey.SurveyType != SurveyTypes.Branch)
            return new ValidationError("A branch can be created only from a Branch Survey.");

        if (survey.Status != SurveyStatuses.Approved || survey.BranchOrganizationUnitId.HasValue)
            return new ValidationError("A branch can be created only once, from an approved survey.");

        if (!await committeeRepository.IsActiveLeadAsync(request.EmployeeId))
            return new ForbiddenError("Only the Committee Lead can create the branch.");

        var branchTypeId = await organizationUnitRepository.GetBranchTypeIdAsync(cancellationToken);
        if (branchTypeId is null)
            return new ValidationError("The organization unit type \"Branch\" was not found.");

        // The district is pre-filled from the survey area, but the Lead can change it.
        if (!await organizationUnitRepository.IsDistrictAsync(request.Branch.DistrictId, cancellationToken))
            return new ValidationError("The selected district is not valid.");

        var name = request.Branch.Name.Trim();
        if (await organizationUnitRepository.ExistsByNameAsync(name, request.Branch.ParentId, null, cancellationToken))
            return new ValidationError($"Organization Unit {name} already exists under the selected parent.");

        var branchSurvey = await branchSurveyRepository.GetBySurveyIdAsync(request.SurveyId);
        if (branchSurvey is null)
            return new ValidationError("The survey form has not been filled in.");

        // The code is generated, like on the Organization Unit page.
        var codeVal = await organizationUnitRepository.GetNextCodeValAsync(cancellationToken);
        var branch = OrganizationUnit.Create(
            Guid.NewGuid(), branchTypeId.Value, name, codeVal,
            request.Branch.ParentId, request.Branch.CompanyId, request.Branch.DistrictId,
            request.Branch.Latitude, request.Branch.Longitude,
            request.Branch.AttendanceInTime, request.Branch.AttendanceOutTime, request.Branch.CheckRadiusInMeter,
            request.UserId);
        await organizationUnitRepository.AddAsync(branch);

        branchSurvey.IsBranchCreate = true;
        branchSurvey.LastModifiedBy = request.UserId;
        branchSurvey.LastModifiedOn = DateTime.Now;
        await surveyRepository.SetBranchCreatedAsync(request.SurveyId, branch.Id, request.UserId);

        // Tell the surveyors and the committee.
        var recipients = survey.SurveyAssigneds.Select(a => a.EmployeeId)
            .Concat(await committeeRepository.GetActiveMemberEmployeeIdsAsync())
            .Distinct().ToList();
        await notificationRepository.AddForEmployeesAsync(
            recipients,
            title: "Branch Created",
            message: $"Branch {name} was created from survey {survey.AssignmentNo}.",
            icon: "building-2",
            referenceId: survey.Id,
            referenceType: "SURVEY",
            actionUrl: "/secure/global/surveys",
            createdBy: request.UserId);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok($"Branch {name} created successfully.");
    }
}
#endregion
