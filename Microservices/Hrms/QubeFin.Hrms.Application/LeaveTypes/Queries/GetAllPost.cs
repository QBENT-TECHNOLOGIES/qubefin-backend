using FluentResults;
using MediatR;
using QubeFin.Hrms.Persistence.Repositories;

namespace QubeFin.Hrms.Application.LeaveTypes.Queries;

public record GetAllPostQuery() : IRequest<Result<List<GetAllPostResponse>>>;

public record GetAllPostResponse(Guid Id, string Name, bool IsActive);
internal sealed class GetAllPostQueryHandler(IApprovalWorkflowEventRepository ApprovalWorkflowEventRepository) : IRequestHandler<GetAllPostQuery, Result<List<GetAllPostResponse>>>
{
    public async Task<Result<List<GetAllPostResponse>>> Handle(GetAllPostQuery request, CancellationToken cancellationToken)
    {
        var salaryGrade = await ApprovalWorkflowEventRepository.GetAllPost();
        return Result.Ok(salaryGrade.Select(m => new GetAllPostResponse(m.Id, m.Name, m.IsActive)).OrderBy(m => m.Name).ToList());
    }
}