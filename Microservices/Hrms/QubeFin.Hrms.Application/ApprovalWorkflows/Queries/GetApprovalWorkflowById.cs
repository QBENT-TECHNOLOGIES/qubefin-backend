using FluentResults;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.ApprovalWorkflows.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.ApprovalWorkflows.Queries;

public record GetApprovalWorkflowByIdQuery(Guid Id)
    : IRequest<Result<ApprovalWorkflowDetail>>;

internal sealed class GetApprovalWorkflowByIdQueryHandler(
    IApprovalWorkflowRepository approvalWorkflowRepository)
    : IRequestHandler<
        GetApprovalWorkflowByIdQuery,
        Result<ApprovalWorkflowDetail>>
{
    public async Task<Result<ApprovalWorkflowDetail>> Handle(
        GetApprovalWorkflowByIdQuery request,
        CancellationToken cancellationToken)
    {
        // ============================================================
        // 1. GET WORKFLOW
        // ============================================================

        var workflow = await approvalWorkflowRepository.GetByIdAsync(
            request.Id);

        if (workflow is null)
        {
            return new RecordNotFoundError(
                "Approval workflow not found.");
        }


        // ============================================================
        // 2. GET APPROVAL STEPS
        // ============================================================

        var approvalSteps = (workflow.Steps ??
                             new List<ApprovalWorkflowStep>())
            .OrderBy(s => s.SequenceNo)
            .ToList();


        // ============================================================
        // 3. GET SALARY GRADES BELONGING TO THIS WORKFLOW
        // ============================================================

        List<Guid>? salaryGradeIds = null;
        string? salaryGradesName = null;

        if (workflow.SalaryGradeId.HasValue)
        {
            var siblings =
                await approvalWorkflowRepository.GetSiblingsAsync(
                    workflow.Category,
                    workflow.OrganizationUnitTypeId,
                    workflow.LeaveTypeId,
                    workflow.MinimumDays,
                    workflow.MaximumDays);


            // --------------------------------------------------------
            // Important:
            //
            // Siblings can contain different approval paths.
            //
            // Example:
            //
            // Branch Manager:
            //     V, VI, VII, VIII, IX
            //
            // Area Manager:
            //     X, XI
            //
            // So only keep siblings having the SAME approval path
            // as the selected workflow.
            // --------------------------------------------------------

            var workflowPathKey = GetApprovalPathKey(workflow);

            var gradedSiblings = siblings
                .Where(x =>
                    x.SalaryGradeId.HasValue &&
                    GetApprovalPathKey(x) == workflowPathKey)
                .ToList();


            // --------------------------------------------------------
            // Make sure the selected workflow itself is included.
            // --------------------------------------------------------

            if (gradedSiblings.All(x => x.Id != workflow.Id))
            {
                gradedSiblings.Add(workflow);
            }


            // --------------------------------------------------------
            // Salary Grade IDs
            // --------------------------------------------------------

            salaryGradeIds = gradedSiblings
                .Where(x => x.SalaryGradeId.HasValue)
                .Select(x => x.SalaryGradeId!.Value)
                .Distinct()
                .ToList();


            // --------------------------------------------------------
            // Salary Grade Names
            // --------------------------------------------------------

            salaryGradesName = string.Join(
                ", ",
                gradedSiblings
                    .Select(x => x.SalaryGradeName)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct());
        }


        // ============================================================
        // 4. MAP APPROVAL STEPS
        // ============================================================

        var approvalStepResponse = approvalSteps
            .Select(s => new ApprovalStep
            {
                Id = s.Id,
                ApprovalWorkflowId = s.ApprovalWorkflowId,
                OrganizationUnitTypeId = s.OrganizationUnitTypeId,
                ReceiverPostId = s.ReceiverPostId,
                IsRecommendEvent = s.IsRecommendEvent,
                IsApprovalEvent = s.IsApprovalEvent,
                EventStatus = s.EventStatus,
                EventButtonText = s.EventButtonText,
                SequenceNo = s.SequenceNo,
                OrganizationUnitTypeName = s.OrganizationUnitTypeName
            })
            .ToList();


        // ============================================================
        // 5. FIRST APPROVAL STEP
        // ============================================================

        var firstStep = approvalSteps.FirstOrDefault();


        // ============================================================
        // 6. BUILD DETAIL RESPONSE
        // ============================================================

        var detail = new ApprovalWorkflowDetail
        {
            Id = workflow.Id,

            Category = workflow.Category,

            OrganizationUnitTypeId =
                workflow.OrganizationUnitTypeId,

            LeaveTypeId =
                workflow.LeaveTypeId,

            PostId =
                workflow.PostId,

            SalaryGradeIds =
                salaryGradeIds,

            MinimumDays =
                workflow.MinimumDays,

            MaximumDays =
                workflow.MaximumDays,

            LeaveTypeName =
                workflow.LeaveTypeName,

            SalaryGradesName =
                string.IsNullOrWhiteSpace(salaryGradesName)
                    ? null
                    : salaryGradesName,

            OrganizationUnitTypeName =
                workflow.OrganizationUnitTypeName,

            PostName =
                workflow.PostName,

            CreatedByName =
                workflow.CreatedByName,

            LastModifiedByName =
                workflow.LastModifiedByName,

            CreatedOn =
                workflow.CreatedOn,

            LastModifiedOn =
                workflow.LastModifiedOn,

            ApprovalSteps =
                approvalStepResponse,

            StepPost =
                firstStep?.ReceiverPostName
        };


        return Result.Ok(detail);
    }


    // ================================================================
    // Creates a unique key for the approval workflow/path.
    //
    // Salary Grade is deliberately NOT included.
    //
    // Example:
    //
    // Branch Manager:
    // D6FB...:D1AE...:1
    //
    // Area Manager:
    // D6FB...:A514...:1
    //
    // Therefore they are treated as different workflows.
    // ================================================================

    private static string GetApprovalPathKey(
        ApprovalWorkflow workflow)
    {
        return string.Join(
            "|",
            (workflow.Steps ??
             new List<ApprovalWorkflowStep>())
                .OrderBy(s => s.SequenceNo)
                .Select(s =>
                    $"{s.OrganizationUnitTypeId}:" +
                    $"{s.ReceiverPostId}:" +
                    $"{s.SequenceNo}"));
    }
}