using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblSurvey
{
    public Guid Id { get; set; }

    public int Sequence { get; set; }

    public string SurveyType { get; set; } = null!;

    public string AssignmentNo { get; set; } = null!;

    public DateOnly AssignmentDate { get; set; }

    public string ProposedArea { get; set; } = null!;

    public Guid AdministrativeUnitId { get; set; }

    public DateOnly TentativeSubmissionDate { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual TblAdministrativeUnit AdministrativeUnit { get; set; } = null!;

    public virtual ICollection<TblBranchSurvey> TblBranchSurveys { get; set; } = new List<TblBranchSurvey>();

    public virtual ICollection<TblSurveyAssigned> TblSurveyAssigneds { get; set; } = new List<TblSurveyAssigned>();

    public virtual ICollection<TblSurveyCommitteeEvaluation> TblSurveyCommitteeEvaluations { get; set; } = new List<TblSurveyCommitteeEvaluation>();

    public virtual ICollection<TblSurveyDocument> TblSurveyDocuments { get; set; } = new List<TblSurveyDocument>();
}
