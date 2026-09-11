namespace QubeFin.Global.Api.Requests;

public class OrganizationUnitRequest
{
    public Guid OrganizationUnitTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Codeval { get; set; }
    public Guid ParentId { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public TimeOnly? AttendanceInTime { get; set; }
    public TimeOnly? AttendanceOutTime { get; set; }
    public int? CheckRadiusInMeter { get; set; }
    public Guid? CompanyId { get; set; }
}
