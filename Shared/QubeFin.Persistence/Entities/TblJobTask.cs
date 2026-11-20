using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace QubeFin.Persistence.Entities;

public partial class TblJobTask
{
    public Guid Id { get; set; }

    public string TaskNo { get; set; } = null!;

    public Guid JobId { get; set; }

    public string TaskType { get; set; } = null!;

    public Guid BranchId { get; set; }

    public Guid? OriginRefId { get; set; }

    public string? TaskTitle { get; set; }

    public string? Notes { get; set; }

    public string? Priority { get; set; }

    public Guid? CoOrdinatedBy { get; set; }

    public string? TargetName { get; set; }

    public Geometry? TargetLocation { get; set; }

    public int? RadiusMeters { get; set; }

    public string? Status { get; set; }

    public bool? IsSelfAdded { get; set; }

    public Guid? TaskAddedBy { get; set; }

    public DateTime? TaskAddedOn { get; set; }

    public Geometry? ArriveLocation { get; set; }

    public DateTime? ArrivedOn { get; set; }

    public decimal? ArriveDistanceMeters { get; set; }

    public bool IsCompleted { get; set; }

    public Geometry? CompletedLocation { get; set; }

    public Guid? TaskCompletedBy { get; set; }

    public DateTime? TaskCompletedOn { get; set; }

    public decimal? CompletedDistanceMeter { get; set; }

    public bool IsLocationMatched { get; set; }

    public string? PhotoUrl { get; set; }

    public bool IsOdMemberAvailable { get; set; }

    public bool IsEmireceived { get; set; }

    public decimal? ReceivedAmount { get; set; }

    public DateOnly? Odptpdate { get; set; }

    public string? Remarks { get; set; }

    public Guid? TransferTaskId { get; set; }

    public bool IsTaskCancel { get; set; }

    public Guid? TaskCancelBy { get; set; }

    public DateTime? TaskCancelDate { get; set; }

    public string? TaskCancelReason { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public virtual TblOrganizationUnit Branch { get; set; } = null!;

    public virtual TblEmployee? CoOrdinatedByNavigation { get; set; }

    public virtual TblJob Job { get; set; } = null!;

    public virtual TblUser? TaskAddedByNavigation { get; set; }

    public virtual TblUser? TaskCancelByNavigation { get; set; }

    public virtual TblUser? TaskCompletedByNavigation { get; set; }

    public virtual ICollection<TblJobHistory> Histories { get; set; } = new List<TblJobHistory>();
}
