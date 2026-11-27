namespace QubeFin.Persistence.Models.Members
{
    public class Member
    {
        public Guid Id { get; private set; }
        public string? Code { get; private set; }
        public Guid? DesignationId { get; private set; }
        public Guid CreatedBy { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime? LastModifiedOn { get; private set; }
        public Guid? LastModifiedBy { get; private set; }
        public PersonalInfo PersonalInfo { get; private set; } = default!;
        public List<MemberAddress> Addresses { get; private set; } = [];
        public List<MemberDocument> Documents { get; private set; } = [];

        private Member() { }

        public Member(
            Guid id,
            string? code,
            PersonalInfo personalInfo,
            Guid createdBy,
            DateTime createdOn,
            Guid? lastModifiedBy,
            DateTime? lastModifiedOn,
            List<MemberAddress>? addresses = null,
            List<MemberDocument>? documents = null,
            Guid? designationId = null)
        {
            Id = id;
            Code = code;
            PersonalInfo = personalInfo;
            CreatedBy = createdBy;
            CreatedDate = createdOn;
            LastModifiedBy = lastModifiedBy;
            LastModifiedOn = lastModifiedOn;
            Addresses = addresses ?? [];
            Documents = documents ?? [];
            DesignationId = designationId;
        }

        public static Member Create(
            Guid id,
            string? code,
            PersonalInfo personalInfo,
            Guid createdBy,
            Guid? designationId = null)
        {
            ArgumentNullException.ThrowIfNull(personalInfo);

            return new Member
            {
                Id = id,
                Code = code,
                DesignationId = designationId,
                PersonalInfo = personalInfo,
                CreatedBy = createdBy,
                CreatedDate = DateTime.UtcNow
            };
        }

        public void UpdatePersonalInfo(PersonalInfo personal, Guid modifiedBy)
        {
            ArgumentNullException.ThrowIfNull(personal);
            PersonalInfo = personal;
            SetModified(modifiedBy);
        }

        public void UpdateAddresses(List<MemberAddress> addresses, Guid modifiedBy)
        {
            ArgumentNullException.ThrowIfNull(addresses);
            Addresses = addresses;
            SetModified(modifiedBy);
        }

        public void UpdateDocuments(List<MemberDocument> documents, Guid modifiedBy)
        {
            ArgumentNullException.ThrowIfNull(documents);
            Documents = documents;
            SetModified(modifiedBy);
        }

        public void SetModified(Guid userId)
        {
            LastModifiedBy = userId;
            LastModifiedOn = DateTime.UtcNow;
        }
    }
}