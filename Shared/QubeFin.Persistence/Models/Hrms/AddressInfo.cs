namespace QubeFin.Persistence.Models.Hrms;

public class AddressInfo
{
    public string? HouseNo { get; private set; }
    public string? RoadName { get; private set; }
    public string? LandMark { get; private set; }
    public Guid? AdministrativeUnitId { get; private set; }
    public Guid? PoliceStationId { get; private set; }
    public Guid? PostOfficeId { get; private set; }
    public string? PinCode { get; private set; }
    public string? OwnerShipOfHouse { get; private set; }
    public int? DurationOfStayInMonths { get; private set; }
}
