using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblHoliday
{
    public Guid Id { get; set; }

    public Guid OrgUnitId { get; set; }

    public DateOnly HolidayDate { get; set; }

    public string Description { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public byte[] RowVersion { get; set; } = null!;
}
