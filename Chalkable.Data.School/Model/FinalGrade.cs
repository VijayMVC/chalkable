using System;
using System.Collections.Generic;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class FinalGrade
    {
        public virtual Guid Id { get; set; }
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
        private ClassDetails _class;
        private MarkingPeriodClass markingPeriodClass;
        
        public override Guid Id
        {
            get
            {
                return base.Id;
            }
            set
            {
                base.Id = value;
                if (markingPeriodClass != null)
                    markingPeriodClass.Id = value;
            }
        }
        [DataEntityAttr]
        public MarkingPeriodClass MarkingPeriodClass
        {
            get { return markingPeriodClass; } 
            set
            {
                markingPeriodClass = value;
                if (value != null)
                {
                    if(Id != Guid.Empty)
                        markingPeriodClass.Id = Id;
                    if (_class != null && _class.Id != Guid.Empty)
                        markingPeriodClass.ClassRef = _class.Id;
                }
            }
        }
        [DataEntityAttr]
        public ClassDetails Class
        {
            get { return _class; }
            set
            {
                _class = value;
                if (value != null && markingPeriodClass != null)
                    markingPeriodClass.ClassRef = _class.Id;

            }
        }
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
                if (value != null)
                {
                    foreach (var studentFinalGrade in studentFinalGrades)
                    {
                        studentFinalGrade.FinalGrade = this;
                    }
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
