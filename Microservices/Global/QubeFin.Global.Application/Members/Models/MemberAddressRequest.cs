namespace QubeFin.Global.Application.Members.Models;

public class MemberAddressRequest
{
    public Guid? Id { get; set; }
    public string? ParmanentHouseNo { get; set; }
    public string? ParmanentStreetName { get; set; }
    public string? ParmanentCity { get; set; }
    public Guid ParmanentAdministrativeUnitId { get; set; }
    public Guid ParmanentPoliceStationId { get; set; }
    public string? ParmanentPostOffice { get; set; }
    public string? ParmanentPinCode { get; set; }
    public string? ParmanentNearestLandmark { get; set; }
    public int? ParmanentResidenceYear { get; set; }
    public bool PresentPermanentAddressSame { get; set; }
    public string? PresentHouseNo { get; set; }
    public string? PresentStreetName { get; set; }
    public string? PresentCity { get; set; }
    public Guid PresentAdministrativeUnitId { get; set; }
    public Guid PresentPoliceStationId { get; set; }
    public string? PresentPostOffice { get; set; }
    public string? PresentPinCode { get; set; }
    public string? PresentNearestLandmark { get; set; }
    public int? PresentResidenceYear { get; set; }
    public bool IsDefault { get; set; }
}