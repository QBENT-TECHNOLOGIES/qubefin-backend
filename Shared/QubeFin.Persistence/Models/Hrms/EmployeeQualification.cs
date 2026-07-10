using QubeFin.Persistence.Models.Global;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Persistence.Models.Hrms
{
   public class EmployeeQualification
    {
        public Guid Id { get; private set; }
        public string AcademicStream { get; private set; } = null!;
        public string? Specialization { get; private set; }
        public int YearOfPassing { get; private set; }
        public string? UniversityOrBoard { get; private set; }
        public string? SchoolOrCollege { get; private set; }
        public string? GradeOrMarks { get; private set; }
        public string? DocFileName { get; private set; }
        public string? DocFileNo { get; private set; }
        public Guid EmployeeId { get; private set; }
        public int Sequence { get; private set; }

        private EmployeeQualification() { }
        public EmployeeQualification(
            Guid id,
            string academicStream,
            string? specialization,
            int yearOfPassing,
            string? universityOrBoard,
            string? schoolOrCollege,
            string? gradeOrMarks,
            string? docFileName,
            string? docFileNo,
            Guid employeeId,
            int sequence
        )
        {
            Id = id;
            AcademicStream = academicStream;
            Specialization = specialization;
            YearOfPassing = yearOfPassing;
            UniversityOrBoard = universityOrBoard;
            SchoolOrCollege = schoolOrCollege;
            GradeOrMarks = gradeOrMarks;
            DocFileName = docFileName;
            DocFileNo = docFileNo;
            EmployeeId = employeeId;
            Sequence = sequence;
        }
    }


}
