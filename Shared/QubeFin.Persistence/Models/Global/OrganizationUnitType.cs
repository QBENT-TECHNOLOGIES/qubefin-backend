using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Persistence.Models.Global
{
    public class OrganizationUnitType
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; } = null!;

        public int LevelNo { get; private set; }
        private OrganizationUnitType() { }
        public OrganizationUnitType(Guid id, string name, int levelNo)
        {
            Id = id;
            Name = name;
            LevelNo = levelNo;
        }

    }
}
