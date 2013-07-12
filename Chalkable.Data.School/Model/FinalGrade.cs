using System;
using System.Collections.Generic;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class FinalGrade
    {
        public Guid Id { get; set; }
        public FinalGradeStatus Status { get; set; }
        public int ParticipationPercent { get; set; }
        public int Discipline { get; set; }
        public bool DropLowestDiscipline { get; set; }
        public int Attendance { get; set; }
        public bool DropLowestAttendance { get; set; }
        public GradingStyleEnum GradingStyle { get; set; }

    }

    public class VwFinalGrade : FinalGrade
    {
        [DataEntityAttr]
        public MarkingPeriodClass MarkingPeriodClass { get; set; }
        [DataEntityAttr]
        public ClassComplex Class { get; set; }
    }

    public class FinalGradeDetails : VwFinalGrade
    {
        public IList<FinalGradeAnnouncementType> FinalGradeAnnouncementTypes { get; set; }

        
        private IList<StudentFinalGradeDetails> studentFinalGrades;
        public IList<StudentFinalGradeDetails> StudentFinalGrades
        {
            get { return studentFinalGrades; }
            set
            {
                studentFinalGrades = value;
                foreach (var studentFinalGrade in studentFinalGrades)
                {
                    studentFinalGrade.FinalGrade = this;
                }
            }
        } 
    }

    public enum FinalGradeStatus
    {
        Open = 0,
        Submit = 1,
        Approve = 2
    }
}
