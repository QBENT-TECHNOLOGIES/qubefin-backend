using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblMemberAddress
{
    public Guid Id { get; set; }

    public Guid MemberId { get; set; }

    public string? HouseNo { get; set; }

    public string? StreetName { get; set; }

    public string? City { get; set; }

    public Guid AdministrativeUnitId { get; set; }

    public Guid PoliceStationId { get; set; }

    public string? PostOffice { get; set; }

    public string? PinCode { get; set; }

    public string? NearestLandmark { get; set; }

    public int? ResidenceYear { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public bool IsDefault { get; set; }

    public int Sequence { get; set; }

    public virtual TblAdministrativeUnit AdministrativeUnit { get; set; } = null!;

    public virtual TblMember Member { get; set; } = null!;

    public virtual TblPoliceStation PoliceStation { get; set; } = null!;

    public virtual ICollection<TblLoanApplication> TblLoanApplicationBorrowerAddresses { get; set; } = new List<TblLoanApplication>();

    public virtual ICollection<TblLoanApplication> TblLoanApplicationCoBorrowerAddresses { get; set; } = new List<TblLoanApplication>();
}
