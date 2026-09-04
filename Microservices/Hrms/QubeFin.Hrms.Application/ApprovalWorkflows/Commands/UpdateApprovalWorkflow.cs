using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.ApprovalWorkflows.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.ApprovalWorkflows.Commands;

public record UpdateApprovalWorkflowCommand(Guid Id, ApprovalWorkflowRequest Workflow, Guid ModifiedBy) : IRequest<Result<string>>;

public class UpdateApprovalWorkflowCommandValidator : AbstractValidator<UpdateApprovalWorkflowCommand>
{
    public UpdateApprovalWorkflowCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ModifiedBy).NotEmpty();
        RuleFor(x => x.Workflow).SetValidator(new ApprovalWorkflowRequestValidator());
    }
}


internal sealed class UpdateApprovalWorkflowCommandHandler(IApprovalWorkflowRepository approvalWorkflowRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateApprovalWorkflowCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateApprovalWorkflowCommand request, CancellationToken cancellationToken)
    {
        var anchor = await approvalWorkflowRepository.GetByIdAsync(request.Id);
        if (anchor is null)
        {
            return new RecordNotFoundError("Approval workflow not found.");
        }

        var requestedGradeIds = (request.Workflow.SalaryGradeIds ?? new List<Guid>()).Distinct().ToList();

        // Categories without a grade concept (ONDUTY / ATTENDANCE) — single-row update.
        if (requestedGradeIds.Count == 0 && anchor.SalaryGradeId is null)
        {
            ApplyFields(anchor, request.Workflow, salaryGradeId: null, request.ModifiedBy);
            anchor.ReplaceSteps(BuildSteps(request.Workflow.Steps, anchor.Id));
            await approvalWorkflowRepository.UpdateAsync(anchor);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok("Approval workflow updated successfully.");
        }

        if (requestedGradeIds.Count == 0)
        {
            return Result.Fail("At least one Salary Grade is required.");
        }

        // Find siblings using the ANCHOR's current values — that defines who it's
        // currently grouped with, before we apply any incoming changes.
        var siblings = await approvalWorkflowRepository.GetSiblingsAsync(anchor.Category, anchor.OrganizationUnitTypeId, anchor.LeaveTypeId, anchor.MinimumDays, anchor.MaximumDays);

        if (siblings.All(w => w.Id != anchor.Id))
        {
            siblings = siblings.Append(anchor).ToList();
        }

        var existingByGrade = siblings.Where(w => w.SalaryGradeId.HasValue).ToDictionary(w => w.SalaryGradeId!.Value);

        var toRemove = existingByGrade.Where(kv => !requestedGradeIds.Contains(kv.Key)).Select(kv => kv.Value).ToList();
        var toKeep = requestedGradeIds.Where(g => existingByGrade.ContainsKey(g)).ToList();
        var toAdd = requestedGradeIds.Except(existingByGrade.Keys).ToList();

        foreach (var gradeId in toKeep)
        {
            var wf = existingByGrade[gradeId];
            ApplyFields(wf, request.Workflow, gradeId, request.ModifiedBy);
            wf.ReplaceSteps(BuildSteps(request.Workflow.Steps, wf.Id));
            await approvalWorkflowRepository.UpdateAsync(wf);
        }

        foreach (var wf in toRemove)
        {
            await approvalWorkflowRepository.DeleteAsync(wf.Id);
        }

        foreach (var gradeId in toAdd)
        {
            var newId = Guid.NewGuid();
            var newWorkflow = ApprovalWorkflow.Create(
                newId,
                request.Workflow.Category.Trim(),
                request.Workflow.LeaveTypeId,
                request.Workflow.OrganizationUnitTypeId,
                gradeId,
                request.Workflow.PostId,
                request.Workflow.MinimumDays,
                request.Workflow.MaximumDays,
                request.ModifiedBy,
                BuildSteps(request.Workflow.Steps, newId));
            await approvalWorkflowRepository.AddAsync(newWorkflow);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok("Approval workflow updated successfully.");
    }

    private static void ApplyFields(ApprovalWorkflow wf, ApprovalWorkflowRequest request, Guid? salaryGradeId, Guid modifiedBy) =>
        wf.Update(
            request.Category.Trim(),
            request.LeaveTypeId,
            request.OrganizationUnitTypeId,
            salaryGradeId,
            request.PostId,
            request.MinimumDays,
            request.MaximumDays,
            modifiedBy);

    private static IEnumerable<ApprovalWorkflowStep> BuildSteps(
        IReadOnlyList<ApprovalWorkflowStepRequest> steps, Guid workflowId) =>
        steps.Select(step => ApprovalWorkflowStep.Create(
            step.Id.GetValueOrDefault() == Guid.Empty ? Guid.NewGuid() : step.Id!.Value,
            workflowId,
            step.ReceiverPostId,
            step.OrganizationUnitTypeId,
            step.IsRecommendEvent,
            step.IsApprovalEvent,
            step.EventStatus.Trim(),
            step.EventButtonText.Trim(),
            step.SequenceNo));
}
