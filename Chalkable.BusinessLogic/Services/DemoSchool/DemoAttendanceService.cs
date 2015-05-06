using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
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
            var ds = date.ToString("yyyy-MM-dd");
            if (AttendanceStorage.GetData().Count(x => x.Value.SectionId == classId && x.Value.Date == ds) == 0)
            {
                GenerateSectionAttendanceForClass(classId, date, date);
            }
            return AttendanceStorage.GetData().First(x => x.Value.SectionId == classId && x.Value.Date == ds).Value;
        }

        public void SetSectionAttendance(DateTime date, int classId, SectionAttendance sa)
        {
            var ds = date.ToString("yyyy-MM-dd");
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
            var classRoomLevels = new[] { "Absent", "Missing", "Tardy", "Present" };
            var random = new Random();
            for (var start = startDate; start <= endDate; start = start.AddDays(1))
            {
                var ds = start.ToString("yyyy-MM-dd");
                var sa = new SectionAttendance()
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
            var sectionAttendanceSummary = new SectionAttendanceSummary();
            sectionAttendanceSummary.SectionId = classId;

            var days = new List<DailySectionAttendanceSummary>();

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
                var day = new DailySectionAttendanceSummary();
                var attendance = GetSectionAttendance(start, classId);

                day.Absences = attendance.StudentAttendance.Count(x => x.ClassroomLevel == "Absent");
                day.Tardies = attendance.StudentAttendance.Count(x => x.ClassroomLevel == "Tardy");
                day.Date = start;
                day.SectionId = classId;
                days.Add(day);

                foreach (var studentId in studentIds)
                {
                    absenceCount[studentId] += attendance.StudentAttendance.Count(x => x.ClassroomLevel == "Absent" && x.StudentId == studentId);
                    tardiesCount[studentId] += attendance.StudentAttendance.Count(x => x.ClassroomLevel == "Tardy" && x.StudentId == studentId);
                }
            }
            sectionAttendanceSummary.Days = days;
            sectionAttendanceSummary.Students = studentIds.Select(x => new StudentSectionAttendanceSummary
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
                throw new NoMarkingPeriodException("No marking period is scheduled for this date");

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
                SetSectionAttendance(date, classId, sa);
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

        public AttendanceSummary GetAttendanceSummary(int teacherId, GradingPeriod gradingPeriod)
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
                return StudentClassAttendance.PRESENT;
            if (StudentClassAttendance.IsLateLevel(level))
                return StudentClassAttendance.TARDY;
            if (level == "A" || level == "AO")
                return StudentClassAttendance.ABSENT;
            return StudentClassAttendance.MISSING;
        }

        public IList<ClassAttendanceDetails> GetClassAttendances(DateTime date, int classId)
        {
            var markingPeriod = ServiceLocator.MarkingPeriodService.GetMarkingPeriodByDate(date, true);
            if (markingPeriod == null)
            {
                throw new ChalkableException("No marking period is scheduled for this date");
            }

            var sa = GetSectionAttendance(date, classId);
            if (sa != null)
            {
                var clazz = ServiceLocator.ClassService.GetClassDetailsById(classId);
                var persons = ServiceLocator.StudentService.GetClassStudents(classId, markingPeriod.Id);
                var attendances = new List<ClassAttendanceDetails>();
                foreach (var ssa in sa.StudentAttendance)
                {
                    var student = persons.FirstOrDefault(x => x.Id == ssa.StudentId);
                    if (student != null)
                    {
                        attendances.Add(new ClassAttendanceDetails
                        {
                            Date = date,
                            Class = clazz,
                            IsPosted = sa.IsPosted
                        });
                    }
                }
                return attendances;    
            }
            return null;
        }

        public IList<ClassDetails> GetNotTakenAttendanceClasses(DateTime dateTime)
        {
            return new List<ClassDetails>();
        }

        public IList<StudentDateAttendance> GetStudentAttendancesByDateRange(int studentId, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public StudentAttendanceSummary GetStudentAttendanceSummary(int studentId, int? markingPeriodId)
        {
            throw new NotImplementedException();
        }

    }
}
