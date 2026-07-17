using QubeFin.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Global.Application.Survey.Models
{
    public class SurveySearchParam : SearchParam
    {
        public string? SurveyType { get; set; }
        public DateOnly? SurveyFrom { get; set; }
        public DateOnly? SurveyTo { get; set; }
    }
}
