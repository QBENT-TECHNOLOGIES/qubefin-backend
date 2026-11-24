using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Global.Application.Groups.Models;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.Groups.Queries;

#region --- QUERY ---
// Tells the screen whether to show "Add Group", for which branch, and the Credit Officers to choose from.
public record GetGroupAddInfoQuery(Guid EmployeeId) : IRequest<Result<GroupAddInfoResponse>>;
#endregion

#region --- HANDLER ---
internal sealed class GetGroupAddInfoQueryHandler(
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

        return Result.Ok(response);
    }
}
#endregion
