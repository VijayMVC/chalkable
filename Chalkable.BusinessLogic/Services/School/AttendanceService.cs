using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Model.Attendances;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;
using Chalkable.StiConnector.Connectors.Model.Attendances;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAttendanceService
    {
        ClassAttendanceDetails GetClassAttendance(DateTime date, int classId);
        void SetClassAttendances(DateTime date, int classId, IList<StudentClassAttendance> studentAttendances);
        SeatingChartInfo GetSeatingChart(int classId, int markingPeriodId);
        void UpdateSeatingChart(int classId, int markingPeriodId, SeatingChartInfo seatingChart);
        Task<AttendanceSummary> GetAttendanceSummary(int teacherId, GradingPeriod gradingPeriod);
        Task<IList<ClassDetails>> GetNotTakenAttendanceClasses(DateTime date);
        IList<StudentDateAttendance> GetStudentAttendancesByDateRange(int studentId, DateTime startDate, DateTime endDate);
        StudentAttendanceSummary GetStudentAttendanceSummary(int studentId, int? gradingPeriodId);
        ClassAttendanceSummary GetClassAttendanceSummary(int classId, int? gradingPeriodId);
        IList<ClassPeriodAttendance> GetClassPeriodAttendances(int classId, DateTime start, DateTime end);
        Task<IList<DailyAttendanceSummary>> GetDailyAttendanceSummaries(int classId, DateTime? startDate, DateTime? endDate);
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
                    ClassroomLevel = LevelToClassRoomLevel(sca.Level, sca.IsDailyAttendancePeriod),
                    ReasonId = (short) (sca.AttendanceReasonId ?? 0),
                    SectionId = classId,
                    StudentId = sca.StudentId,
                }).ToList();
            if (!Context.SchoolYearId.HasValue)
                throw new ChalkableException(ChlkResources.ERR_CANT_DETERMINE_SCHOOL_YEAR);
            ConnectorLocator.AttendanceConnector.SetSectionAttendance(Context.SchoolYearId.Value, date, classId, sa);
        }

        public ClassAttendanceDetails GetClassAttendance(DateTime date, int classId)
        {
            var mp = ServiceLocator.MarkingPeriodService.GetLastClassMarkingPeriod(classId, date);
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

        private string LevelToClassRoomLevel(string level, bool? isDailyAttendancePeriod)
        {
            if (level == null)
                return BaseAttendance.PRESENT;
            if (BaseAttendance.IsLateLevel(level))
                return BaseAttendance.TARDY;
            if ((level == "A" || level == "AO") && isDailyAttendancePeriod.HasValue && isDailyAttendancePeriod.Value)
                return BaseAttendance.ABSENT;
            return BaseAttendance.MISSING;
        }

        private static string ClassroomLevelToLevelCvt(string classroomLevel)
        {
            switch (classroomLevel)
            {
                case BaseAttendance.PRESENT:
                    return null;
                case BaseAttendance.ABSENT:
                    return "A";
                case BaseAttendance.TARDY:
                    return "T";
                default:
                    return "H";
            }
        }

        public async Task<AttendanceSummary> GetAttendanceSummary(int teacherId, GradingPeriod gradingPeriod)
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
            var attSummarytask =  ConnectorLocator.AttendanceConnector.GetSectionAttendanceSummary(classesIds, gradingPeriod.StartDate, gradingPeriod.EndDate);
            var students = ServiceLocator.StudentService.GetTeacherStudents(teacherId, gradingPeriod.SchoolYearRef);
            var sectionsAttendanceSummary = await attSummarytask;
            var res = new AttendanceSummary();
            var dailySectionAttendances = new List<DailySectionAbsenceSummary>();
            var studentAtts = new List<StudentSectionAbsenceSummary>();
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
            studentAtts = studentAtts.Where(x => classesIds.Contains(x.SectionId)).ToList();
            res.ClassesDaysStat = ClassDailyAttendanceSummary.Create(dailySectionAttendances, classes);
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
        
        public async Task<IList<ClassDetails>> GetNotTakenAttendanceClasses(DateTime dateTime)
        {
            Trace.Assert(Context.PersonId.HasValue);
            var syId = Context.SchoolYearId ?? ServiceLocator.SchoolYearService.GetCurrentSchoolYear().Id;
            var postedAttendanceTask = ConnectorLocator.AttendanceConnector.GetPostedAttendances(syId, dateTime);
            var classes = ServiceLocator.ClassService.GetTeacherClasses(syId, Context.PersonId.Value).Where(x=>x.StudentsCount > 0).ToList();
            var postedAttendances = await postedAttendanceTask;
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
    

        public IList<StudentDateAttendance> GetStudentAttendancesByDateRange(int studentId,  DateTime startDate, DateTime endDate)
        {
            var sy = ServiceLocator.SchoolYearService.GetCurrentSchoolYear();
            var student = ServiceLocator.StudentService.GetById(studentId, sy.Id);
            var periods = ServiceLocator.PeriodService.GetPeriods(sy.Id);
            var classes = ServiceLocator.ClassService.GetStudentClasses(sy.Id, studentId);
            var stiAttendanceDetails = ConnectorLocator.StudentConnector.GetStudentAttendanceDetailDashboard(studentId, sy.Id, startDate, endDate);
            var res = new List<StudentDateAttendance>();
            var currentDate = startDate;
            while (currentDate <= endDate)
            {
                var dailyAtt = stiAttendanceDetails.DailyAbsences.FirstOrDefault(x => x.Date.Date == currentDate.Date && x.StudentId == studentId);
                var periodAttendances = stiAttendanceDetails.PeriodAbsences.Where(x => x.Date.Date == currentDate.Date && x.StudentId == studentId).ToList();
                var checkIncheckOuts = stiAttendanceDetails.CheckInCheckOuts.Where(x => x.Date == currentDate.Date && x.StudentId == studentId).ToList();
                var item = new StudentDateAttendance
                    {
                        Date = currentDate.Date,
                        Student = student
                    };
                if (dailyAtt != null)
                {
                   item.DailyAttendance = new StudentDailyAttendance
                       {
                           StudentId = dailyAtt.StudentId,
                           AttendanceReasonId = dailyAtt.AbsenceReasonId,
                           Category = dailyAtt.AbsenceCategory,
                           Date = dailyAtt.Date,
                           Level = dailyAtt.AbsenceLevel,
                       };
                }

                item.CheckInCheckOuts = checkIncheckOuts.Select(checkIncheckOut => new CheckInCheckOut
                {
                    AttendanceReasonId = checkIncheckOut.AbsenceReasonId,
                    Category = checkIncheckOut.AbsenceCategory,
                    Note = checkIncheckOut.Note,
                    Time = checkIncheckOut.Time,
                    PeriodId = checkIncheckOut.TimeSlotId,
                    IsCheckIn = checkIncheckOut.Action == "I"
                }).ToList();

                item.StudentPeriodAttendances = periodAttendances.Select(x => new StudentPeriodAttendance
                    {
                        Student = student,
                        AttendanceReasonId = x.AbsenceReasonId,
                        Category = x.AbsenceCategory,
                        ClassId = x.SectionId,
                        Date = x.Date,
                        Level = x.AbsenceLevel,
                        StudentId = studentId,
                        Period = periods.FirstOrDefault(period => period.Id == x.TimeSlotId),
                        Class = classes.FirstOrDefault(c=>c.Id == x.SectionId)
                    }).ToList();
                res.Add(item);
                currentDate = currentDate.AddDays(1);
            }
            return res;
        }
        
        public StudentAttendanceSummary GetStudentAttendanceSummary(int studentId, int? gradingPeriodId)
        {
            var syId = ServiceLocator.SchoolYearService.GetCurrentSchoolYear().Id;
            var student = ServiceLocator.StudentService.GetById(studentId, syId);
            var stiModel = ConnectorLocator.StudentConnector.GetStudentAttendanceSummary(studentId, syId, gradingPeriodId);

            int? mpId = null;
            if (gradingPeriodId.HasValue)
            {
                var gp = ServiceLocator.GradingPeriodService.GetGradingPeriodById(gradingPeriodId.Value);
                mpId = gp.MarkingPeriodRef;
            }
            var classes = ServiceLocator.ClassService.GetStudentClasses(syId, studentId, mpId);
            var res = new StudentAttendanceSummary {Student = student};
            if (stiModel != null)
            {
                if (stiModel.DailyAttendance != null)
                    res.DailyAttendanceSummary = StudentDailyAttendanceSummary.Create(stiModel.DailyAttendance);
                if (stiModel.PeriodAttendance != null)
                    res.ClassAttendanceSummaries = StudentClassAttendanceSummary.Create(stiModel.PeriodAttendance.ToList(), classes);
            }
            return res;
        }

        public ClassAttendanceSummary GetClassAttendanceSummary(int classId, int? gradingPeriodId)
        {
            var sectionAbcense = ConnectorLocator.SectionDashboardConnector.GetAttendanceSummaryDashboard(classId, gradingPeriodId);
            return ClassAttendanceSummary.Create(sectionAbcense.PeriodAttendance);
        }


        public IList<ClassPeriodAttendance> GetClassPeriodAttendances(int classId, DateTime start, DateTime end)
        {
            var stiAttendances = ConnectorLocator.SectionDashboardConnector.GetAttendanceDetailDashboard(classId, start, end);
            if (stiAttendances != null)
            {
                var syId = ServiceLocator.SchoolYearService.GetCurrentSchoolYear().Id;
                var mps = ServiceLocator.MarkingPeriodService.GetMarkingPeriodsByDateRange(start, end, syId);
                if (mps.Count == 0)
                    return null;
                
                var c = ServiceLocator.ClassService.GetById(classId);
                var periods = ServiceLocator.PeriodService.GetPeriods(syId);
                var students = new List<Student>();
                students = mps.Select(mp => ServiceLocator.StudentService.GetClassStudents(classId, mp.Id))
                              .Aggregate(students, (current, items) => current.Union(items).ToList());

                var res = new List<ClassPeriodAttendance>();
                var currentDate = start;
                while (currentDate <= end)
                {
                    var date = currentDate.Date;
                    var periodAtts = stiAttendances.PeriodAbsences.Where(x => x.Date == date).ToList();
                    var item = new ClassPeriodAttendance
                        {
                            Class = c,
                            ClassId = c.Id,
                            Date = currentDate,
                            StudentAttendances = periodAtts.Select(x=> new StudentPeriodAttendance
                                {
                                    Class = c,
                                    Student = students.FirstOrDefault(s=>s.Id == x.StudentId),
                                    AttendanceReasonId = x.AbsenceReasonId,
                                    Category = x.AbsenceCategory,
                                    ClassId = x.SectionId,
                                    Date = x.Date,
                                    Level = x.AbsenceLevel,
                                    StudentId = x.StudentId,
                                    Period = periods.FirstOrDefault(period => period.Id == x.TimeSlotId)
                                }).Where(x=>x.Student != null).ToList()
                        };
                    res.Add(item);
                    currentDate = currentDate.AddDays(1);
                }
                return res;
            }
            return null;
        }

        public async Task<IList<DailyAttendanceSummary>> GetDailyAttendanceSummaries(int classId, DateTime? startDate, DateTime? endDate)
        {
            var inowRes = await ConnectorLocator.SectionDashboardConnector.GetAttendanceDailySummaries(classId, startDate, endDate);
            return inowRes.Select(DailyAttendanceSummary.Create).ToList();
        }

        public static decimal CalculatePresencePercent(decimal absenceCount, int studentsCount)
        {
            return (1 - absenceCount / studentsCount) * 100;
        }
    }
}
