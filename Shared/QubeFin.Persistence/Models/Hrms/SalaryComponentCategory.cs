using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Persistence.Models.Hrms
{
    public class SalaryComponentCategory
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; } = null!;
        private SalaryComponentCategory() { }
        public SalaryComponentCategory(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
