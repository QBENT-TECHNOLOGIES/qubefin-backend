using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblSurveyDocument
{
    public Guid Id { get; set; }

    public Guid SurveyId { get; set; }

    public string FileName { get; set; } = null!;

    public string FileSequence { get; set; } = null!;

    public int Sequence { get; set; }

    public virtual TblSurvey Survey { get; set; } = null!;
}
