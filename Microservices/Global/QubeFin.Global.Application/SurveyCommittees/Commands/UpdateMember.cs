using MediatR;
using FluentResults;
using QubeFin.Persistence;
using QubeFin.Core.Results;
using QubeFin.Global.Persistence.Repositories;

namespace QubeFin.Global.Application.SurveyCommittees.Commands;

#region --- COMMAND ---
public record UpdateMemberCommand(Guid Id, Guid EmployeId, bool IsActive, bool IsLead, DateOnly AssignTo, Guid UserId) : IRequest<Result<UpdateMemberResponse>>;
#endregion

#region --- RESPONSE ---
public record UpdateMemberResponse(bool Created);
#endregion

#region --- HANDLER ---
internal sealed class UpdateMemberCommandHandler(ISurveyCommitteeRepository surveyCommitteeRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateMemberCommand, Result<UpdateMemberResponse>>
{
    public async Task<Result<UpdateMemberResponse>> Handle(UpdateMemberCommand request, CancellationToken cancellationToken)
    {
        var existingSurveyCommitteeMember = await surveyCommitteeRepository.GetByIdAsync(request.Id);
        if (existingSurveyCommitteeMember is null) return new RecordNotFoundError("Member not found");

        existingSurveyCommitteeMember.Update(isActive: request.IsActive, isLead: request.IsLead, assignedTo: request.AssignTo, lastModifiedBy: request.UserId);
        await surveyCommitteeRepository.UpdateMember(existingSurveyCommitteeMember);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(new UpdateMemberResponse(true));
    }
}
#endregion
