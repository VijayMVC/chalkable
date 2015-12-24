using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Model.Attendances;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;
using Chalkable.StiConnector.Connectors.Model.Attendances;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoSeatingChartStorage : BaseDemoIntStorage<KeyValuePair<int, SeatingChart>>
    {
        public DemoSeatingChartStorage()
            : base(null, true)
        {

        }

        public SeatingChart GetChart(int classId, int markingPeriodId)
        {
            return data.First(x => x.Value.Key == markingPeriodId && x.Value.Value.SectionId == classId).Value.Value;
        }

        public void UpdateChart(int classId, int markingPeriodId, SeatingChart seatingChart)
        {
            if (data.Count(x => x.Value.Key == markingPeriodId && x.Value.Value.SectionId == classId) == 0)
            {
                Add(new KeyValuePair<int, SeatingChart>(markingPeriodId, seatingChart));
            }
            var item = data.First(x => x.Value.Value.SectionId == classId && x.Value.Key == markingPeriodId).Key;
            data[item] = new KeyValuePair<int, SeatingChart>(markingPeriodId, seatingChart);
        }
    }

    public class DemoAttendanceStorage : BaseDemoIntStorage<SectionAttendance>
    {
        public DemoAttendanceStorage()
            : base(null, true)
        {
        }

        
    }

    public class DemoAttendanceService : DemoSchoolServiceBase, IAttendanceService
    {
        private DemoSeatingChartStorage SeatingChartStorage { get; set; }
        private DemoAttendanceStorage AttendanceStorage { get; set; }
        public DemoAttendanceService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            SeatingChartStorage = new DemoSeatingChartStorage();
            AttendanceStorage = new DemoAttendanceStorage();
        }

        public SectionAttendance GetSectionAttendance(DateTime date, int classId)
        {
            var ds = date.ToString(Constants.DATE_FORMAT);
            if (AttendanceStorage.GetData().Count(x => x.Value.SectionId == classId && x.Value.Date == ds) == 0)
            {
                GenerateSectionAttendanceForClass(classId, date, date);
            }
            return AttendanceStorage.GetData().First(x => x.Value.SectionId == classId && x.Value.Date == ds).Value;
        }

        public void SetSectionAttendance(DateTime date, int classId, SectionAttendance sa)
        {
            var ds = date.ToString(Constants.DATE_FORMAT);
            if (AttendanceStorage.GetData().Count(x => x.Value.SectionId == classId && x.Value.Date == ds) == 0)
            {
                AttendanceStorage.Add(sa);
            }
            var item = AttendanceStorage.GetData().First(x => x.Value.SectionId == classId && x.Value.Date == ds).Key;
            sa.IsPosted = true;
            AttendanceStorage.GetData()[item] = sa;
        }


        public void GenerateSectionAttendanceForClass(int classId, DateTime startDate, DateTime endDate)
        {
            var classRoomLevels = new[] {BaseAttendance.ABSENT, BaseAttendance.MISSING, BaseAttendance.TARDY, BaseAttendance.PRESENT};
            var random = new Random();
            for (var start = startDate; start <= endDate; start = start.AddDays(1))
            {
                var ds = start.ToString(Constants.DATE_FORMAT);
                var sa = new SectionAttendance
                {
                    SectionId = classId,
                    Date = ds,
                    StudentAttendance = ((DemoClassService)ServiceLocator.ClassService).GetClassPersons(classId)
                    .Select(x => x.PersonRef).Distinct().Select(x => new StudentSectionAttendance()
                    {
                        Date = ds,
                        SectionId = classId,
                        StudentId = x,
                        ClassroomLevel = classRoomLevels[random.Next(0, 4)]
                    }).ToList()
                };
                AttendanceStorage.Add(sa);
            }
        }
        
        private SectionAttendanceSummary GetSaSummary(int classId, DateTime startDate, DateTime endDate)
        {
            var sectionAttendanceSummary = new SectionAttendanceSummary {SectionId = classId};

            var days = new List<DailySectionAbsenceSummary>();

            var absenceCount = new Dictionary<int, int>();
            var tardiesCount = new Dictionary<int, int>();

            var studentIds = ((DemoPersonService)ServiceLocator.PersonService).GetTeacherStudents(Context.PersonId.Value).Select(x => x.Id);

            foreach (var studentId in studentIds)
            {
                absenceCount.Add(studentId, 0);
                tardiesCount.Add(studentId, 0);
            }

            for (var start = startDate; start < endDate; start = start.AddDays(1))
            {
                var day = new DailySectionAbsenceSummary();
                var attendance = GetSectionAttendance(start, classId);

                day.Absences = attendance.StudentAttendance.Count(x => x.ClassroomLevel == BaseAttendance.ABSENT);
                day.Tardies = attendance.StudentAttendance.Count(x => x.ClassroomLevel == BaseAttendance.TARDY);
                day.Date = start;
                day.SectionId = classId;
                days.Add(day);

                foreach (var studentId in studentIds)
                {
                    absenceCount[studentId] += attendance.StudentAttendance.Count(x => x.ClassroomLevel == BaseAttendance.ABSENT && x.StudentId == studentId);
                    tardiesCount[studentId] += attendance.StudentAttendance.Count(x => x.ClassroomLevel == BaseAttendance.TARDY && x.StudentId == studentId);
                }
            }
            sectionAttendanceSummary.Days = days;
            sectionAttendanceSummary.Students = studentIds.Select(x => new StudentSectionAbsenceSummary
            {
                SectionId = classId,
                StudentId = x,
                Absences = absenceCount[x],
                Tardies = tardiesCount[x]
            });

            return sectionAttendanceSummary;
        }

        public IList<SectionAttendanceSummary> GetSectionAttendanceSummary(List<int> classesIds, DateTime startDate, DateTime endDate)
        {
            return classesIds.Select(classId => GetSaSummary(classId, startDate, endDate)).ToList();
        }


        public IList<SectionAttendanceSummary> GetSectionAttendanceSummary(int studentId, DateTime startDate, DateTime endDate)
        {
            var classesIds = ((DemoClassService) ServiceLocator.ClassService)
                .GetStudentClasses(Context.SchoolYearId.Value, studentId).Select(x => x.Id).Distinct().ToList();
            return classesIds.Select(classId => GetSaSummary(classId, startDate, endDate)).ToList();
        }

        public IList<SectionAbsenceSummary> GetStudentAbsenceSummary(int studentId)
        {
            return GetSectionAttendanceSummary(studentId, DateTime.Today.AddDays(-1), DateTime.Today)
                .Select(sectionAttendanceSummary => new SectionAbsenceSummary
                {
                    SectionId = sectionAttendanceSummary.SectionId,
                    Tardies = sectionAttendanceSummary.Students.Select(x => x.Tardies).Sum(),
                    Absences = sectionAttendanceSummary.Students.Select(x => x.Absences).Sum(),

                }).ToList();
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
            SetSectionAttendance(date, classId, sa);
        }

        public ClassAttendanceDetails GetClassAttendance(DateTime date, int classId)
        {
            var markingPeriod = ServiceLocator.MarkingPeriodService.GetMarkingPeriodByDate(date, true);
            if (markingPeriod == null)
                throw new NoMarkingPeriodException();

            var sa = GetSectionAttendance(date, classId);
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
                //SetSectionAttendance(date, classId, sa);
                return res;
            }
            return null;
        }

        public SeatingChartInfo GetSeatingChart(int classId, int markingPeriodId)
        {
            var seatingChart = SeatingChartStorage.GetChart(classId, markingPeriodId);
            return SeatingChartInfo.Create(seatingChart);
        }

        
        public void AddSeatingChart(int mpId, int classId, SeatingChart chart)
        {
            chart.SectionId = classId;
            SeatingChartStorage.Add(new KeyValuePair<int, SeatingChart>(mpId, chart));
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
            SeatingChartStorage.UpdateChart(classId, markingPeriodId, seatingChart);
        }

        public async Task<AttendanceSummary> GetAttendanceSummary(int teacherId, GradingPeriod gradingPeriod)
        {
            Trace.Assert(Context.SchoolYearId.HasValue);
            var classes = ServiceLocator.ClassService.GetTeacherClasses(gradingPeriod.SchoolYearRef, teacherId, gradingPeriod.MarkingPeriodRef);
            var classesIds = classes.Select(x => x.Id).ToList();
            var students = ServiceLocator.StudentService.GetTeacherStudents(teacherId, Context.SchoolYearId.Value);
            var sectionsAttendanceSummary = GetSectionAttendanceSummary(classesIds, gradingPeriod.StartDate, gradingPeriod.EndDate);
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
            res.Students = StudentAttendanceSummary.Create(studentAtts, students, classes);
            return res;
        }

        private static string LevelToClassRoomLevel(string level)
        {
            if (level == null)
                return BaseAttendance.PRESENT;
            if (BaseAttendance.IsLateLevel(level))
                return BaseAttendance.TARDY;
            if (level == "A" || level == "AO")
                return BaseAttendance.ABSENT;
            return BaseAttendance.MISSING;
        }
        
        public async Task<IList<ClassDetails>> GetNotTakenAttendanceClasses(DateTime dateTime)
        {
            return new List<ClassDetails>();
        }

        public IList<StudentDateAttendance> GetStudentAttendancesByDateRange(int studentId, DateTime startDate, DateTime endDate)
        {
            var syId = ServiceLocator.SchoolYearService.GetCurrentSchoolYear().Id;
            var student = ServiceLocator.StudentService.GetById(studentId, syId);
            var classes = ServiceLocator.ClassService.GetStudentClasses(syId, studentId);
            var res = new List<StudentDateAttendance>();
            var currentData = startDate.Date;
            var schedulesItems = ServiceLocator.ClassPeriodService.GetSchedule(null, studentId, null, startDate, endDate);
            var periods = ServiceLocator.PeriodService.GetPeriods(syId);
            while (currentData < endDate)
            {
                var item = new StudentDateAttendance
                    {
                        Date = currentData,
                        Student = student,
                        StudentPeriodAttendances = new List<StudentPeriodAttendance>()
                    };
                foreach (var clazz in classes)
                {
                    var classId = clazz.Id;
                    var classAttendnace = ServiceLocator.AttendanceService.GetClassAttendance(currentData, classId);
                    var currentSchedules = schedulesItems.Where(x => x.ClassId == clazz.Id && x.Day == currentData).ToList();
                    var period = periods.FirstOrDefault(p => currentSchedules.Any(y => y.PeriodId == p.Id));
                    var studentClassAttendance = classAttendnace.StudentAttendances.FirstOrDefault(x => x.StudentId == studentId);
                    if (studentClassAttendance != null && period != null)
                    {
                        item.StudentPeriodAttendances.Add(new StudentPeriodAttendance
                        {
                            Level = studentClassAttendance.Level,
                            AbsentPreviousDay = studentClassAttendance.AbsentPreviousDay,
                            AttendanceReasonId = studentClassAttendance.AttendanceReasonId,
                            Category = studentClassAttendance.Category,
                            Class = clazz,
                            ClassId = studentClassAttendance.ClassId,
                            Student = student,
                            Date = currentData,
                            StudentId = studentId,
                            Period = period
                        });
                    }
                }
                currentData = currentData.AddDays(1);
                res.Add(item);
            }
            return res;
        }

        public StudentAttendanceSummary GetStudentAttendanceSummary(int studentId, int? gradingPeriodId)
        {
            DateTime startDate, endDate;
            ((DemoGradingPeriodService)ServiceLocator.GradingPeriodService).GetDateRangeByGpID(gradingPeriodId, out startDate, out endDate);
            var syId = ServiceLocator.SchoolYearService.GetCurrentSchoolYear().Id;
            var student = ServiceLocator.StudentService.GetById(studentId, syId);      
            var classes = ServiceLocator.ClassService.GetStudentClasses(syId, studentId);
            var sectionSummaries = GetSectionAttendanceSummary(studentId, startDate, endDate);
            var res = new StudentAttendanceSummary
                {
                    Student = student,
                    ClassAttendanceSummaries = new List<StudentClassAttendanceSummary>()
                };
            foreach (var classDetailse in classes)
            {
                var sectionSummay = sectionSummaries.FirstOrDefault(x => x.SectionId == classDetailse.Id);
                if (sectionSummay != null)
                {
                    var studentSectionSummary = sectionSummay.Students.FirstOrDefault(s => s.StudentId == studentId && s.SectionId == classDetailse.Id);
                    if(studentSectionSummary == null) continue;
                    res.ClassAttendanceSummaries.Add(StudentClassAttendanceSummary.Create(studentSectionSummary, classDetailse));
                }
            }
            return res;
        }

        public ClassAttendanceSummary GetClassAttendanceSummary(int classId, int? gradingPeriodId)
        {
            DateTime startDate, endDate;
            ((DemoGradingPeriodService)ServiceLocator.GradingPeriodService).GetDateRangeByGpID(gradingPeriodId, out startDate, out endDate);
            var sectionSummary = GetSaSummary(classId, startDate, endDate);
            return new ClassAttendanceSummary
                {
                    ClassId = classId,
                    Tardies = sectionSummary.Students.Sum(x=>x.Tardies),
                    Absences = sectionSummary.Students.Sum(x=>x.Absences),
                    Presents = sectionSummary.Students.Sum(x => x.Presents)
                };
        }

        public IList<ClassPeriodAttendance> GetClassPeriodAttendances(int classId, DateTime start, DateTime end)
        {
            var syId = ServiceLocator.SchoolYearService.GetCurrentSchoolYear().Id;
            var currentDate = start.Date;
            var res = new List<ClassPeriodAttendance>();
            var cClass = ServiceLocator.ClassService.GetById(classId);
            var periods = ServiceLocator.PeriodService.GetPeriods(syId);
            var schedules = ServiceLocator.ClassPeriodService.GetSchedule(null, null, classId, start, end);
            while (currentDate < end)
            {
                var classAttendance = GetClassAttendance(currentDate.Date, classId);
                var period = periods.FirstOrDefault(p => schedules.Any(y => y.Day == currentDate && p.Id == y.PeriodId));
                if(period != null)
                    res.Add(new ClassPeriodAttendance
                        {
                            Class = cClass,
                            ClassId = classId,
                            Date = currentDate,
                            StudentAttendances = classAttendance.StudentAttendances.Select(x => CreateStudentPeriodAttendnace(x, period, cClass)).ToList()
                        });
                currentDate = currentDate.AddDays(1);
            }
            return res;
        }

        public Task<IList<DailyAttendanceSummary>> GetDailyAttendanceSummaries(int classId, DateTime? startDate, DateTime? endDate)
        {
            throw new NotImplementedException();
        }

        private StudentPeriodAttendance CreateStudentPeriodAttendnace(StudentClassAttendance studentClassAttendance, Period period, Class cClass)
        {
            return new StudentPeriodAttendance
                {
                    Level = studentClassAttendance.Level,
                    AbsentPreviousDay = studentClassAttendance.AbsentPreviousDay,
                    AttendanceReasonId = studentClassAttendance.AttendanceReasonId,
                    Category = studentClassAttendance.Category,
                    Class = cClass,
                    ClassId = studentClassAttendance.ClassId,
                    Student = studentClassAttendance.Student,
                    Date = studentClassAttendance.Date,
                    StudentId = studentClassAttendance.StudentId,
                    Period = period
                };
        }
    }
}
