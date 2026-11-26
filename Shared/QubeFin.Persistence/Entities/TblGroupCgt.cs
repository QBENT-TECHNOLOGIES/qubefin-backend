using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblGroupCgt
{
    public Guid Id { get; set; }

    public Guid SurveyId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public Guid ConductedByEmployeeId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? CompletedOn { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual TblEmployee ConductedByEmployee { get; set; } = null!;

    public virtual TblSurvey Survey { get; set; } = null!;

    public virtual ICollection<TblGroupCgtTopic> TblGroupCgtTopics { get; set; } = new List<TblGroupCgtTopic>();
}
