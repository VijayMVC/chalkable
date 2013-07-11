using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public bool DropLowestAttendacne { get; set; }
        public GradingStyleEnum GradingStyle { get; set; }
    }

    public enum FinalGradeStatus
    {
        Open = 0,
        Submit = 1,
        Approve = 2
    }
}
