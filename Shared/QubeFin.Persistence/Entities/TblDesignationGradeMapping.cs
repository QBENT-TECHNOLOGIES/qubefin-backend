using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblDesignationGradeMapping
{
    public Guid Id { get; set; }

    public Guid DesignationId { get; set; }

    public Guid GradeId { get; set; }

    public bool IsActive { get; set; }
}
