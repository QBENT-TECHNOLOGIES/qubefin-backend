using FluentResults;
using MediatR;
using QubeFin.Hrms.Application.ApprovalWorkflows.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.ApprovalWorkflows.Queries;

public record SearchApprovalWorkflowQuery(
    ApprovalWorkflowSearchRequest filterParam)
    : IRequest<Result<SearchApprovalWorkflowResponse>>;

public record SearchApprovalWorkflowResponse(
    IReadOnlyList<ApprovalWorkflowListItem> Workflows,
    int TotalRecords);

internal sealed class SearchApprovalWorkflowQueryHandler(
    IApprovalWorkflowRepository approvalWorkflowRepository)
    : IRequestHandler<
        SearchApprovalWorkflowQuery,
        Result<SearchApprovalWorkflowResponse>>
{
    public async Task<Result<SearchApprovalWorkflowResponse>> Handle(
    SearchApprovalWorkflowQuery request,
    CancellationToken cancellationToken)
    {
        // ============================================================
        // 1. GET ALL DATA
        // ============================================================

        var rows = await approvalWorkflowRepository.SearchAsync(
            request.filterParam.Category,
            request.filterParam.OrganizationUnitTypeId,
            salaryGradeId: null);


        // ============================================================
        // 2. GROUP BY PARENT + APPROVAL WORKFLOW
        // ============================================================

        var parentGroups = rows
            .GroupBy(x => new
            {
                x.Category,
                x.OrganizationUnitTypeId,
                x.LeaveTypeId,
                x.MinimumDays,
                x.MaximumDays,
                x.PostId,

                // Child workflow identifies whether this is
                // BM Approval or AM Approval.
                ApprovalPath = string.Join(
                    "|",
                    (x.Steps ?? new List<ApprovalWorkflowStep>())
                        .OrderBy(s => s.SequenceNo)
                        .Select(s =>
                            $"{s.OrganizationUnitTypeId}:" +
                            $"{s.ReceiverPostId}:" +
                            $"{s.SequenceNo}"))
            })
            .Select(g => BuildListItem(g.ToList()))
            .ToList();


        // ============================================================
        // 3. SALARY GRADE FILTER
        // ============================================================

        if (request.filterParam.SalaryGradeId.HasValue &&
            request.filterParam.SalaryGradeId != Guid.Empty)
        {
            var salaryGradeId =
                request.filterParam.SalaryGradeId.Value;

            parentGroups = parentGroups
                .Where(item =>
                    rows.Any(r =>
                        r.SalaryGradeId == salaryGradeId &&
                        r.Category == item.Category &&
                        r.OrganizationUnitTypeId ==
                            item.OrganizationUnitTypeId &&
                        r.LeaveTypeId ==
                            item.LeaveTypeId &&
                        r.MinimumDays ==
                            item.MinimumDays &&
                        r.MaximumDays ==
                            item.MaximumDays &&
                        r.PostId ==
                            item.PostId &&
                        GetApprovalPathKey(r) ==
                            GetApprovalPathKey(
                                rows.First(x =>
                                    x.Id == item.Id))))
                .ToList();
        }


        // ============================================================
        // 4. SORT
        // ============================================================

        IEnumerable<ApprovalWorkflowListItem> sorted =
            request.filterParam.SortOn?.ToLower() switch
            {
                "category" =>
                    request.filterParam.SortDirection?.ToLower() == "asc"
                        ? parentGroups.OrderBy(x => x.Category)
                        : parentGroups.OrderByDescending(x => x.Category),

                "minimumdays" =>
                    request.filterParam.SortDirection?.ToLower() == "asc"
                        ? parentGroups.OrderBy(x => x.MinimumDays)
                        : parentGroups.OrderByDescending(x => x.MinimumDays),

                _ =>
                    parentGroups
                        .OrderBy(x => x.Category)
                        .OrderBy(x => x.OrganizationUnitTypeName)
                        .ThenBy(x => x.LeaveTypeName)
                        .ThenBy(x => x.MinimumDays)
            };


        // ============================================================
        // 5. PAGINATION
        // ============================================================

        var totalRecords = parentGroups.Count;

        var paged = request.filterParam.PageSize > 0
            ? sorted
                .Skip(request.filterParam.PageIndex *
                      request.filterParam.PageSize)
                .Take(request.filterParam.PageSize)
                .ToList()
            : sorted.ToList();


        // ============================================================
        // 6. RESPONSE
        // ============================================================

        return Result.Ok(
            new SearchApprovalWorkflowResponse(
                paged,
                totalRecords));
    }


    private static ApprovalWorkflowListItem BuildListItem(
    List<ApprovalWorkflow> members)
    {
        var first = members[0];

        // Merge all salary grades belonging to this
        // parent + approval path.
        var salaryGradesName = string.Join(
            ", ",
            members
                .Where(x => x.SalaryGradeId.HasValue)
                .Select(x => x.SalaryGradeName)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct());

        // Steps belong to the same workflow configuration,
        // so use one representative workflow.
        var childSteps = (first.Steps ??
                          new List<ApprovalWorkflowStep>())
            .GroupBy(s => new
            {
                s.OrganizationUnitTypeId,
                s.ReceiverPostId,
                s.SequenceNo
            })
            .Select(g => g.First())
            .OrderBy(s => s.SequenceNo)
            .ToList();

        var approvalPath = string.Join(
            " → ",
            childSteps
                .Select(s => s.ReceiverPostName)
                .Where(name => !string.IsNullOrWhiteSpace(name)));

        return new ApprovalWorkflowListItem
        {
            Id = first.Id,
            Category = first.Category,
            OrganizationUnitTypeName = first.OrganizationUnitTypeName,
            LeaveTypeName = first.LeaveTypeName,
            PostName = first.PostName,
            PostId = first.PostId,

            SalaryGradesName =
                string.IsNullOrEmpty(salaryGradesName)
                    ? null
                    : salaryGradesName,

            MinimumDays = first.MinimumDays,
            MaximumDays = first.MaximumDays,

            ApprovalPath = approvalPath
        };
    }

    private static string GetApprovalPathKey(
    ApprovalWorkflow workflow)
    {
        return string.Join(
            "|",
            (workflow.Steps ?? new List<ApprovalWorkflowStep>())
                .OrderBy(s => s.SequenceNo)
                .Select(s =>
                    $"{s.OrganizationUnitTypeId}:" +
                    $"{s.ReceiverPostId}:" +
                    $"{s.SequenceNo}"));
    }
}