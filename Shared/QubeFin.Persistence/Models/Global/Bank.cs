using System;

namespace QubeFin.Persistence.Models.Global
{
    public class Bank
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; } = null!;

        public bool IsBank { get; private set; }

        public Guid CreatedBy { get; private set; }

        public DateTime CreatedOn { get; private set; }

        public Guid? LastModifiedBy { get; private set; }

        public DateTime? LastModifiedOn { get; private set; }

        protected Bank() { }

        public Bank(Guid id, string name, bool isBank, Guid createdBy, DateTime createdOn, Guid? lastModifiedBy, DateTime? lastModifiedOn)
        {
            Id = id;
            Name = name;
            IsBank = isBank;
            CreatedBy = createdBy;
            CreatedOn = createdOn;
            LastModifiedBy = lastModifiedBy;
            LastModifiedOn = lastModifiedOn;
        }
    }
}
