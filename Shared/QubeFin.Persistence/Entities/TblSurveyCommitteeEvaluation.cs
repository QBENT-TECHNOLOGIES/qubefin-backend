using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblSurveyCommitteeEvaluation
{
    public Guid Id { get; set; }

    public Guid SurveyId { get; set; }

    public Guid EmployeeId { get; set; }

    public string Comments { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime EvaluationDate { get; set; }

    public virtual TblEmployee Employee { get; set; } = null!;

    public virtual TblSurvey Survey { get; set; } = null!;
}
