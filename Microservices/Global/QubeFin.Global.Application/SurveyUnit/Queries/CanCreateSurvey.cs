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
    // The Head of a branch (person in charge, else the Branch Manager) creates Group Surveys for that branch.
    public bool CanCreateGroupSurvey { get; set; }
    public Guid? InchargeBranchId { get; set; }
    public string? InchargeBranchName { get; set; }
    // The company of that branch: a Group Survey takes it (the form shows it and it cannot be changed).
    public Guid? InchargeBranchCompanyId { get; set; }
    public string? InchargeBranchCompanyName { get; set; }
}
#endregion

#region --- HANDLER ---
internal sealed class CanCreateSurveyQueryHandler(
    ISurveyCommitteeRepository surveyCommitteeRepository,
    ISurveyRepository surveyRepository) : IRequestHandler<CanCreateSurveyQuery, Result<CanCreateSurveyResponse>>
{
    public async Task<Result<CanCreateSurveyResponse>> Handle(CanCreateSurveyQuery request, CancellationToken cancellationToken)
    {
        var branch = await surveyRepository.GetHeadBranchAsync(request.EmployeeId);
        var company = branch is null ? null : await surveyRepository.GetBranchCompanyAsync(branch.Id);
        return Result.Ok(new CanCreateSurveyResponse
        {
            CanCreateBranchSurvey = await surveyCommitteeRepository.IsActiveLeadAsync(request.EmployeeId),
            CanCreateGroupSurvey = branch is not null,
            InchargeBranchId = branch?.Id,
            InchargeBranchName = branch?.Name,
            InchargeBranchCompanyId = company?.Id,
            InchargeBranchCompanyName = company?.Name
        });
    }
}
#endregion
