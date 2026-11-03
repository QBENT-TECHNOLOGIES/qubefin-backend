using System;
using System.Collections.Generic;

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

    public string? TargetLatitude { get; set; }

    public string? TargetLongtitude { get; set; }

    public int? RadiusMeters { get; set; }

    public string? Status { get; set; }

    public bool? IsSelfAdded { get; set; }

    public Guid? TaskAddedBy { get; set; }

    public DateTime? TaskAddedOn { get; set; }

    public DateTime? ArrivedOn { get; set; }

    public string? ArriveLatitude { get; set; }

    public string? ArriveLontitude { get; set; }

    public decimal? ArriveDistanceMeters { get; set; }

    public bool IsCompleted { get; set; }

    public Guid? TaskCompletedBy { get; set; }

    public DateTime? TaskCompletedOn { get; set; }

    public string? CompletedLatitude { get; set; }

    public string? CompletedLongitude { get; set; }

    public decimal? CompletedDistanceMeter { get; set; }

    public bool IsLoacationMatched { get; set; }

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

    public string? TaskCancelReson { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public virtual TblOrganizationUnit Branch { get; set; } = null!;

    public virtual TblDesignation? CoOrdinatedByNavigation { get; set; }

    public virtual TblJob Job { get; set; } = null!;

    public virtual TblUser? TaskAddedByNavigation { get; set; }

    public virtual TblUser? TaskCancelByNavigation { get; set; }

    public virtual TblUser? TaskCompletedByNavigation { get; set; }
}
