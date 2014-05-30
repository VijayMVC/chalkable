using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.School.DataAccess;
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
            var sy = ServiceLocator.SchoolYearService.GetCurrentSchoolYear();
            Storage.StiAttendanceStorage.SetSectionAttendance(sy.Id, date, classId, sa);
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
            var students = ServiceLocator.PersonService.GetPaginatedPersons(new PersonQuery
            {
                ClassId = classId,
                RoleId = CoreRoles.STUDENT_ROLE.Id
            });
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

        public AttendanceSummary GetAttendanceSummary(int teacherId, int gradingPeriodId)
        {
            var gradingPeriod = ServiceLocator.GradingPeriodService.GetGradingPeriodById(gradingPeriodId);
            var classes = ServiceLocator.ClassService.GetClasses(gradingPeriod.SchoolYearRef, gradingPeriod.MarkingPeriodRef, teacherId, 0);
            //var classTeachers = ServiceLocator.ClassService.GetClassTeachers(null, teacherId);
            //var classesIds = classes.Where(x => classTeachers.Any(y => y.ClassRef == x.Id)).Select(x => x.Id).ToList();
            var classesIds = classes.Select(x => x.Id).ToList();
            var students = ServiceLocator.PersonService.GetPaginatedPersons(new PersonQuery
            {
                RoleId = CoreRoles.STUDENT_ROLE.Id,
                TeacherId = teacherId
            });
            var sectionsAttendanceSummary = Storage.StiAttendanceStorage.GetSectionAttendanceSummary(classesIds, gradingPeriod.StartDate, gradingPeriod.EndDate);
            var res = new AttendanceSummary();
            var dailySectionAttendances = new List<DailySectionAttendanceSummary>();
            var studentAtts = new List<StudentSectionAttendanceSummary>();
            foreach (var sectionAttendanceSummary in sectionsAttendanceSummary)
            {
                dailySectionAttendances.AddRange(sectionAttendanceSummary.Days);
                studentAtts.AddRange(sectionAttendanceSummary.Students);
            }
            res.DaysStat = DailyAttendanceSummary.Create(dailySectionAttendances);
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
            var sa = Storage.StiAttendanceStorage.GetSectionAttendance(date, classId);
            if (sa != null)
            {
                var clazz = ServiceLocator.ClassService.GetClassById(classId);
                var persons = ServiceLocator.ClassService.GetStudents(classId);
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
