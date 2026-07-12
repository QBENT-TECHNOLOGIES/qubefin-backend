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

    public AddressInfo()
    {
    }

    public AddressInfo(string? houseNo, string? roadName, string? landMark, Guid? administrativeUnitId, Guid? policeStationId, Guid? postOfficeId,
        string? pinCode, string? ownerShipOfHouse, int? durationOfStayInMonths)
    {
        HouseNo = houseNo;
        RoadName = roadName;
        LandMark = landMark;
        AdministrativeUnitId = administrativeUnitId;
        PoliceStationId = policeStationId;
        PostOfficeId = postOfficeId;
        PinCode = pinCode;
        OwnerShipOfHouse = ownerShipOfHouse;
        DurationOfStayInMonths = durationOfStayInMonths;
    }
}
