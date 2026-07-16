using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Persistence.Models.Global
{
    public class Company
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; } = null!;
        protected Company() { }
        public Company(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

    }
}
