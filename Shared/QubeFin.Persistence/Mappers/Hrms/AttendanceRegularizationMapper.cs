using QubeFin.Persistence.Models.Hrms;
using Entity = QubeFin.Persistence.Entities.TblAttendanceRegularization;

namespace QubeFin.Persistence.Mappers.App;

public static class AttendanceRegularizationMapper
{
    //public static AttendanceRegularization ToDomain(this Entity entity)
    //{
    //    return new AttendanceRegularization(
    //        entity.Id,
    //        entity.EmployeeId,
    //        entity.RegularizationDate,
    //        entity.Reason,
    //        entity.AppliedOn,
    //        entity.IsSubmit,
    //        entity.SubmitOn,
    //        entity.IsApproved,
    //        entity.IsRejected,
    //        entity.ActivityBy,
    //        entity.ActivityOn,
    //        entity.Attachment
    //    );
    //}

    //public static Entity ToEntity(this AttendanceRegularization domain)
    //{
    //    return new Entity
    //    {
    //        Id = domain.Id,
    //        EmployeeId = domain.EmployeeId,
    //        RegularizationDate = domain.RegularizationDate,
    //        Reason = domain.Reason,
    //        AppliedOn = domain.AppliedOn,
    //        IsSubmit = domain.IsSubmit,
    //        SubmitOn = domain.SubmitOn,
    //        IsApproved = domain.IsApproved,
    //        IsRejected = domain.IsRejected,
    //        ActivityBy = domain.ActivityBy,
    //        ActivityOn = domain.ActivityOn,
    //        Attachment = domain.Attachment
    //    };
    //}
}
