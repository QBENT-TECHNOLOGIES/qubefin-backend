using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Global.Application.SurveyCommittees.Models;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Global.Application.SurveyCommittees.Commands;

#region --- COMMAND ---
public record UpdateMemberCommand(MemberUpdateRequest member, Guid UserId) : IRequest<Result<UpdateMemberResponse>>;
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

#region --- RESPONSE ---
public record UpdateMemberResponse(bool Created);
#endregion

#region --- HANDLER ---
internal sealed class UpdateMemberCommandHandler(ISurveyCommitteeRepository surveyCommitteeRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateMemberCommand, Result<UpdateMemberResponse>>
{
    public async Task<Result<UpdateMemberResponse>> Handle(UpdateMemberCommand request, CancellationToken cancellationToken)
    {
        var existingSurveyCommitteeMember = await surveyCommitteeRepository.GetByIdAsync(request.member.Id);
        if (existingSurveyCommitteeMember is null) return new RecordNotFoundError("Member not found");

        existingSurveyCommitteeMember.Update(isActive: request.member.IsActive, isLead: request.member.IsLead, assignedTo: request.member.AssignedTo, lastModifiedBy: request.UserId);
        await surveyCommitteeRepository.UpdateMember(existingSurveyCommitteeMember);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(new UpdateMemberResponse(true));
    }
}
#endregion
