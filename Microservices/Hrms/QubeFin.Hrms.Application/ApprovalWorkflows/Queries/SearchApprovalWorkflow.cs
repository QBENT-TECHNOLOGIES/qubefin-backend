using FluentResults;
using MediatR;
using QubeFin.Hrms.Application.ApprovalWorkflows.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.ApprovalWorkflows.Queries;

public record SearchApprovalWorkflowQuery(ApprovalWorkflowSearchRequest filterParam)
    : IRequest<Result<SearchApprovalWorkflowResponse>>;

public record SearchApprovalWorkflowResponse(
    IReadOnlyList<ApprovalWorkflowListItem> Workflows,
    int TotalRecords);

internal sealed class SearchApprovalWorkflowQueryHandler(IApprovalWorkflowRepository approvalWorkflowRepository) : IRequestHandler<SearchApprovalWorkflowQuery, Result<SearchApprovalWorkflowResponse>>
{

    public async Task<Result<SearchApprovalWorkflowResponse>> Handle(SearchApprovalWorkflowQuery request, CancellationToken cancellationToken)
    {
        var rows = await approvalWorkflowRepository.SearchAsync(
            request.filterParam.Category,
            request.filterParam.OrganizationUnitTypeId,
            salaryGradeId: null);

        var groups = rows
            .GroupBy(x => new
            {
                x.Category,
                x.OrganizationUnitTypeId,
                x.LeaveTypeId,
                x.MinimumDays,
                x.MaximumDays,
            })
            .Select(g => BuildListItem(g.ToList()))
            .ToList();

        if (request.filterParam.SalaryGradeId.HasValue && request.filterParam.SalaryGradeId != Guid.Empty)
        {
            groups = groups
                .Where(item => item.SalaryGradesName != null
                    && rows.Any(r => r.SalaryGradeId == request.filterParam.SalaryGradeId
                        && r.Category == item.Category))
                .ToList();
        }

        IEnumerable<ApprovalWorkflowListItem> sorted = request.filterParam.SortOn?.ToLower() switch
        {
            "category" => request.filterParam.SortDirection?.ToLower() == "asc"
                ? groups.OrderBy(g => g.Category)
                : groups.OrderByDescending(g => g.Category),

            "minimumdays" => request.filterParam.SortDirection?.ToLower() == "asc"
                ? groups.OrderBy(g => g.MinimumDays)
                : groups.OrderByDescending(g => g.MinimumDays),

            _ => groups.OrderBy(g => g.Category).ThenBy(g => g.MinimumDays)
        };

        var paged = request.filterParam.PageSize > 0
            ? sorted.Skip(request.filterParam.PageIndex * request.filterParam.PageSize).Take(request.filterParam.PageSize).ToList()
            : sorted.ToList();

        var response = new SearchApprovalWorkflowResponse(paged, groups.Count);

        return Result.Ok(response);
    }

    private static ApprovalWorkflowListItem BuildListItem(List<ApprovalWorkflow> members)
    {
        var first = members[0];

        var salaryGradesName = string.Join(", ", members.Where(m => m.SalaryGradeId.HasValue).Select(m => m.SalaryGradeName)
                .Where(name => !string.IsNullOrWhiteSpace(name)).Distinct());

        var approvalPath = string.Join(" → ", (first.Steps ?? new List<ApprovalWorkflowStep>()).OrderBy(s => s.SequenceNo).Select(s => s.ReceiverPostName).Where(name => !string.IsNullOrWhiteSpace(name)));

        return new ApprovalWorkflowListItem
        {
            Id = first.Id,
            Category = first.Category,
            OrganizationUnitTypeName = first.OrganizationUnitTypeName,
            LeaveTypeName = first.LeaveTypeName,
            PostName = first.PostName,
            SalaryGradesName = string.IsNullOrEmpty(salaryGradesName) ? null : salaryGradesName,
            MinimumDays = first.MinimumDays,
            MaximumDays = first.MaximumDays,
            ApprovalPath = approvalPath,
        };
    }
}
