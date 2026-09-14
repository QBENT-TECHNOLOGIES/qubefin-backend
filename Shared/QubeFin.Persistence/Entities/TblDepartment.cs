using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblDepartment
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public Guid? HodEmployeeId { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual TblEmployee? HodEmployee { get; set; }

    public virtual ICollection<TblEmployee> TblEmployees { get; set; } = new List<TblEmployee>();

    public virtual ICollection<TblInterviewCandidate> TblInterviewCandidates { get; set; } = new List<TblInterviewCandidate>();
}
