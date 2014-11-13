using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAttendanceService
    {
        IList<ClassAttendanceDetails> GetClassAttendances(DateTime date, int classId);
        void SetClassAttendances(DateTime date, int classId, IList<ClassAttendance> items);
        SeatingChartInfo GetSeatingChart(int classId, int markingPeriodId);
        void UpdateSeatingChart(int classId, int markingPeriodId, SeatingChartInfo seatingChart);
        AttendanceSummary GetAttendanceSummary(int teacherId, GradingPeriod gradingPeriod);
    }

    public class AttendanceService : SisConnectedService, IAttendanceService
    {
        public AttendanceService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void SetClassAttendances(DateTime date, int classId, IList<ClassAttendance> items)
        {
            var dataStr = date.ToString("yyyy-MM-dd");
            var sa = new SectionAttendance
            {
                Date = dataStr,
                SectionId = classId,
                StudentAttendance = new List<StudentSectionAttendance>()
            };
            foreach (var item in items)
            {

                sa.StudentAttendance.Add(new StudentSectionAttendance
                {
                    Category = item.Category,
                    Date = dataStr,
                    ClassroomLevel = LevelToClassRoomLevel(item.Level),
                    ReasonId = (short)(item.AttendanceReasonRef.HasValue ? item.AttendanceReasonRef.Value : 0),
                    SectionId = classId,
                    StudentId = item.PersonRef,
                });
            }
            if (!Context.SchoolYearId.HasValue)
                throw new ChalkableException(ChlkResources.ERR_CANT_DETERMINE_SCHOOL_YEAR);
            ConnectorLocator.AttendanceConnector.SetSectionAttendance(Context.SchoolYearId.Value, date, classId, sa);
        }

        private string LevelToClassRoomLevel(string level)
        {
            if (level == null)
                return "Present";
            if (level == "T")
                return "Tardy";
            if (level == "A" || level == "AO")
                return "Absent";
            return "Missing";
        }

        public IList<ClassAttendanceDetails> GetClassAttendances(DateTime date, int classId)
        {
            var mp = ServiceLocator.MarkingPeriodService.GetMarkingPeriodByDate(date, true);
            if (mp == null)
            {
                //throw new ChalkableException("No marking period is scheduled on this date");
                return null;
            }

            var sa = ConnectorLocator.AttendanceConnector.GetSectionAttendance(date, classId);
            if (sa != null)
            {
                var clazz = ServiceLocator.ClassService.GetClassDetailsById(classId);
                var persons = ServiceLocator.PersonService.GetClassStudents(classId, mp.Id);
                var attendances = new List<ClassAttendanceDetails>();
                foreach (var ssa in sa.StudentAttendance)
                {
                    var student = persons.FirstOrDefault(x => x.Id == ssa.StudentId);
                    if (student != null)
                    {
                        attendances.Add(new ClassAttendanceDetails
                        {
                            ClassRef = ssa.SectionId,
                            AttendanceReasonRef = ssa.ReasonId,
                            Date = date,
                            PersonRef = ssa.StudentId,
                            Level = ClassroomLevelToLevelCvt(ssa.ClassroomLevel),
                            Class = clazz,
                            Student = student,
                            Category = ssa.Category,
                            IsPosted = sa.IsPosted,
                            AbsentPreviousDay = ssa.AbsentPreviousDay,
                            ReadOnly = sa.ReadOnly,
                            ReadOnlyReason = sa.ReadOnlyReason
                        });
                    }
                }
                return attendances;    
            }
            return null;
        }

        private static string ClassroomLevelToLevelCvt(string classroomLevel)
        {
            switch (classroomLevel)
            {
                case "Present":
                    return null;
                case "Absent":
                    return "A";
                case "Tardy":
                    return "T";
                default:
                    return "H";
            }
        }

        public AttendanceSummary GetAttendanceSummary(int teacherId, GradingPeriod gradingPeriod)
        {
            var classes = ServiceLocator.ClassService.GetClasses(gradingPeriod.SchoolYearRef, gradingPeriod.MarkingPeriodRef, teacherId, 0);
            if (classes.Count == 0)
            {
                return new AttendanceSummary
                    {
                        ClassesDaysStat = new List<ClassDailyAttendanceSummary>(),
                        Students = new List<StudentAttendanceSummary>()
                    };
            }

            var classesIds = classes.Select(x => x.Id).ToList();
            var students = ServiceLocator.PersonService.GetTeacherStudents(teacherId, gradingPeriod.SchoolYearRef);
        
            var sectionsAttendanceSummary = ConnectorLocator.AttendanceConnector.GetSectionAttendanceSummary(classesIds, gradingPeriod.StartDate, gradingPeriod.EndDate);
            var res = new AttendanceSummary();
            var dailySectionAttendances = new List<DailySectionAttendanceSummary>();
            var studentAtts = new List<StudentSectionAttendanceSummary>();
            var sectionStSet = new HashSet<Pair<int, int>>();
            var sectionDaySet = new HashSet<Pair<int, DateTime>>();
            foreach (var sectionAttendanceSummary in sectionsAttendanceSummary)
            {

                foreach (var dailySectionAtt in sectionAttendanceSummary.Days)
                {
                    var pair = new Pair<int, DateTime>(dailySectionAtt.SectionId, dailySectionAtt.Date);
                    if (!sectionDaySet.Contains(pair))
                    {
                        sectionDaySet.Add(pair);
                        dailySectionAttendances.Add(dailySectionAtt);
                    }
                }
                foreach (var student in sectionAttendanceSummary.Students)
                {
                    var pair = new Pair<int, int>(student.SectionId, student.StudentId);
                    if (!sectionStSet.Contains(pair))
                    {
                        sectionStSet.Add(pair);
                        studentAtts.Add(student); 
                    }
                }
            }
            res.ClassesDaysStat = ClassDailyAttendanceSummary.Create(dailySectionAttendances, classes);
            studentAtts = studentAtts.Where(x => classesIds.Contains(x.SectionId)).ToList();
            res.Students = StudentAttendanceSummary.Create(studentAtts, students, classes);
            return res;

        }

        public SeatingChartInfo GetSeatingChart(int classId, int markingPeriodId)
        {
            var seatingChart = ConnectorLocator.SeatingChartConnector.GetChart(classId, markingPeriodId);
            if (seatingChart == null) return null;
            return SeatingChartInfo.Create(seatingChart);
        }

        public void UpdateSeatingChart(int classId, int markingPeriodId, SeatingChartInfo seatingChartInfo)
        {
            var seatingChart = new SeatingChart
                {
                    Columns = seatingChartInfo.Columns,
                    Rows = seatingChartInfo.Rows,
                    SectionId = classId,
                };
            var stiSeats = new List<Seat>();
            var students = ServiceLocator.PersonService.GetClassStudents(classId, markingPeriodId, true);
            var defaultStudent = students.FirstOrDefault(x => seatingChartInfo.SeatingList.All(y => y.All(z => z.StudentId != x.Id)));
            if (defaultStudent == null)
                defaultStudent = students.First();
            foreach (var seats in seatingChartInfo.SeatingList)
            {
                foreach (var seatInfo in seats)
                {
                    var seat = new Seat();
                    if (seatInfo.StudentId.HasValue)
                    {
                        seat.Column = seatInfo.Column;
                        seat.Row = seatInfo.Row;
                        seat.StudentId = seatInfo.StudentId.Value;
                    }
                    else seat.StudentId = defaultStudent.Id;
                    stiSeats.Add(seat);
                }
            }
            seatingChart.Seats = stiSeats;
            ConnectorLocator.SeatingChartConnector.UpdateChart(classId, markingPeriodId, seatingChart);
        }
    }
}
