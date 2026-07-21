namespace QubeFin.Global.Api.Requests;

public class OrganizationUnitRequest
{
    public Guid OrganizationUnitTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Codeval { get; set; }
    public Guid ParentId { get; set; }
    public TimeOnly AttendanceInTime { get; set; }
    public TimeOnly AttendanceOutTime { get; set; }
}
