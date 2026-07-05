using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblSystemValue
{
    public string SysKey { get; set; } = null!;

    public string SysVal { get; set; } = null!;
}
