namespace QubeFin.Persistence.Models.Collection;

public sealed class CreditOfficerGroup
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid DesignationId { get; set; }
    public string DesignationName { get; set; } = string.Empty;
    public Guid? CreditOfficerId { get; set; }
    public string? CreditOfficerName { get; set; }
    public Guid OrganizationUnitId { get; set; }
    public string OrganizationUnitName { get; set; } = string.Empty;
    public int OrganizationCode { get; set; }
    public DateOnly? FormationDate { get; set; }
    public string? CollectionDay { get; set; }
    public TimeOnly? CollectionTime { get; set; }
    public bool IsMonthlyGroup { get; set; }
    public DateOnly? BiWeeklyStartDate { get; set; }
    public string? ContactNo { get; set; }
    public string? AddressLine1 { get; set; }
    public string? PinCode { get; set; }
}
