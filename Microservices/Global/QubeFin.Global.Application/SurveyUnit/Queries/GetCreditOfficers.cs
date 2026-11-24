using FluentResults;
using MediatR;
using QubeFin.Global.Persistence.Repositories;

namespace QubeFin.Global.Application.SurveyUnit.Queries;

#region --- QUERY ---
// The Credit Officers of the branch the user is in charge of, who can be assigned to a new Group Survey.
public record GetCreditOfficersQuery(Guid EmployeeId) : IRequest<Result<List<CreditOfficerResponse>>>;

public class CreditOfficerResponse
{
    public Guid Id { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
}
#endregion

#region --- HANDLER ---
internal sealed class GetCreditOfficersQueryHandler(
    ISurveyRepository surveyRepository,
    ISurveyCommitteeRepository committeeRepository) : IRequestHandler<GetCreditOfficersQuery, Result<List<CreditOfficerResponse>>>
{
    public async Task<Result<List<CreditOfficerResponse>>> Handle(GetCreditOfficersQuery request, CancellationToken cancellationToken)
    {
        var branch = await surveyRepository.GetHeadBranchAsync(request.EmployeeId);
        if (branch is null)
            return Result.Ok(new List<CreditOfficerResponse>());

        // Same rules as the server check on save: not himself, not a committee member, not on another open survey.
        var result = new List<CreditOfficerResponse>();
        foreach (var officer in await surveyRepository.GetCreditOfficersOfBranchAsync(branch.Id))
        {
            if (officer.EmployeeId == request.EmployeeId) continue;
            if (await committeeRepository.AnyActiveMemberAsync(new[] { officer.EmployeeId })) continue;
            if (await surveyRepository.IsAssignedToOpenSurveyAsync(officer.EmployeeId)) continue;
            result.Add(new CreditOfficerResponse { Id = officer.EmployeeId, EmployeeName = officer.EmployeeName });
        }
        return Result.Ok(result);
    }
}
#endregion
