using FluentResults;
using MediatR;
using QubeFin.Global.Persistence.Repositories;

namespace QubeFin.Global.Application.SurveyUnit.Queries;

#region --- QUERY ---
// Tells the screen whether to show the "Add Survey" button and which survey types the user can create.
public record CanCreateSurveyQuery(Guid EmployeeId) : IRequest<Result<CanCreateSurveyResponse>>;

public class CanCreateSurveyResponse
{
    // The Committee Lead creates Branch Surveys.
    public bool CanCreateBranchSurvey { get; set; }
    // Whoever is in charge of a branch creates Group Surveys for that branch.
    public bool CanCreateGroupSurvey { get; set; }
    public Guid? InchargeBranchId { get; set; }
    public string? InchargeBranchName { get; set; }
}
#endregion

#region --- HANDLER ---
internal sealed class CanCreateSurveyQueryHandler(
    ISurveyCommitteeRepository surveyCommitteeRepository,
    ISurveyRepository surveyRepository) : IRequestHandler<CanCreateSurveyQuery, Result<CanCreateSurveyResponse>>
{
    public async Task<Result<CanCreateSurveyResponse>> Handle(CanCreateSurveyQuery request, CancellationToken cancellationToken)
    {
        var branch = await surveyRepository.GetInchargeBranchAsync(request.EmployeeId);
        return Result.Ok(new CanCreateSurveyResponse
        {
            CanCreateBranchSurvey = await surveyCommitteeRepository.IsActiveLeadAsync(request.EmployeeId),
            CanCreateGroupSurvey = branch is not null,
            InchargeBranchId = branch?.Id,
            InchargeBranchName = branch?.Name
        });
    }
}
#endregion
