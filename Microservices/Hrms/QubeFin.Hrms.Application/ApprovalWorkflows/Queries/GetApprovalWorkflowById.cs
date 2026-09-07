using FluentResults;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.ApprovalWorkflows.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.ApprovalWorkflows.Queries;

public record GetApprovalWorkflowByIdQuery(Guid Id) : IRequest<Result<ApprovalWorkflowDetail>>;

//public record GetApprovalWorkflowByIdResponse(ApprovalWorkflow Workflow);

internal sealed class GetApprovalWorkflowByIdQueryHandler(IApprovalWorkflowRepository approvalWorkflowRepository) : IRequestHandler<GetApprovalWorkflowByIdQuery, Result<ApprovalWorkflowDetail>>
{
    public async Task<Result<ApprovalWorkflowDetail>> Handle(GetApprovalWorkflowByIdQuery request, CancellationToken cancellationToken)
    {
        var workflow = await approvalWorkflowRepository.GetByIdAsync(request.Id);
        if (workflow is null)
        {
            return new RecordNotFoundError("Approval workflow not found.");
        }

        List<Guid>? salaryGradeIds = null;
        string? salaryGradesName = null;

        if (workflow.SalaryGradeId.HasValue)
        {
            var siblings = await approvalWorkflowRepository.GetSiblingsAsync(workflow.Category, workflow.OrganizationUnitTypeId, workflow.LeaveTypeId, workflow.MinimumDays, workflow.MaximumDays);

            var gradedSiblings = siblings.Where(w => w.SalaryGradeId.HasValue).ToList();

            // Guard: make sure the anchor itself is represented even if the
            // sibling query somehow missed it (AsNoTracking / timing edge cases).
            if (gradedSiblings.All(w => w.SalaryGradeId!.Value != workflow.SalaryGradeId.Value))
            {
                gradedSiblings.Add(workflow);
            }

            salaryGradeIds = gradedSiblings.Select(w => w.SalaryGradeId!.Value).Distinct().ToList();

            salaryGradesName = string.Join(", ", gradedSiblings.Select(w => w.SalaryGradeName).Where(name => !string.IsNullOrWhiteSpace(name)).Distinct());
        }

        var approvalSteps = (workflow.Steps ?? new List<ApprovalWorkflowStep>()).OrderBy(s => s.SequenceNo).Select(s => new ApprovalStep
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
            OrganizationUnitTypeName = s.OrganizationUnitTypeName,
        }).ToList();

        var detail = new ApprovalWorkflowDetail
        {
            Id = workflow.Id,
            Category = workflow.Category,
            OrganizationUnitTypeId = workflow.OrganizationUnitTypeId,
            LeaveTypeId = workflow.LeaveTypeId,
            PostId = workflow.PostId,
            SalaryGradeIds = salaryGradeIds,
            MinimumDays = workflow.MinimumDays,
            MaximumDays = workflow.MaximumDays,
            LeaveTypeName = workflow.LeaveTypeName,
            SalaryGradesName = salaryGradesName,
            OrganizationUnitTypeName = workflow.OrganizationUnitTypeName,
            PostName = workflow.PostName,
            CreatedByName = workflow.CreatedByName,
            LastModifiedByName = workflow.LastModifiedByName,
            CreatedOn = workflow.CreatedOn,
            LastModifiedOn = workflow.LastModifiedOn,
            ApprovalSteps = approvalSteps,
            StepPost = approvalSteps.FirstOrDefault()?.ReceiverPostId != null ? workflow.Steps?.FirstOrDefault()?.ReceiverPostName : null,
        };

        return Result.Ok(detail);
    }
}