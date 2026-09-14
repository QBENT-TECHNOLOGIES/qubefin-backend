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


        // ---------------------------------------------------------
        // 3. Check salary grade conflicts
        // ---------------------------------------------------------
        if (requestedGradeIds.Count > 0)
        {
            var hasConflict =
                await approvalWorkflowRepository.HasConflictingWorkflowAsync(
                    anchor.Id,
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
        }


        // =========================================================
        // 4. Categories WITHOUT salary grade
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
        // 5. Salary grade is required for this workflow
        // =========================================================
        if (requestedGradeIds.Count == 0)
        {
            return Result.Fail(
                "At least one Salary Grade is required.");
        }


        // =========================================================
        // 6. Get all sibling workflows
        // =========================================================
        var siblings =
            await approvalWorkflowRepository.GetSiblingsAsync(
                anchor.Category,
                anchor.OrganizationUnitTypeId,
                anchor.LeaveTypeId,
                anchor.MinimumDays,
                anchor.MaximumDays);


        // Make sure anchor is included
        if (siblings.All(x => x.Id != anchor.Id))
        {
            siblings = siblings
                .Append(anchor)
                .ToList();
        }


        // =========================================================
        // 7. Existing workflows grouped by Salary Grade
        // =========================================================
        var existingByGrade =
            siblings
                .Where(x => x.SalaryGradeId.HasValue)
                .ToDictionary(x => x.SalaryGradeId!.Value);


        // =========================================================
        // 8. Determine:
        //    - workflows to keep/update
        //    - workflows to remove
        //    - workflows to create
        // =========================================================
        var toRemove =
            existingByGrade
                .Where(x => !requestedGradeIds.Contains(x.Key))
                .Select(x => x.Value)
                .ToList();


        var toKeep =
            requestedGradeIds
                .Where(existingByGrade.ContainsKey)
                .ToList();


        var toAdd =
            requestedGradeIds
                .Except(existingByGrade.Keys)
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
    // Update workflow fields
    // =============================================================
    private static void ApplyFields(
        ApprovalWorkflow workflow,
        ApprovalWorkflowRequest request,
        Guid? salaryGradeId,
        Guid modifiedBy)
    {
        workflow.Update(
            request.Category.Trim(),
            request.LeaveTypeId,
            request.OrganizationUnitTypeId,
            salaryGradeId,
            request.PostId,
            request.MinimumDays,
            request.MaximumDays,
            modifiedBy);
    }


    // =============================================================
    // Create model used for updating an EXISTING workflow
    //
    // Existing step IDs from request are preserved here.
    // Repository uses these IDs to find existing DB steps.
    // =============================================================
    private static ApprovalWorkflow CreateUpdateModel(
        ApprovalWorkflow existingWorkflow,
        ApprovalWorkflowRequest request,
        Guid? salaryGradeId,
        Guid modifiedBy)
    {
        return ApprovalWorkflow.Create(
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
    }


    // =============================================================
    // Existing workflow steps
    //
    // IMPORTANT:
    // Existing step ID is preserved.
    // This allows repository UpdateAsync() to find and update
    // the existing database row.
    // =============================================================
    private static IEnumerable<ApprovalWorkflowStep> BuildExistingSteps(
        IReadOnlyList<ApprovalWorkflowStepRequest> steps,
        Guid workflowId)
    {
        return steps.Select(step =>
            ApprovalWorkflowStep.Create(
                step.Id.GetValueOrDefault() == Guid.Empty
                    ? Guid.NewGuid()
                    : step.Id.Value,

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