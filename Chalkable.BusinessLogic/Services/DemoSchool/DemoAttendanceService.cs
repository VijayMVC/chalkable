using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Model.Attendances;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;
using Chalkable.StiConnector.Connectors.Model.Attendances;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoAttendanceService : DemoSchoolServiceBase, IAttendanceService
    {
        public DemoAttendanceService(IServiceLocatorSchool serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
        }

        public void SetClassAttendances(DateTime date, int classId, IList<StudentClassAttendance> studentAttendances)
        {
            var dataStr = date.ToString(Constants.DATE_FORMAT);
            var sa = new SectionAttendance
            {
                Date = dataStr,
                SectionId = classId,
                StudentAttendance = new List<StudentSectionAttendance>()
            };
            sa.StudentAttendance = studentAttendances.Select(sca => new StudentSectionAttendance
                {
                    Category = sca.Category,
                    Date = dataStr,
                    ClassroomLevel = LevelToClassRoomLevel(sca.Level),
                    Level = sca.Level,
                    ReasonId = (short)(sca.AttendanceReasonId.HasValue ? sca.AttendanceReasonId.Value : 0),
                    SectionId = classId,
                    StudentId = sca.StudentId,
                }).ToList();
            Storage.StiAttendanceStorage.SetSectionAttendance(date, classId, sa);
        }

        public ClassAttendanceDetails GetClassAttendance(DateTime date, int classId)
        {
            var markingPeriod = ServiceLocator.MarkingPeriodService.GetMarkingPeriodByDate(date, true);
            if (markingPeriod == null)
                throw new NoMarkingPeriodException("No marking period is scheduled for this date");

            var sa = Storage.StiAttendanceStorage.GetSectionAttendance(date, classId);
            if (sa != null)
            {
                var clazz = ServiceLocator.ClassService.GetClassDetailsById(classId);
                var persons = ServiceLocator.StudentService.GetClassStudents(classId, markingPeriod.Id);
                var res = new ClassAttendanceDetails
                {
                    Class = clazz,
                    Date = date,
                    IsDailyAttendancePeriod = sa.IsDailyAttendancePeriod,
                    ClassId = sa.SectionId,
                    IsPosted = sa.IsPosted,
                    MergeRosters = sa.MergeRosters,
                    ReadOnly = sa.ReadOnly,
                    ReadOnlyReason = sa.ReadOnlyReason,
                    StudentAttendances = new List<StudentClassAttendance>()
                };

                foreach (var ssa in sa.StudentAttendance)
                {
                    var student = persons.FirstOrDefault(x => x.Id == ssa.StudentId);
                    if (student != null)
                    {
                        res.StudentAttendances.Add(new StudentClassAttendance
                        {
                            ClassId = ssa.SectionId,
                            AttendanceReasonId = ssa.ReasonId,
                            Date = date,
                            StudentId = ssa.StudentId,
                            Level = ssa.Level,
                            Student = student,
                            Category = ssa.Category,
                            AbsentPreviousDay = ssa.AbsentPreviousDay,
                            ReadOnly = ssa.ReadOnly,
                            ReadOnlyReason = ssa.ReadOnlyReason,
                        });
                    }
                }
                return res;    
            }
            return null;
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
            var students = ServiceLocator.StudentService.GetClassStudents(classId, markingPeriodId);
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
            var classes = ServiceLocator.ClassService.GetTeacherClasses(gradingPeriod.SchoolYearRef, teacherId, gradingPeriod.MarkingPeriodRef);
            var classesIds = classes.Select(x => x.Id).ToList();
            var students = ServiceLocator.StudentService.GetTeacherStudents(teacherId, Context.SchoolYearId.Value);
            var sectionsAttendanceSummary = Storage.StiAttendanceStorage.GetSectionAttendanceSummary(classesIds, gradingPeriod.StartDate, gradingPeriod.EndDate);
            var res = new AttendanceSummary();
            var dailySectionAttendances = new List<DailySectionAbsenceSummary>();
            var studentAtts = new List<StudentSectionAbsenceSummary>();
            foreach (var sectionAttendanceSummary in sectionsAttendanceSummary)
            {
                dailySectionAttendances.AddRange(sectionAttendanceSummary.Days);
                studentAtts.AddRange(sectionAttendanceSummary.Students);
            }
            res.ClassesDaysStat = ClassDailyAttendanceSummary.Create(dailySectionAttendances, classes);
            studentAtts = studentAtts.Where(x => classesIds.Contains(x.SectionId)).ToList();
            res.Students = ShortStudentAttendanceSummary.Create(studentAtts, students, classes);
            return res;
        }

        private string LevelToClassRoomLevel(string level)
        {
            if (level == null)
                return StudentClassAttendance.PRESENT;
            if (StudentClassAttendance.IsLateLevel(level))
                return StudentClassAttendance.TARDY;
            if (level == "A" || level == "AO")
                return StudentClassAttendance.ABSENT;
            return StudentClassAttendance.MISSING;
        }


        public IList<ClassDetails> GetNotTakenAttendanceClasses(DateTime dateTime)
        {
            return new List<ClassDetails>();
        }

        public IList<StudentAttendanceDetails> GetStudentAttendanceDetailsByDateRange(int studentId, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public FullStudentAttendanceSummary GetStudentAttendanceSummary(int studentId, int? markingPeriodId)
        {
            throw new NotImplementedException();
        }
    }
}
