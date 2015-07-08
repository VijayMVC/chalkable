using System;
using Chalkable.BusinessLogic.Model.Attendances;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.ClassesViewData
{
    public class ClassAttendanceSummaryViewData : ClassViewData
    {
        public BaseBoxesViewData Absences { get; set; }
        public BaseBoxesViewData Lates { get; set; }
        public BaseBoxesViewData Presents { get; set; }
        
        protected ClassAttendanceSummaryViewData(ClassDetails classDetails)
            : base(classDetails)
        {   
        }
        public static ClassAttendanceSummaryViewData Create(ClassDetails classDetails, ClassAttendanceSummary attendanceSummary)
        {
            return new ClassAttendanceSummaryViewData(classDetails)
                {
                    Absences = BuildBoxViewData(attendanceSummary, x => x.Absences, 5),
                    Lates = BuildBoxViewData(attendanceSummary, x => x.Tardies, 10),
                    Presents = BuildBoxViewData(attendanceSummary, x => x.Presents, 101)
                };
        }

        private static BaseBoxesViewData BuildBoxViewData(ClassAttendanceSummary attendanceSummary, Func<ClassAttendanceSummary, decimal?> getAttendanceAction, int persents)
        {
            var attIssues = getAttendanceAction(attendanceSummary) ?? 0;
            return new BaseBoxesViewData
                {
                    Title = attIssues.ToString(),
                    IsPassing = ((attIssues * 100) / attendanceSummary.PosibleAttendanceCount < persents)
                };
        }
    }
}