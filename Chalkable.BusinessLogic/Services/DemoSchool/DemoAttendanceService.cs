using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoAttendanceService : DemoSchoolServiceBase, IAttendanceService
    {
        public DemoAttendanceService(IServiceLocatorSchool serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
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
                    Level = item.Level,
                    ReasonId = (short?)item.AttendanceReasonRef,
                    SectionId = classId,
                    StudentId = item.PersonRef
                });
            }
            Storage.StiAttendanceStorage.SetSectionAttendance(date, classId, sa);
        }

        public SeatingChartInfo GetSeatingChart(int classId, int markingPeriodId)
        {
            var seatingChart = Storage.StiSeatingChartStorage.GetChart(classId, markingPeriodId);
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
            var students = ServiceLocator.PersonService.GetClassStudents(classId, markingPeriodId);
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
            Storage.StiSeatingChartStorage.UpdateChart(classId, markingPeriodId, seatingChart);
        }

        public AttendanceSummary GetAttendanceSummary(int teacherId, GradingPeriod gradingPeriod)
        {
            Trace.Assert(Context.SchoolYearId.HasValue);
            var classes = ServiceLocator.ClassService.GetClasses(gradingPeriod.SchoolYearRef, gradingPeriod.MarkingPeriodRef, teacherId, 0);
            var classesIds = classes.Select(x => x.Id).ToList();
            var students = ServiceLocator.PersonService.GetTeacherStudents(teacherId, Context.SchoolYearId.Value);
            var sectionsAttendanceSummary = Storage.StiAttendanceStorage.GetSectionAttendanceSummary(classesIds, gradingPeriod.StartDate, gradingPeriod.EndDate);
            var res = new AttendanceSummary();
            var dailySectionAttendances = new List<DailySectionAttendanceSummary>();
            var studentAtts = new List<StudentSectionAttendanceSummary>();
            foreach (var sectionAttendanceSummary in sectionsAttendanceSummary)
            {
                dailySectionAttendances.AddRange(sectionAttendanceSummary.Days);
                studentAtts.AddRange(sectionAttendanceSummary.Students);
            }
            res.ClassesDaysStat = ClassDailyAttendanceSummary.Create(dailySectionAttendances, classes);
            studentAtts = studentAtts.Where(x => classesIds.Contains(x.SectionId)).ToList();
            res.Students = StudentAttendanceSummary.Create(studentAtts, students, classes);
            return res;
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
            var markingPeriod = ServiceLocator.MarkingPeriodService.GetMarkingPeriodByDate(date, true);
            if (markingPeriod == null)
            {
                throw new ChalkableException("No marking period is scheduled for this date");
            }

            var sa = Storage.StiAttendanceStorage.GetSectionAttendance(date, classId);
            if (sa != null)
            {
                var clazz = ServiceLocator.ClassService.GetClassDetailsById(classId);
                var persons = ServiceLocator.PersonService.GetClassStudents(classId, markingPeriod.Id);
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
                            Level = ssa.Level,
                            Class = clazz,
                            Student = student,
                            Category = ssa.Category,
                            IsPosted = sa.IsPosted
                        });
                    }
                }
                return attendances;    
            }
            return null;
        }
    }
}
