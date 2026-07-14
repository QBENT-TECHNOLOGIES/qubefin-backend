namespace QubeFin.Persistence.Models.Hrms;

public class EmployeeDesignation
{
    public Guid Id { get; private set; }
    public Guid DesignationId { get; private set; }
    public DateTime EffectiveFrom { get; private set; }
    public DateTime? EffectiveTo { get; private set; }

    public EmployeeDesignation()
    {
    }

    public void Update(Guid designationId, DateTime effectiveFrom, DateTime? effectiveTo)
    {
        if (effectiveTo.HasValue && effectiveTo < effectiveFrom)
        {
            throw new InvalidOperationException(
                "Effective To date cannot be earlier than Effective From date.");
        }

        DesignationId = designationId;
        EffectiveFrom = effectiveFrom;
        EffectiveTo = effectiveTo;
    }
}
