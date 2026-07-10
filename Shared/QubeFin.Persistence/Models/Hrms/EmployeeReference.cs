using QubeFin.Persistence.Models.Global;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Persistence.Models.Hrms
{
    public class EmployeeReference
    {
        public Guid Id { get; set; }
        public string DocumentCategory { get; set; } = null!;
        public string DocumentName { get; set; } = null!;
        public string? DocumentNo { get; set; }
        public DateOnly? ValidFrom { get; set; }
        public DateOnly? ValidTill { get; set; }
        public string? FileName { get; set; }
        public string? FileNo { get; set; }
        public Guid EmployeeId { get; set; }
        public DateTime? UploadedOn { get; set; }
        public Guid? UploadedBy { get; set; }


        private EmployeeReference() { }
        public EmployeeReference(
            Guid id,
            string documentCategory,
            string documentName,
            string? documentNo,
            DateOnly? validFrom,
            DateOnly? validTill,
            string? fileName,
            string? fileNo,
            Guid employeeId,
            Guid? uploadedBy,
            DateTime? uploadedOn
        )
        {
            Id = id;
            DocumentCategory = documentCategory;
            DocumentName = documentName;
            DocumentNo = documentNo;
            ValidFrom = validFrom;
            ValidTill = validTill;
            FileName = fileName;
            FileNo = fileNo;
            EmployeeId = employeeId;
            UploadedOn = uploadedOn;
            UploadedBy = uploadedBy;
        }
    }


}
