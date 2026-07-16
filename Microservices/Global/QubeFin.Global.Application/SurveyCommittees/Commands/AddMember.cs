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
public record AddMemberCommand(MemberAddRequest member, Guid UserId) : IRequest<Result<AddMemberResponse>>;
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
#region --- RESPONSE ---
public record AddMemberResponse(bool Created);
#endregion

#region --- HANDLER ---
internal sealed class AddMemberCommandHandler(ISurveyCommitteeRepository surveyCommitteeRepository, IUnitOfWork unitOfWork) : IRequestHandler<AddMemberCommand, Result<AddMemberResponse>>
{
    public async Task<Result<AddMemberResponse>> Handle(AddMemberCommand request, CancellationToken cancellationToken)
    {
        DateOnly toDay = DateOnly.FromDateTime(DateTime.UtcNow);

        var duplicateMember = await surveyCommitteeRepository.ExistsByEmployeeIdAsync(request.member.EmployeeId);
        if (duplicateMember) return new ValidationError("This member is already in survey committee.");

        var surveyCommitte = SurveyCommittee.Create(Guid.NewGuid(), request.member.EmployeeId, request.member.IsLead, toDay, request.UserId);

        await surveyCommitteeRepository.AddMember(surveyCommitte);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(new AddMemberResponse(true));
    }
}
#endregion
