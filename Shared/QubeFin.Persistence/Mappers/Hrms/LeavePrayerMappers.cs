using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Persistence.Mappers.Hrms
{
    public static class LeavePrayerMappers
    {
        public static LeavePrayer ToDomain(this TblLeavePrayer entity)
        {
            if (entity == null) return null!;
            return new LeavePrayer(
                entity.Id,
                entity.EmployeeId,
                entity.LeaveTypeId,
                entity.PrayerDays,
                entity.Attachment,
                entity.Remarks,
                entity.CreatedBy,
                entity.CreatedOn,
                entity.CurrentStatus
            );
        }
        public static TblLeavePrayer ToEntity(this LeavePrayer domain)
        {
            if (domain == null) return null!;
            return new TblLeavePrayer
            {
                Id = domain.Id,
                EmployeeId = domain.EmployeeId,
                LeaveTypeId = domain.LeaveTypeId,
                PrayerDays = domain.PrayerDays,
                Attachment = domain.Attachment,
                Remarks = domain.Remarks,
                CreatedBy = domain.CreatedBy,
                CreatedOn = domain.CreatedOn,
                CurrentStatus = domain.CurrentStatus
            };
        }
    }
}
