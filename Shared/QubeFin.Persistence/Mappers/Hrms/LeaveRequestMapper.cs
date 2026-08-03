using QubeFin.Persistence.Models.Hrms;
using Entity = QubeFin.Persistence.Entities.TblLeaveRequest;

namespace QubeFin.Persistence.Mappers.Hrms;

public static class LeaveRequestMapper
{
    public static LeaveRequest ToDomain(this Entity entity)
    {
        return new LeaveRequest(
            entity.Id,
            entity.EmployeeId,
            entity.LeaveTypeId,
            entity.FromDate,
            entity.ToDate,
            entity.RequestDate,
            entity.Reason,
            entity.Address,
            entity.TotalDays,
            entity.EnclosedDocName,
            entity.EnclosedDocNo,
            entity.LeavePrayerId,
            entity.LeaveYear,
            entity.CurrentStatus,
            entity.IsSubmitted,
            entity.SubmittedOn,
            entity.SubmittedBy
            );
    }

    public static Entity ToEntity(this LeaveRequest domain)
    {
        return new Entity
        {
            Id = domain.Id,
            EmployeeId = domain.EmployeeId,
            LeaveTypeId = domain.LeaveTypeId,
            FromDate = domain.FromDate,
            ToDate = domain.ToDate,
            RequestDate = domain.RequestDate,
            Reason = domain.Reason,
            Address = domain.Address,
            TotalDays = domain.TotalDays,
            EnclosedDocName = domain.EnclosedDocName,
            EnclosedDocNo = domain.EnclosedDocNo,
            LeavePrayerId = domain.LeavePrayerId,
            LeaveYear = domain.LeaveYear,
            CurrentStatus = domain.CurrentStatus,
            IsSubmitted = domain.IsSubmitted,
            SubmittedOn = domain.SubmittedOn,
            SubmittedBy = domain.SubmittedBy
        };
    }
}
