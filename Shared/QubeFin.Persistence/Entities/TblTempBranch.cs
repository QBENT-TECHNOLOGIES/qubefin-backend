using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblTempBranch
{
    public string? Name { get; set; }

    public string? Address { get; set; }

    public string? GoogleAddress { get; set; }

    public string? Lat { get; set; }

    public string? Lon { get; set; }
}
