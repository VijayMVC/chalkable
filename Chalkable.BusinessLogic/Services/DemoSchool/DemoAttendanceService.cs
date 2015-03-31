using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage.sti;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoStiSeatingChartStorage : BaseDemoIntStorage<KeyValuePair<int, SeatingChart>>
    {
        public DemoStiSeatingChartStorage()
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

    public class DemoStiAttendanceStorage : BaseDemoIntStorage<SectionAttendance>
    {
        public DemoStiAttendanceStorage()
            : base(null, true)
        {
        }

        public SectionAttendance GetSectionAttendance(DateTime date, int classId)
        {
            var ds = date.ToString("yyyy-MM-dd");
            if (data.Count(x => x.Value.SectionId == classId && x.Value.Date == ds) == 0)
            {
                GenerateSectionAttendanceForClass(classId, date, date);
            }
            return data.First(x => x.Value.SectionId == classId && x.Value.Date == ds).Value;
        }

        public void SetSectionAttendance(DateTime date, int classId, SectionAttendance sa)
        {
            var ds = date.ToString("yyyy-MM-dd");
            if (data.Count(x => x.Value.SectionId == classId && x.Value.Date == ds) == 0)
            {
                data.Add(GetNextFreeId(), sa);
            }
            var item = data.First(x => x.Value.SectionId == classId && x.Value.Date == ds).Key;
            sa.IsPosted = true;
            data[item] = sa;
        }


        public void GenerateSectionAttendanceForClass(int classId, DateTime startDate, DateTime endDate)
        {
            var classRoomLevels = new string[] { "Absent", "Missing", "Tardy", "Present" };
            var random = new Random();
            for (var start = startDate; start <= endDate; start = start.AddDays(1))
            {
                var ds = start.ToString("yyyy-MM-dd");
                var sa = new SectionAttendance()
                {
                    SectionId = classId,
                    Date = ds,
                    StudentAttendance = StorageLocator.ClassPersonStorage.GetClassPersons(new ClassPersonQuery()
                    {
                        ClassId = classId
                    }).Select(x => x.PersonRef).Distinct().Select(x => new StudentSectionAttendance()
                    {
                        Date = ds,
                        SectionId = classId,
                        StudentId = x,
                        ClassroomLevel = classRoomLevels[random.Next(0, 4)]
                    }).ToList()
                };
                data.Add(GetNextFreeId(), sa);
            }
        }


        private SectionAttendanceSummary GetSaSummary(int classId, DateTime startDate, DateTime endDate)
        {
            var sectionAttendanceSummary = new SectionAttendanceSummary();
            sectionAttendanceSummary.SectionId = classId;

            var days = new List<DailySectionAttendanceSummary>();

            var absenceCount = new Dictionary<int, int>();
            var tardiesCount = new Dictionary<int, int>();

            var studentIds = StorageLocator.PersonStorage.GetPersons(new PersonQuery()
            {
                TeacherId = Context.PersonId
            }).Persons.Select(x => x.Id);

            foreach (var studentId in studentIds)
            {
                absenceCount.Add(studentId, 0);
                tardiesCount.Add(studentId, 0);
            }

            for (var start = startDate; start < endDate; start = start.AddDays(1))
            {
                var day = new DailySectionAttendanceSummary();
                var attendance = StorageLocator.StiAttendanceStorage.GetSectionAttendance(start, classId);

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
            var classesIds = StorageLocator.ClassPersonStorage.GetClassPersons(new ClassPersonQuery
            {
                PersonId = studentId
            }).Select(x => x.ClassRef).Distinct().ToList();

            return classesIds.Select(classId => GetSaSummary(classId, startDate, endDate)).ToList();
        }
    }

    public class DemoStiActivityStorage : BaseDemoIntStorage<Activity>
    {
        public DemoStiActivityStorage()
            : base(x => x.Id, true)
        {
        }

        public Activity CreateActivity(int classId, Activity activity)
        {
            activity.SectionId = classId;
            activity.Id = GetNextFreeId();
            data.Add(activity.Id, activity);
            return activity;
        }

        public void CopyActivity(int sisActivityId, IList<int> classIds)
        {
            var activity = GetById(sisActivityId);

            var classIdsFiltered = classIds.Where(x => x != activity.SectionId).ToList();

            foreach (var classId in classIdsFiltered)
            {
                activity.Id = GetNextFreeId();
                activity.SectionId = classId;

                if (activity.Attachments == null)
                    activity.Attachments = new List<ActivityAttachment>();

                foreach (var attachment in activity.Attachments)
                {
                    attachment.ActivityId = activity.Id;
                }
                data.Add(activity.Id, activity);
            }
        }

        public IEnumerable<Activity> GetAll(int sectionId)
        {
            return data.Where(x => x.Value.SectionId == sectionId).Select(x => x.Value).ToList();
        }
    }


    public class DemoAttendanceService : DemoSchoolServiceBase, IAttendanceService
    {
        private DemoStiSeatingChartStorage SeatingChartStorage { get; set; }
        private DemoStiAttendanceStorage AttendanceStorage { get; set; }
        public DemoAttendanceService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            SeatingChartStorage = new DemoStiSeatingChartStorage();
            AttendanceStorage = new DemoStiAttendanceStorage();
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
            AttendanceStorage.SetSectionAttendance(date, classId, sa);
        }

        public SeatingChartInfo GetSeatingChart(int classId, int markingPeriodId)
        {
            var seatingChart = SeatingChartStorage.GetChart(classId, markingPeriodId);
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
            SeatingChartStorage.UpdateChart(classId, markingPeriodId, seatingChart);
        }

        public AttendanceSummary GetAttendanceSummary(int teacherId, GradingPeriod gradingPeriod)
        {
            Trace.Assert(Context.SchoolYearId.HasValue);
            var classes = ServiceLocator.ClassService.GetTeacherClasses(gradingPeriod.SchoolYearRef, teacherId, gradingPeriod.MarkingPeriodRef);
            var classesIds = classes.Select(x => x.Id).ToList();
            var students = ServiceLocator.StudentService.GetTeacherStudents(teacherId, Context.SchoolYearId.Value);
            var sectionsAttendanceSummary = AttendanceStorage.GetSectionAttendanceSummary(classesIds, gradingPeriod.StartDate, gradingPeriod.EndDate);
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

        private static string LevelToClassRoomLevel(string level)
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

            var sa = AttendanceStorage.GetSectionAttendance(date, classId);
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

        public IList<ClassDetails> GetNotTakenAttendanceClasses(DateTime dateTime)
        {
            return new List<ClassDetails>();
        }

        public IList<ClassAttendance> SetAttendanceForClass(Guid classPeriodId, DateTime date, string level, Guid? attendanceReasonId = null, int? sisId = null)
        {
            throw new NotImplementedException();
        }

        public StudentDailyAttendance SetDailyAttendance(DateTime date, Guid personId, int? timeIn, int? timeOut)
        {
            throw new NotImplementedException();
        }

        public StudentDailyAttendance GetDailyAttendance(DateTime date, Guid personId)
        {
            throw new NotImplementedException();
        }

        public IList<StudentDailyAttendance> GetDailyAttendances(DateTime date)
        {
            throw new NotImplementedException();
        }

        public ClassAttendanceDetails GetClassAttendanceDetailsById(Guid classAttendanceId)
        {
            throw new NotImplementedException();
        }

        public IList<ClassAttendanceDetails> GetClassAttendanceDetails(Guid? schoolYearId, Guid? markingPeriodId, Guid? classId, Guid? personId, string level, DateTime date)
        {
            throw new NotImplementedException();
        }

        public ClassAttendanceDetails SwipeCard(Guid personId, DateTime dateTime, Guid classPeriodId)
        {
            throw new NotImplementedException();
        }

        public int PossibleAttendanceCount(Guid markingPeriodId, Guid classId, DateTime? tillDate)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, int> CalcAttendanceTotalPerTypeForStudent(Guid studentId, Guid? schoolYearId, Guid? markingPeriodId, DateTime? fromDate, DateTime? toDate)
        {
            throw new NotImplementedException();
        }

        public IList<PersonAttendanceTotalPerType> CalcAttendanceTotalPerTypeForStudents(IList<Guid> studentsIds, Guid? schoolYearId, Guid? markingPeriodId, DateTime? fromDate, DateTime? toDate)
        {
            throw new NotImplementedException();
        }

        public IDictionary<Guid, int> CalcAttendanceTotalForStudents(IList<Guid> studentsIds, Guid? schoolYearId, Guid? markingPeriodId, DateTime? fromDate, DateTime? toDate, string level)
        {
            throw new NotImplementedException();
        }

        public IDictionary<DateTime, int> GetStudentCountAbsentFromDay(DateTime fromDate, DateTime toDate, IList<Guid> gradeLevelIds)
        {
            throw new NotImplementedException();
        }

        public IList<Guid> GetStudentsAbsentFromDay(DateTime date, IList<Guid> gradeLevelsIds)
        {
            throw new NotImplementedException();
        }

        public IList<StudentCountAbsentFromPeriod> GetStudentCountAbsentFromPeriod(DateTime fromDate, DateTime toDate, IList<Guid> gradeLevelsIds, int fromPeriodOrder, int toPeriodOrder)
        {
            throw new NotImplementedException();
        }

        public IList<StudentAbsentFromPeriod> GetStudentsAbsentFromPeriod(DateTime date, IList<Guid> gradeLevelsIds, int periodOrder)
        {
            throw new NotImplementedException();
        }

        public IDictionary<int, IList<AttendanceTotalPerType>> CalcAttendanceTotalPerPeriod(DateTime fromDate, DateTime toDate, int fromPeriodOrder, int toPeriodOrder, string level, IList<Guid> gradeLevelsIds)
        {
            throw new NotImplementedException();
        }

        public IDictionary<DateTime, IList<AttendanceTotalPerType>> CalcAttendanceTotalPerDate(DateTime fromDate, DateTime toDate, string level, IList<Guid> gradeLevelsIds)
        {
            throw new NotImplementedException();
        }

        public void ProcessClassAttendance(DateTime date)
        {
            throw new NotImplementedException();
        }

        public void NotAssignedAttendanceProcess()
        {
            throw new NotImplementedException();
        }
    }
}
