using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chalkable.Web.Models.FinalGradesViewData
{
    public class FinalGradeInputModel
    {
        public Guid FinalGradeId { get; set; }
        public int ParticipationPercent { get; set; }
        public int Discipline { get; set; }
        public bool DropLowestDiscipline { get; set; }
        public int Attendance { get; set; }
        public bool DropLowestAttendance { get; set; }
        public int GradingStyle { get; set; }
        public IList<FinalGradeAnnouncementTypeInputModel> FinalGradeAnnouncementType { get; set; }
    }

    public class FinalGradeAnnouncementTypeInputModel
    {
        public Guid FinalGradeAnnouncementTypeId { get; set; }
        public bool DropLowest { get; set; }
        public int PercentValue { get; set; }
        public int GradingStyle { get; set; }
        public Guid FinalGradeId { get; set; }
    }
}