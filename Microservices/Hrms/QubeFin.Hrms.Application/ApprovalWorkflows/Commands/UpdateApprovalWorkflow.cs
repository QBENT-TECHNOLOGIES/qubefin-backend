using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.ApprovalWorkflows.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.ApprovalWorkflows.Commands;

public record UpdateApprovalWorkflowCommand(
    Guid Id,
    ApprovalWorkflowRequest Workflow,
    Guid ModifiedBy) : IRequest<Result<string>>;


public class UpdateApprovalWorkflowCommandValidator
    : AbstractValidator<UpdateApprovalWorkflowCommand>
{
    public UpdateApprovalWorkflowCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.ModifiedBy)
            .NotEmpty();

        RuleFor(x => x.Workflow)
            .SetValidator(new ApprovalWorkflowRequestValidator());
    }
}


internal sealed class UpdateApprovalWorkflowCommandHandler(
    IApprovalWorkflowRepository approvalWorkflowRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateApprovalWorkflowCommand, Result<string>>
{
    public async Task<Result<string>> Handle(
        UpdateApprovalWorkflowCommand request,
        CancellationToken cancellationToken)
    {
        // ---------------------------------------------------------
        // 1. Get the workflow being edited
        // ---------------------------------------------------------
        var anchor = await approvalWorkflowRepository.GetByIdAsync(request.Id);

        if (anchor is null)
        {
            return new RecordNotFoundError(
                "Approval workflow not found.");
        }


        // ---------------------------------------------------------
        // 2. Get requested salary grades
        // ---------------------------------------------------------
        var requestedGradeIds =
            (request.Workflow.SalaryGradeIds ?? [])
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToList();


        // =========================================================
        // 3. Categories WITHOUT salary grade
        //    Example: ONDUTY / ATTENDANCE
        // =========================================================
        if (requestedGradeIds.Count == 0 &&
            anchor.SalaryGradeId is null)
        {
            var workflowForUpdate = CreateUpdateModel(
                anchor,
                request.Workflow,
                salaryGradeId: null,
                request.ModifiedBy);

            await approvalWorkflowRepository.UpdateAsync(
                workflowForUpdate);

            await unitOfWork.SaveChangesAsync(
                cancellationToken);

            return Result.Ok(
                "Approval workflow updated successfully.");
        }


        // =========================================================
        // 4. Salary grade is required for this workflow
        // =========================================================
        if (requestedGradeIds.Count == 0)
        {
            return Result.Fail(
                "At least one Salary Grade is required.");
        }


        // =========================================================
        // 5. Get all sibling workflows
        //
        // The workflow is stored as one row per salary grade. Only
        // the rows walking the SAME approval path belong to the
        // workflow being edited.
        // =========================================================
        var siblings =
            await approvalWorkflowRepository.GetSiblingsAsync(
                anchor.Category,
                anchor.OrganizationUnitTypeId,
                anchor.LeaveTypeId,
                anchor.MinimumDays,
                anchor.MaximumDays);

        var anchorPathKey = ApprovalWorkflowPath.GetKey(anchor);

        siblings = siblings
            .Where(x => ApprovalWorkflowPath.GetKey(x) == anchorPathKey)
            .ToList();


        // Make sure anchor is included
        if (siblings.All(x => x.Id != anchor.Id))
        {
            siblings = siblings
                .Append(anchor)
                .ToList();
        }


        // =========================================================
        // 6. Check salary grade conflicts
        //
        // Every sibling is excluded, not just the anchor: the other
        // rows of this same workflow are the ones already holding
        // the selected salary grades.
        // =========================================================
        var siblingIds = siblings
            .Select(x => x.Id)
            .ToList();

        var hasConflict =
            await approvalWorkflowRepository.HasConflictingWorkflowAsync(
                siblingIds,
                request.Workflow.Category.Trim(),
                request.Workflow.OrganizationUnitTypeId,
                request.Workflow.LeaveTypeId,
                request.Workflow.MinimumDays,
                request.Workflow.MaximumDays,
                requestedGradeIds);

        if (hasConflict)
        {
            return Result.Fail(
                "Cannot update approval workflow because one or more " +
                "selected Salary Grade(s) already have an approval " +
                "workflow for the selected Leave Type.");
        }


        // =========================================================
        // 7. Existing workflows grouped by Salary Grade
        // =========================================================
        var existingByGrade =
            siblings
                .Where(x => x.SalaryGradeId.HasValue)
                .GroupBy(x => x.SalaryGradeId!.Value)
                .ToDictionary(x => x.Key, x => x.First());


        // =========================================================
        // 8. Determine:
        //    - workflows to keep/update
        //    - workflows to remove
        //    - workflows to create
        // =========================================================
        var toKeep =
            requestedGradeIds
                .Where(existingByGrade.ContainsKey)
                .ToList();


        var toAdd =
            requestedGradeIds
                .Except(existingByGrade.Keys)
                .ToList();


        // Rows whose salary grade was removed, plus any row of this
        // group carrying no salary grade at all (the category was
        // switched to a grade based one).
        var keptWorkflowIds =
            toKeep
                .Select(x => existingByGrade[x].Id)
                .ToHashSet();

        var toRemove =
            siblings
                .Where(x => !keptWorkflowIds.Contains(x.Id))
                .ToList();


        // =========================================================
        // 9. Update existing workflows
        // =========================================================
        foreach (var gradeId in toKeep)
        {
            var existingWorkflow =
                existingByGrade[gradeId];

            var workflowForUpdate =
                CreateUpdateModel(
                    existingWorkflow,
                    request.Workflow,
                    gradeId,
                    request.ModifiedBy);

            await approvalWorkflowRepository.UpdateAsync(
                workflowForUpdate);
        }


        // =========================================================
        // 10. Remove workflows whose salary grade was removed
        //
        // Steps already referenced by an approval request event are
        // soft deleted by the repository instead of being dropped.
        // =========================================================
        foreach (var workflow in toRemove)
        {
            await approvalWorkflowRepository.DeleteAsync(
                workflow.Id);
        }


        // =========================================================
        // 11. Create workflows for newly selected salary grades
        // =========================================================
        foreach (var gradeId in toAdd)
        {
            var newWorkflowId = Guid.NewGuid();

            var newWorkflow =
                ApprovalWorkflow.Create(
                    newWorkflowId,
                    request.Workflow.Category.Trim(),
                    request.Workflow.LeaveTypeId,
                    request.Workflow.OrganizationUnitTypeId,
                    gradeId,
                    request.Workflow.PostId,
                    request.Workflow.MinimumDays,
                    request.Workflow.MaximumDays,
                    request.ModifiedBy,
                    BuildNewSteps(
                        request.Workflow.Steps,
                        newWorkflowId));

            await approvalWorkflowRepository.AddAsync(
                newWorkflow);
        }


        // =========================================================
        // 12. Save everything
        // =========================================================
        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        return Result.Ok(
            "Approval workflow updated successfully.");
    }


    // =============================================================
    // Create model used for updating an EXISTING workflow
    //
    // Update() is called on purpose: Create() alone leaves
    // LastModifiedOn / LastModifiedBy empty.
    // =============================================================
    private static ApprovalWorkflow CreateUpdateModel(
        ApprovalWorkflow existingWorkflow,
        ApprovalWorkflowRequest request,
        Guid? salaryGradeId,
        Guid modifiedBy)
    {
        var workflow = ApprovalWorkflow.Create(
            existingWorkflow.Id,
            request.Category.Trim(),
            request.LeaveTypeId,
            request.OrganizationUnitTypeId,
            salaryGradeId,
            request.PostId,
            request.MinimumDays,
            request.MaximumDays,
            modifiedBy,
            BuildExistingSteps(
                request.Steps,
                existingWorkflow.Id));

        workflow.Update(
            request.Category.Trim(),
            request.LeaveTypeId,
            request.OrganizationUnitTypeId,
            salaryGradeId,
            request.PostId,
            request.MinimumDays,
            request.MaximumDays,
            modifiedBy);

        return workflow;
    }


    // =============================================================
    // Existing workflow steps
    //
    // IMPORTANT:
    // The step ID from the request is preserved so the repository
    // can find the matching database row of the workflow being
    // edited. The sibling workflows keep their own step rows, and
    // for those the repository falls back to SequenceNo.
    // =============================================================
    private static IEnumerable<ApprovalWorkflowStep> BuildExistingSteps(
        IReadOnlyList<ApprovalWorkflowStepRequest> steps,
        Guid workflowId)
    {
        return steps.Select(step =>
            ApprovalWorkflowStep.Create(
                step.Id.GetValueOrDefault(),

                workflowId,

                step.ReceiverPostId,
                step.OrganizationUnitTypeId,
                step.IsRecommendEvent,
                step.IsApprovalEvent,
                step.EventStatus.Trim(),
                step.EventButtonText.Trim(),
                step.SequenceNo));
    }


    // =============================================================
    // New workflow steps
    //
    // IMPORTANT:
    // Always create NEW IDs.
    //
    // We must NOT reuse the IDs coming from the request because
    // those IDs belong to the workflow currently being edited.
    // =============================================================
    private static IEnumerable<ApprovalWorkflowStep> BuildNewSteps(
        IReadOnlyList<ApprovalWorkflowStepRequest> steps,
        Guid workflowId)
    {
        return steps.Select(step =>
            ApprovalWorkflowStep.Create(
                Guid.NewGuid(),
                workflowId,
                step.ReceiverPostId,
                step.OrganizationUnitTypeId,
                step.IsRecommendEvent,
                step.IsApprovalEvent,
                step.EventStatus.Trim(),
                step.EventButtonText.Trim(),
                step.SequenceNo));
    }
}
