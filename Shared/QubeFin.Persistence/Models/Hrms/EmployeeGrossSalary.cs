using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Persistence.Models.Hrms
{
    public class EmployeeGrossSalary
    {
        public Guid Id { get; set; }
        public DateOnly EffectiveFrom { get; set; }
        public DateOnly? EffectiveTill { get; set; }
        public decimal GrossSalary { get; set; }
        public EmployeeGrossSalary() { }
        public EmployeeGrossSalary(Guid id, DateOnly effectiveFrom, DateOnly? effectiveTill, decimal grossSalary)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            EffectiveFrom = effectiveFrom;
            EffectiveTill = effectiveTill;
            GrossSalary = grossSalary;
        }
    }
}
