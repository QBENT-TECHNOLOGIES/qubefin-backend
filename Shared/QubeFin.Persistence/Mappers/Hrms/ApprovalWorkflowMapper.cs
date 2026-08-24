using QubeFin.Persistence.Models.Hrms;
using Entity = QubeFin.Persistence.Entities.TblApprovalWorkflow;

namespace QubeFin.Persistence.Mappers.Hrms;

public static class ApprovalWorkflowMapper
{
    public static ApprovalWorkflow ToDomain(this Entity entity)
    {
        return new ApprovalWorkflow(
            entity.Id,
            entity.Category,
            entity.LeaveTypeId,
            entity.OrganizationUnitTypeId,
            entity.SalaryGradeId,
            entity.PostId,
            entity.MinimumDays,
            entity.MaximumDays,
            entity.CreatedOn,
            entity.CreatedBy,
            entity?.CreatedByNavigation?.UserName,
            entity.LastModifiedOn,
            entity.LastModifiedBy,
            entity?.LastModifiedByNavigation?.UserName,
            entity.TblApprovalWorkflowSteps.Select(x => x.ToDomain()).OrderBy(m => m.SequenceNo),
            entity.LeaveType?.Title,
            entity.SalaryGrade?.Name,
            entity.OrganizationUnitType?.Name,
            entity.Post?.Name);
    }

    public static Entity ToEntity(this ApprovalWorkflow domain)
    {
        return new Entity
        {
            Id = domain.Id,
            Category = domain.Category,
            LeaveTypeId = domain.LeaveTypeId,
            OrganizationUnitTypeId = domain.OrganizationUnitTypeId,
            SalaryGradeId = domain.SalaryGradeId,
            PostId = domain.PostId,
            MinimumDays = domain.MinimumDays,
            MaximumDays = domain.MaximumDays,
            CreatedOn = domain.CreatedOn,
            CreatedBy = domain.CreatedBy,
            LastModifiedOn = domain.LastModifiedOn,
            LastModifiedBy = domain.LastModifiedBy,
            TblApprovalWorkflowSteps = domain.Steps.Select(x => x.ToEntity()).ToList()
        };
    }
}
