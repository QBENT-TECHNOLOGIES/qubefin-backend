using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblMobileAppVersion
{
    public Guid Id { get; set; }

    public string Version { get; set; } = null!;

    public string AppUrl { get; set; } = null!;

    public bool IsCurrentVersion { get; set; }

    public DateTime CreatedOn { get; set; }
}
