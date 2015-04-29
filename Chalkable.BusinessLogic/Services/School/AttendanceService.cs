using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        ClassAttendanceDetails GetClassAttendance(DateTime date, int classId);
        void SetClassAttendances(DateTime date, int classId, IList<StudentClassAttendance> studentAttendances);
        SeatingChartInfo GetSeatingChart(int classId, int markingPeriodId);
        void UpdateSeatingChart(int classId, int markingPeriodId, SeatingChartInfo seatingChart);
        AttendanceSummary GetAttendanceSummary(int teacherId, GradingPeriod gradingPeriod);
        IList<ClassDetails> GetNotTakenAttendanceClasses(DateTime date);
    }

    public class AttendanceService : SisConnectedService, IAttendanceService
    {
        public AttendanceService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
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
                    ReasonId = (short) (sca.AttendanceReasonId.HasValue ? sca.AttendanceReasonId.Value : 0),
                    SectionId = classId,
                    StudentId = sca.StudentId,
                }).ToList();
            if (!Context.SchoolYearId.HasValue)
                throw new ChalkableException(ChlkResources.ERR_CANT_DETERMINE_SCHOOL_YEAR);
            ConnectorLocator.AttendanceConnector.SetSectionAttendance(Context.SchoolYearId.Value, date, classId, sa);
        }

        public ClassAttendanceDetails GetClassAttendance(DateTime date, int classId)
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
                var persons = ServiceLocator.StudentService.GetClassStudents(classId, mp.Id);

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
                            Level = ClassroomLevelToLevelCvt(ssa.ClassroomLevel),
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

        private static string ClassroomLevelToLevelCvt(string classroomLevel)
        {
            switch (classroomLevel)
            {
                case StudentClassAttendance.PRESENT:
                    return null;
                case StudentClassAttendance.ABSENT:
                    return "A";
                case StudentClassAttendance.TARDY:
                    return "T";
                default:
                    return "H";
            }
        }

        public AttendanceSummary GetAttendanceSummary(int teacherId, GradingPeriod gradingPeriod)
        {
            var classes = ServiceLocator.ClassService.GetTeacherClasses(gradingPeriod.SchoolYearRef, teacherId, gradingPeriod.MarkingPeriodRef);
            if (classes.Count == 0)
            {
                return new AttendanceSummary
                    {
                        ClassesDaysStat = new List<ClassDailyAttendanceSummary>(),
                        Students = new List<StudentAttendanceSummary>()
                    };
            }

            var classesIds = classes.Select(x => x.Id).ToList();
            var students = ServiceLocator.StudentService.GetTeacherStudents(teacherId, gradingPeriod.SchoolYearRef);
        
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
            var students = ServiceLocator.StudentService.GetClassStudents(classId, markingPeriodId, true);
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
        
        public IList<ClassDetails> GetNotTakenAttendanceClasses(DateTime dateTime)
        {
            Trace.Assert(Context.PersonId.HasValue);
            var syId = Context.SchoolYearId ?? ServiceLocator.SchoolYearService.GetCurrentSchoolYear().Id;
            var classes = ServiceLocator.ClassService.GetTeacherClasses(syId, Context.PersonId.Value).Where(x=>x.StudentsCount > 0).ToList();
            var postedAttendances = ConnectorLocator.AttendanceConnector.GetPostedAttendances(syId, dateTime);
            if (postedAttendances != null && dateTime.Date <= Context.NowSchoolYearTime.Date)
            {
                if (dateTime.Date == Context.NowSchoolYearTime.Date)
                {
                    var time = (int)(Context.NowSchoolYearTime - Context.NowSchoolYearTime.Date).TotalMinutes - 3;
                    postedAttendances = postedAttendances.Where(x => x.StartTime.TotalMinutes <= time).ToList();    
                }
                classes = classes.Where(x => postedAttendances.Any(y => y.SectionId == x.Id && !y.AttendancePosted)).ToList();
            }
            else
                classes = new List<ClassDetails>();
            return classes;
        }
    }
}
