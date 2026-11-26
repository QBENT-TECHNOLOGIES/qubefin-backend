using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace QubeFin.Persistence.Entities;

public partial class TblMemberAddress
{
    public Guid Id { get; set; }

    public Guid MemberId { get; set; }

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

    public Geometry? Location { get; set; }

    public bool IsDefault { get; set; }

    public int Sequence { get; set; }

    public virtual TblMember Member { get; set; } = null!;

    public virtual TblAdministrativeUnit ParmanentAdministrativeUnit { get; set; } = null!;

    public virtual TblPoliceStation ParmanentPoliceStation { get; set; } = null!;

    public virtual TblAdministrativeUnit PresentAdministrativeUnit { get; set; } = null!;

    public virtual TblPoliceStation PresentPoliceStation { get; set; } = null!;

    public virtual ICollection<TblLoanApplication> TblLoanApplicationBorrowerAddresses { get; set; } = new List<TblLoanApplication>();

    public virtual ICollection<TblLoanApplication> TblLoanApplicationCoBorrowerAddresses { get; set; } = new List<TblLoanApplication>();
}
