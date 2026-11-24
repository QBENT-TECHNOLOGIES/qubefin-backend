using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Global.Application.Groups.Models;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.Groups.Queries;

#region --- QUERY ---
// Tells the screen whether to show "Add Group", for which branch, and the Credit Officers and approved surveys to choose from.
public record GetGroupAddInfoQuery(Guid EmployeeId) : IRequest<Result<GroupAddInfoResponse>>;
#endregion

#region --- HANDLER ---
internal sealed class GetGroupAddInfoQueryHandler(
    QubeFinDataContext context,
    ISurveyRepository surveyRepository,
    IGroupRepository groupRepository) : IRequestHandler<GetGroupAddInfoQuery, Result<GroupAddInfoResponse>>
{
    public async Task<Result<GroupAddInfoResponse>> Handle(GetGroupAddInfoQuery request, CancellationToken cancellationToken)
    {
        var response = new GroupAddInfoResponse();

        // The Head of a branch adds for any Credit Officer of the branch; a Credit Officer adds for himself, in his own branch.
        var headBranch = await surveyRepository.GetHeadBranchAsync(request.EmployeeId);
        Guid branchId;
        if (headBranch is not null)
        {
            response.IsHead = true;
            response.BranchId = headBranch.Id;
            response.BranchName = headBranch.Name;
            branchId = headBranch.Id;
            response.CreditOfficers = (await groupRepository.GetCreditOfficerPostsOfBranchAsync(branchId))
                .Select(p => new GroupCreditOfficerOption { DesignationId = p.DesignationId, Name = p.EmployeeName })
                .OrderBy(p => p.Name)
                .ToList();
        }
        else
        {
            var post = await groupRepository.GetCreditOfficerPostOfEmployeeAsync(request.EmployeeId);
            if (post is null)
                return Result.Ok(response);

            branchId = post.BranchId;
            response.BranchId = post.BranchId;
            response.BranchName = post.BranchName;
            response.CreditOfficers = [new GroupCreditOfficerOption { DesignationId = post.DesignationId, Name = post.EmployeeName }];
        }

        response.CanAdd = true;

        // Approved Group Surveys of the branch that have no group yet (a Credit Officer sees the ones he was assigned to).
        var isHead = response.IsHead;
        var employeeId = request.EmployeeId;
        var surveys = await context.TblSurveys.AsNoTracking()
            .Where(s => s.SurveyType == SurveyTypes.Group
                && s.Status == SurveyStatuses.Approved
                && s.BranchOrganizationUnitId == branchId
                && !context.TblGroups.Any(g => g.SurveyId == s.Id)
                // Only the Credit Officers of the survey create a group from it; the Head adds manually.
                && !isHead
                && s.TblSurveyAssigneds.Any(a => a.EmployeeId == employeeId))
            .OrderByDescending(s => s.AssignmentDate)
            .Select(s => new
            {
                s.Id, s.AssignmentNo, s.ProposedArea, s.AdministrativeUnitId,
                Form = context.TblGroupSurveys.Where(f => f.SurveyId == s.Id)
                    .Select(f => new { f.Location, f.NearestLandmark }).FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        response.Surveys = surveys.Select(s => new GroupSurveyOption
        {
            Id = s.Id,
            AssignmentNo = s.AssignmentNo,
            ProposedArea = s.ProposedArea,
            AdministrativeUnitId = s.AdministrativeUnitId,
            Latitude = s.Form?.Location != null ? (decimal)s.Form.Location.Coordinate.Y : null,
            Longitude = s.Form?.Location != null ? (decimal)s.Form.Location.Coordinate.X : null,
            LandMark = s.Form?.NearestLandmark
        }).ToList();

        return Result.Ok(response);
    }
}
#endregion
