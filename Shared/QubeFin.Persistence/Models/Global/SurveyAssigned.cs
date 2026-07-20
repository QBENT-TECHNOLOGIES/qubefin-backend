using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Persistence.Models.Global
{
    public class SurveyAssigned
    {
        public Guid Id { get; private set; }
        public Guid SurveyId { get; private set; }
        public Guid EmployeeId { get; private set; }
        public bool IsLead { get; private set; }
        public DateTime AssignedDate { get; private set; }
        public Guid AssignedBy { get; private set; }
        private SurveyAssigned() { }
        public SurveyAssigned(Guid id, Guid surveyId, Guid employeeId, bool isLead, Guid assignedBy)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            SurveyId = surveyId;
            EmployeeId = employeeId;
            IsLead = isLead;
            AssignedDate = DateTime.Now;
            AssignedBy = assignedBy;
        }
        public static SurveyAssigned Create(Guid surveyId, Guid employeeId, bool isLead, Guid assignedBy)
        {
            return new SurveyAssigned(Guid.Empty, surveyId, employeeId, isLead, assignedBy);
        }
        public void Update(bool isLead)
        {
            IsLead = isLead;
        }
    }
}
