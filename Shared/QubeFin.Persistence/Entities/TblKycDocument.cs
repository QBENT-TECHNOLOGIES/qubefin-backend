using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblKycDocument
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public bool IsMandatory { get; set; }

    public bool IsIdentityProof { get; set; }

    public bool IsAddressProof { get; set; }

    public bool IsDateValidate { get; set; }

    public int Sequence { get; set; }

    public virtual ICollection<TblMemberDocument> TblMemberDocuments { get; set; } = new List<TblMemberDocument>();
}
