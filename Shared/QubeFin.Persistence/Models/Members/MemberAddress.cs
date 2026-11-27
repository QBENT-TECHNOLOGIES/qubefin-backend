namespace QubeFin.Persistence.Models.Members;

public class MemberAddress
{
    public Guid Id { get; private set; }
    public Guid MemberId { get; private set; }
    public string? ParmanentHouseNo { get; private set; }
    public string? ParmanentStreetName { get; private set; }
    public string? ParmanentCity { get; private set; }
    public Guid ParmanentAdministrativeUnitId { get; private set; }
    public Guid ParmanentPoliceStationId { get; private set; }
    public string? ParmanentPostOffice { get; private set; }
    public string? ParmanentPinCode { get; private set; }
    public string? ParmanentNearestLandmark { get; private set; }
    public int? ParmanentResidenceYear { get; private set; }
    public bool PresentPermanentAddressSame { get; private set; }
    public string? PresentHouseNo { get; private set; }
    public string? PresentStreetName { get; private set; }
    public string? PresentCity { get; private set; }
    public Guid PresentAdministrativeUnitId { get; private set; }
    public Guid PresentPoliceStationId { get; private set; }
    public string? PresentPostOffice { get; private set; }
    public string? PresentPinCode { get; private set; }
    public string? PresentNearestLandmark { get; private set; }
    public int? PresentResidenceYear { get; private set; }
    public bool IsDefault { get; private set; }
    public int Sequence { get; private set; }

    public MemberAddress(
        Guid id,
        Guid memberId,
        string? parmanentHouseNo,
        string? parmanentStreetName,
        string? parmanentCity,
        Guid parmanentAdministrativeUnitId,
        Guid parmanentPoliceStationId,
        string? parmanentPostOffice,
        string? parmanentPinCode,
        string? parmanentNearestLandmark,
        int? parmanentResidenceYear,
        bool presentPermanentAddressSame,
        string? presentHouseNo,
        string? presentStreetName,
        string? presentCity,
        Guid presentAdministrativeUnitId,
        Guid presentPoliceStationId,
        string? presentPostOffice,
        string? presentPinCode,
        string? presentNearestLandmark,
        int? presentResidenceYear,
        bool isDefault,
        int sequence)
    {
        Id = id;
        MemberId = memberId;
        ParmanentHouseNo = parmanentHouseNo;
        ParmanentStreetName = parmanentStreetName;
        ParmanentCity = parmanentCity;
        ParmanentAdministrativeUnitId = parmanentAdministrativeUnitId;
        ParmanentPoliceStationId = parmanentPoliceStationId;
        ParmanentPostOffice = parmanentPostOffice;
        ParmanentPinCode = parmanentPinCode;
        ParmanentNearestLandmark = parmanentNearestLandmark;
        ParmanentResidenceYear = parmanentResidenceYear;
        PresentPermanentAddressSame = presentPermanentAddressSame;
        PresentHouseNo = presentHouseNo;
        PresentStreetName = presentStreetName;
        PresentCity = presentCity;
        PresentAdministrativeUnitId = presentAdministrativeUnitId;
        PresentPoliceStationId = presentPoliceStationId;
        PresentPostOffice = presentPostOffice;
        PresentPinCode = presentPinCode;
        PresentNearestLandmark = presentNearestLandmark;
        PresentResidenceYear = presentResidenceYear;
        IsDefault = isDefault;
        Sequence = sequence;
    }
}