using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblSurveyApproval
{
    public Guid Id { get; set; }

    public Guid SurveyId { get; set; }

    public Guid AuthorityEmployeeId { get; set; }

    public string Action { get; set; } = null!;

    public string? Comments { get; set; }

    public DateTime ActionOn { get; set; }

    public virtual TblEmployee AuthorityEmployee { get; set; } = null!;

    public virtual TblSurvey Survey { get; set; } = null!;
}
