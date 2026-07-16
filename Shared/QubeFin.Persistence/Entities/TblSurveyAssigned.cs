using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblSurveyAssigned
{
    public Guid Id { get; set; }

    public Guid SurveyId { get; set; }

    public Guid EmployeeId { get; set; }

    public bool IsLead { get; set; }

    public DateTime AssignedDate { get; set; }

    public Guid AssignedBy { get; set; }

    public virtual TblEmployee Employee { get; set; } = null!;

    public virtual TblSurvey Survey { get; set; } = null!;
}
