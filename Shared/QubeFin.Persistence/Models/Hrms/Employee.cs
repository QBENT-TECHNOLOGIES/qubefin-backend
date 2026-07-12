namespace QubeFin.Persistence.Models.Hrms
{
    public class Employee
    {
        private readonly List<EmployeeDesignation> _designations = [];
        private readonly List<EmployeeQualification> _qualifications = [];
        private readonly List<EmployeeEmployment> _employments = [];
        private readonly List<EmployeeDocument> _documents = [];
        private readonly List<EmployeeReference> _references = [];

        public Guid Id { get; private set; }
        public string Code { get; private set; } = null!;
        public Guid? CreatedBy { get; private set; }
        public DateTime? CreatedDate { get; private set; }
        public DateTime? LastModifiedOn { get; private set; }
        public Guid? LastModifiedBy { get; private set; }
        public PersonalInfo PersonalInfo { get; private set; } = default!;
        public OfficialInfo OfficialInfo { get; private set; } = default!;
        public ContactInfo ContactInfo { get; private set; } = default!;
        public AddressInfo PresentAddressInfo { get; private set; } = default!;
        public AddressInfo PermanentAddressInfo { get; private set; } = default!;
        public PayrollInfo PayrollInfo { get; private set; } = default!;


        public IReadOnlyCollection<EmployeeDesignation> Designations => _designations;
        public IReadOnlyCollection<EmployeeQualification> Qualifications => _qualifications;
        public IReadOnlyCollection<EmployeeEmployment> Employments => _employments;
        public IReadOnlyCollection<EmployeeDocument> Documents => _documents;
        public IReadOnlyCollection<EmployeeReference> References => _references;

        private Employee()
        {
        }

        public Employee(Guid id, string code, PersonalInfo personalInfo, OfficialInfo officialInfo, ContactInfo contactInfo,
            AddressInfo presentAddressInfo, AddressInfo permanentAddressInfo, PayrollInfo payrollInfo,
            Guid? createdBy, DateTime? createdOn, Guid? lastModifiedBy, DateTime? lastModifiedOn)
        {
            Id = id;
            Code = code;
            PersonalInfo = personalInfo;
            OfficialInfo = officialInfo;
            ContactInfo = contactInfo;
            PresentAddressInfo = presentAddressInfo;
            PermanentAddressInfo = permanentAddressInfo;
            PayrollInfo = payrollInfo;
            CreatedBy = createdBy;
            CreatedDate = createdOn;
            LastModifiedBy = lastModifiedBy;
            LastModifiedOn = lastModifiedOn;
        }

        public static Employee Create(
            Guid id,
            string code,
            PersonalInfo personalInfo,
            OfficialInfo officialInfo,
            ContactInfo contactInfo,
            AddressInfo presentAddressInfo,
            AddressInfo permanentAddressInfo,
            PayrollInfo payrollInfo,
            Guid createdBy)
        {
            return new Employee
            {
                Id = id,
                Code = code,
                PersonalInfo = personalInfo,
                OfficialInfo = officialInfo,
                ContactInfo = contactInfo,
                PresentAddressInfo = presentAddressInfo,
                PermanentAddressInfo = permanentAddressInfo,
                PayrollInfo = payrollInfo,
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

        public void UpdateOfficialInfo(OfficialInfo official, Guid modifiedBy)
        {
            ArgumentNullException.ThrowIfNull(official);
            OfficialInfo = official;
            SetModified(modifiedBy);
        }

        public void UpdateContactInfo(ContactInfo contact, Guid modifiedBy)
        {
            ArgumentNullException.ThrowIfNull(contact);
            ContactInfo = contact;
            SetModified(modifiedBy);
        }

        public void UpdateAddressInfo(AddressInfo presentAddress, AddressInfo permanentAddress, Guid modifiedBy)
        {
            ArgumentNullException.ThrowIfNull(presentAddress);
            ArgumentNullException.ThrowIfNull(permanentAddress);
            PresentAddressInfo = presentAddress;
            PermanentAddressInfo = permanentAddress;
            SetModified(modifiedBy);
        }

        public void UpdatePayrollInfo(PayrollInfo payroll, Guid modifiedBy)
        {
            ArgumentNullException.ThrowIfNull(payroll);
            PayrollInfo = payroll;
            SetModified(modifiedBy);
        }

        public void AddQualification(EmployeeQualification qualification)
        {
            ArgumentNullException.ThrowIfNull(qualification);
            _qualifications.Add(qualification);
        }
        public void RemoveQualification(Guid qualificationId)
        {
            var qualification = _qualifications
                .FirstOrDefault(x => x.Id == qualificationId);

            if (qualification != null)
            {
                _qualifications.Remove(qualification);
            }
        }

        public void AddDesignation(EmployeeDesignation designation)
        {
            ArgumentNullException.ThrowIfNull(designation);
            _designations.Add(designation);
        }
        public void UpdateDesignation(Guid designationId, Guid newDesignationId, DateTime effectiveFrom, DateTime? effectiveTo)
        {
            var designation = _designations
                .FirstOrDefault(x => x.Id == designationId);

            if (designation is null)
            {
                throw new InvalidOperationException("Designation not found.");
            }

            designation.Update(newDesignationId, effectiveFrom, effectiveTo);
        }
        public void RemoveDesignation(Guid designationId)
        {
            var designation = _designations
                .FirstOrDefault(x => x.Id == designationId);

            if (designation != null)
            {
                _designations.Remove(designation);
            }
        }
        public void ReplaceDesignations(IEnumerable<EmployeeDesignation> designations)
        {
            _designations.Clear();
            _designations.AddRange(designations);
        }

        private void SetModified(Guid userId)
        {
            LastModifiedBy = userId;
            LastModifiedOn = DateTime.UtcNow;
        }
    }
}
