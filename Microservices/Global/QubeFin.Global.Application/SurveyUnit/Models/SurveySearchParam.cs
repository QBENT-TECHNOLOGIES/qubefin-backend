using QubeFin.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Global.Application.SurveyUnit.Models
{
    public class SurveySearchParam : SearchParam
    {
        public string? SurveyType { get; set; }
        public DateOnly? SurveyFrom { get; set; }
        public DateOnly? SurveyTo { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid UserId { get; set; }
    }
}
