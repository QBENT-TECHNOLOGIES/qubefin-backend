using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Persistence.Models
{
    public class SearchParam
    {
        public string? SearchText { get; set; }
        public string SortOn { get; set; } = string.Empty;
        public string SortDirection { get; set; } = string.Empty;
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
