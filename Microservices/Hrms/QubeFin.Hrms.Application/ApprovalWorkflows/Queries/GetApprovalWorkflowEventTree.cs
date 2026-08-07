using FluentResults;
using MediatR;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.ApprovalWorkflows.Queries;

public record GetApprovalWorkflowEventTreeQuery
    : IRequest<Result<List<ApprovalWorkflowEventGroupItem>>>;

public sealed class GetApprovalWorkflowEventTreeResponse
{
    public List<CategoryNode> Categories { get; init; } = [];
}

public sealed class CategoryNode
{
    public string Category { get; init; } = "";
    public List<OrganizationUnitNode> OrganizationUnits { get; init; } = [];
}

public sealed class OrganizationUnitNode
{
    public string OrganizationUnitType { get; init; } = "";

    // Used for ATTENDANCE / ONDUTY etc.
    public List<WorkflowNode> Workflows { get; init; } = [];

    // Used for LEAVE
    public List<LeaveTypeNode> LeaveTypes { get; init; } = [];
}

public sealed class LeaveTypeNode
{
    public string LeaveType { get; init; } = "";
    public List<SalaryGradeNode> SalaryGrades { get; init; } = [];
}

public sealed class SalaryGradeNode
{
    public string SalaryGrade { get; init; } = "";
    public List<WorkflowNode> Workflows { get; init; } = [];
}

public sealed class WorkflowNode
{
    public string? WorkflowName { get; init; } // Example: Regularization
    public string RangeDays { get; init; } = "-";
    public string WorkflowEventPath { get; init; } = "";
}

internal sealed class GetApprovalWorkflowEventTreeQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetApprovalWorkflowEventTreeQuery, Result<List<ApprovalWorkflowEventGroupItem>>>
{
    public async Task<Result<List<ApprovalWorkflowEventGroupItem>>> Handle(
        GetApprovalWorkflowEventTreeQuery request,
        CancellationToken cancellationToken)
    {
        var workflowEventGroupItems = await context.SP_GetApprovalWorkflowEventGroupItems();
        return Result.Ok(workflowEventGroupItems);
    }

    public static GetApprovalWorkflowEventTreeResponse BuildDisplayRows(
    IEnumerable<ApprovalWorkflowEventGroupItem> source)
    {
        return new GetApprovalWorkflowEventTreeResponse
        {
            Categories = source
                    .GroupBy(x => x.Category)
                    .OrderBy(x => x.Key)
                    .Select(categoryGroup => new CategoryNode
                    {
                        Category = categoryGroup.Key,

                        OrganizationUnits = categoryGroup
                            .GroupBy(x => x.OrganizationUnitType)
                            .OrderBy(x => x.Key)
                            .Select(orgUnitGroup =>
                            {
                                bool isLeave = categoryGroup.Key.Equals(
                                    "LEAVE",
                                    StringComparison.OrdinalIgnoreCase);

                                return new OrganizationUnitNode
                                {
                                    OrganizationUnitType = orgUnitGroup.Key,

                                    // ATTENDANCE / ONDUTY
                                    Workflows = !isLeave
                                        ? orgUnitGroup
                                            //.OrderBy(x => x.WorkflowName)
                                            .Select(x => new WorkflowNode
                                            {
                                                WorkflowName = "Regularisation",
                                                RangeDays = x.RangeDays,
                                                WorkflowEventPath = x.WorkflowEventPath
                                            })
                                            .ToList()
                                        : [],

                                    // LEAVE
                                    LeaveTypes = isLeave
                                        ? orgUnitGroup
                                            .GroupBy(x => x.LeaveType)
                                            .OrderBy(x => x.Key)
                                            .Select(leaveTypeGroup => new LeaveTypeNode
                                            {
                                                LeaveType = leaveTypeGroup.Key ?? "",

                                                SalaryGrades = leaveTypeGroup
                                                    .GroupBy(x => x.SalaryGrade)
                                                    .OrderBy(x => x.Key)
                                                    .Select(salaryGradeGroup =>
                                                        new SalaryGradeNode
                                                        {
                                                            SalaryGrade =
                                                                salaryGradeGroup.Key ?? "",

                                                            Workflows = salaryGradeGroup
                                                                .OrderBy(x => GetRangeStart(
                                                                    x.RangeDays))
                                                                .Select(x =>
                                                                    new WorkflowNode
                                                                    {
                                                                        RangeDays =
                                                                            x.RangeDays,
                                                                        WorkflowEventPath =
                                                                            x.WorkflowEventPath
                                                                    })
                                                                .ToList()
                                                        })
                                                    .ToList()
                                            })
                                            .ToList()
                                        : []
                                };
                            })
                            .ToList()
                    })
                    .ToList()
        };
    }

    private static int GetRangeStart(string rangeDays)
    {
        // "1 - 3" => 1, "7-" => 7
        var firstValue = rangeDays
            .Split('-', StringSplitOptions.TrimEntries)[0];

        return int.TryParse(firstValue, out int days) ? days : 0;
    }
}
