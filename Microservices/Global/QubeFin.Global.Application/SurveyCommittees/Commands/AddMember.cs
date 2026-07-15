using MediatR;
using FluentResults;
using QubeFin.Persistence;
using QubeFin.Core.Results;
using QubeFin.Persistence.Models.Global;
using QubeFin.Global.Persistence.Repositories;

namespace QubeFin.Global.Application.SurveyCommittees.Commands;

#region --- COMMAND ---
public record AddMemberCommand(Guid EmployeeId, bool IsLead, Guid UserId) : IRequest<Result<AddMemberResponse>>;
#endregion

#region --- RESPONSE ---
public record AddMemberResponse(bool Created);
#endregion

#region --- HANDLER ---
internal sealed class AddMemberCommandHandler(ISurveyCommitteeRepository surveyCommitteeRepository, IUnitOfWork unitOfWork) : IRequestHandler<AddMemberCommand, Result<AddMemberResponse>>
{
    public async Task<Result<AddMemberResponse>> Handle(AddMemberCommand request, CancellationToken cancellationToken)
    {
        DateOnly toDay = DateOnly.FromDateTime(DateTime.UtcNow);

        var duplicateMember = await surveyCommitteeRepository.ExistsByEmployeeIdAsync(request.EmployeeId);
        if (duplicateMember) return new ValidationError("This member is already in survey committee.");

        var surveyCommitte = SurveyCommittee.Create(Guid.NewGuid(), request.EmployeeId, request.IsLead, toDay, request.UserId);

        await surveyCommitteeRepository.AddMember(surveyCommitte);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(new AddMemberResponse(true));
    }
}
#endregion
