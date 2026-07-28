using QubeFin.Persistence.Models.Hrms;
using Entity = QubeFin.Persistence.Entities.TblLeaveType;

namespace QubeFin.Persistence.Mappers.Hrms;

public static class LeaveTypeMapper
{
    public static LeaveType ToDomain(this Entity entity)
    {
        return new LeaveType(
            entity.Id,
            entity.Title,
            entity.Alias,
            entity.IsPrayerable,
            entity.IsConvertible,
            entity.IsEncashable,
            entity.NoOfDaysEntitled,
            entity.NoOfDaysCapped,
            entity.MaxContinuousDays,
            entity.ApplicableAfterProbation,
            entity.IsMonthlyCredit,
            entity.SeqNo,
            entity.CreatedBy,
            entity.CreatedOn,
            entity.LastModifiedBy,
            entity.LastModifiedOn
            );
    }

    public static Entity ToEntity(this LeaveType domain)
    {
        return new Entity
        {
            Id = domain.Id,
            Title = domain.Title,
            Alias = domain.Alias,
            IsPrayerable = domain.IsPrayerable,
            IsConvertible = domain.IsConvertible,
            IsEncashable = domain.IsEncashable,
            NoOfDaysEntitled = domain.NoOfDaysEntitled,
            NoOfDaysCapped = domain.NoOfDaysCapped,
            MaxContinuousDays = domain.MaxContinuousDays,
            ApplicableAfterProbation = domain.ApplicableAfterProbation,
            IsMonthlyCredit = domain.IsMonthlyCredit,
            SeqNo = domain.SeqNo,
            CreatedBy = domain.CreatedBy,
            CreatedOn = domain.CreatedOn,
            LastModifiedBy = domain.LastModifiedBy,
            LastModifiedOn = domain.LastModifiedOn
        };
    }
}
