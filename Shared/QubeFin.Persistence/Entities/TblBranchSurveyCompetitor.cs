using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblBranchSurveyCompetitor
{
    public Guid Id { get; set; }

    public Guid BranchSurveyId { get; set; }

    public string NameOfInstitution { get; set; } = null!;

    public int? ApproxClients { get; set; }

    public decimal? ApproxPortfolio { get; set; }

    public decimal? Parpercent { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual TblBranchSurvey BranchSurvey { get; set; } = null!;
}
