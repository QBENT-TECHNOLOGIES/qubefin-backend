using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Global.Application.SurveyCommittees.Models;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.SurveyCommittees.Commands;

#region --- COMMAND ---
public record AddMemberCommand(MemberAddRequest member, Guid UserId) : IRequest<Result<string>>;
#endregion

#region --- VALIDATOR ---
public class AddMemberCommandValidator : AbstractValidator<AddMemberCommand>
{
    public AddMemberCommandValidator()
    {
        RuleFor(v => v.member.EmployeeId).NotEmpty().WithMessage("Employee Id is required.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class AddMemberCommandHandler(ISurveyCommitteeRepository surveyCommitteeRepository, ISurveyRepository surveyRepository, IUnitOfWork unitOfWork) :
    IRequestHandler<AddMemberCommand, Result<string>>
{
    public async Task<Result<string>> Handle(AddMemberCommand request, CancellationToken cancellationToken)
    {
        DateOnly toDay = DateOnly.FromDateTime(DateTime.UtcNow);

        // Only the super admin can manage the committee.
        if (!await surveyCommitteeRepository.IsSuperAdminAsync(request.UserId))
            return new ForbiddenError("Only the super admin can manage the Survey Committee.");

        var duplicateMember = await surveyCommitteeRepository.ExistsByEmployeeIdAsync(request.member.EmployeeId);
        if (duplicateMember) return new ValidationError("This member is already in survey committee.");

        // An employee who is assigned to an open survey as a surveyor cannot join the committee.
        if (await surveyRepository.IsAssignedToOpenSurveyAsync(request.member.EmployeeId))
            return new ValidationError("This employee is assigned to an open survey and cannot join the committee.");

        var surveyCommitte = SurveyCommittee.Create(Guid.NewGuid(), request.member.EmployeeId, request.member.IsLead, toDay, request.UserId);

        await surveyCommitteeRepository.AddMember(surveyCommitte);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok("Committee member added successfully.");
    }
}
#endregion
