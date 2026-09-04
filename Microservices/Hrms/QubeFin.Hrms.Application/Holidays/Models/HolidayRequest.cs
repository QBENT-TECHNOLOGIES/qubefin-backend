using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Hrms.Application.Holidays.Models
{
    public class HolidayRequest
    {
        public Guid Id { get; set; }
        public DateOnly HolidayDate { get; set; }
        public string Description {  get; set; }
        public List<Guid> OrgUnitIds { get; set; } = new List<Guid>();
    }
}
