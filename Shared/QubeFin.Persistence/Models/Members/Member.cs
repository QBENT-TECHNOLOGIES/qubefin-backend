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
        public AddressInfo? PresentAddressInfo { get; private set; }
        public AddressInfo? PermanentAddressInfo { get; private set; }
        public bool PresentPermanentAddressSame { get; private set; }

        private Member() { }

        public Member(
            Guid id,
            string? code,
            PersonalInfo personalInfo,
            Guid createdBy,
            DateTime createdOn,
            Guid? lastModifiedBy,
            DateTime? lastModifiedOn,
            AddressInfo? presentAddressInfo = null,
            AddressInfo? permanentAddressInfo = null,
            bool presentPermanentAddressSame = false,
            Guid? designationId = null)
        {
            Id = id;
            Code = code;
            PersonalInfo = personalInfo;
            CreatedBy = createdBy;
            CreatedDate = createdOn;
            LastModifiedBy = lastModifiedBy;
            LastModifiedOn = lastModifiedOn;
            PresentAddressInfo = presentAddressInfo;
            PermanentAddressInfo = permanentAddressInfo;
            PresentPermanentAddressSame = presentPermanentAddressSame;
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

        public void UpdateAddressInfo(
            AddressInfo presentAddress,
            AddressInfo permanentAddress,
            bool presentPermanentAddressSame,
            Guid modifiedBy)
        {
            ArgumentNullException.ThrowIfNull(presentAddress);
            ArgumentNullException.ThrowIfNull(permanentAddress);
            PresentAddressInfo = presentAddress;
            PermanentAddressInfo = permanentAddress;
            PresentPermanentAddressSame = presentPermanentAddressSame;
            SetModified(modifiedBy);
        }

        public void SetModified(Guid userId)
        {
            LastModifiedBy = userId;
            LastModifiedOn = DateTime.UtcNow;
        }
    }
}