namespace QubeFin.Hrms.Application.Employees.Models;

public class AddressInfoRequest
{
    public AddressModel? PresentAddress { get;  set; }
    public AddressModel? PermanentAddress { get;  set; }
}
public class AddressModel
{
    public string? HouseNo { get; set; }
    public string? RoadName { get; set; }
    public string? LandMark { get; set; }
    public Guid? AdministrativeUnitId { get; set; }
    public Guid? PoliceStationId { get; set; }
    public Guid? PostOfficeId { get; set; }
    public string? PinCode { get; set; }
    public string? OwnerShipOfHouse { get; set; }
    public int? DurationOfStayInMonths { get; set; }
}
