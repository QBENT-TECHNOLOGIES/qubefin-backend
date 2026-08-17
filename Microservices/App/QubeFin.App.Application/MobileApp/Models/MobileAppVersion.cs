using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.App.Application.MobileApp.Models
{
    public class MobileAppVersion
    {
        public string LatestVersion { get; set; } = string.Empty;
        public bool IsDiscontinued { get; set; }
        public string? DownloadUrl { get; set; }
    }
}
