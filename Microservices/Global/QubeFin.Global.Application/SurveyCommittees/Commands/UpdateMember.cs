using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Global.Application.SurveyCommittees.Models;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Global.Application.SurveyCommittees.Commands;

#region --- COMMAND ---
public record UpdateMemberCommand(MemberUpdateRequest member, Guid UserId) : IRequest<Result<string>>;
#endregion

#region --- VALIDATOR ---
public class UpdateMemberCommandValidator : AbstractValidator<UpdateMemberCommand>
{
    public UpdateMemberCommandValidator()
    {
        RuleFor(v => v.member.Id).NotEmpty().WithMessage("Committee Id is required.");
        RuleFor(v => v.member.AssignedTo).NotEmpty().WithMessage("Separation Date is required.").When( w => w.member.IsActive == false);
    }
}
#endregion

#region --- HANDLER ---
internal sealed class UpdateMemberCommandHandler(ISurveyCommitteeRepository surveyCommitteeRepository, ISurveyRepository surveyRepository, IUnitOfWork unitOfWork) :
    IRequestHandler<UpdateMemberCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateMemberCommand request, CancellationToken cancellationToken)
    {
        // Only the super admin can manage the committee.
        if (!await surveyCommitteeRepository.IsSuperAdminAsync(request.UserId))
            return new ForbiddenError("Only the super admin can manage the Survey Committee.");

        var existingSurveyCommitteeMember = await surveyCommitteeRepository.GetByIdAsync(request.member.Id);
        if (existingSurveyCommitteeMember is null) return new RecordNotFoundError("Member not found");

        // Making a former member active again follows the same rules as adding a new member.
        if (!existingSurveyCommitteeMember.IsActive && request.member.IsActive)
        {
            if (await surveyCommitteeRepository.ExistsByEmployeeIdAsync(existingSurveyCommitteeMember.EmployeeId))
                return new ValidationError("This member is already in survey committee.");
            if (await surveyRepository.IsAssignedToOpenSurveyAsync(existingSurveyCommitteeMember.EmployeeId))
                return new ValidationError("This employee is assigned to an open survey and cannot join the committee.");
        }

        existingSurveyCommitteeMember.Update(
            isActive: request.member.IsActive, 
            isLead: request.member.IsActive ? request.member.IsLead : false, 
            assignedTo: !request.member.IsActive ? request.member.AssignedTo : null, 
            lastModifiedBy: request.UserId);

        await surveyCommitteeRepository.UpdateMember(existingSurveyCommitteeMember);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok("Committee member updated successfully.");
    }
}
#endregion
