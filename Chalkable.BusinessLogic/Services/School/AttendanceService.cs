using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAttendanceService
    {
        ClassAttendance SetClassAttendance(Guid classPersonId, Guid classPeriodId, DateTime date, AttendanceTypeEnum type, Guid? attendanceReasonId = null, int? sisId = null);
        IList<ClassAttendance> SetAttendanceForClass(Guid classPeriodId, DateTime date, AttendanceTypeEnum type, Guid? attendanceReasonId = null, int? sisId = null);
        StudentDailyAttendance SetDailyAttendance(DateTime date, Guid personId,  int? timeIn, int? timeOut);
        StudentDailyAttendance GetDailyAttendance(DateTime date, Guid personId);
        IList<StudentDailyAttendance> GetDailyAttendances(DateTime date); 
        IList<ClassAttendanceDetails> GetClassAttendanceDetails(ClassAttendanceQuery attendanceQuery, IList<Guid> gradeLevelIds = null);
        ClassAttendanceDetails GetClassAttendanceDetails(Guid studentId, Guid classPeriodId, DateTime date);
        ClassAttendanceDetails GetClassAttendanceDetailsById(Guid classAttendanceId);
        IList<ClassAttendanceDetails> GetClassAttendanceDetails(Guid? schoolYearId, Guid? markingPeriodId, Guid? classId, Guid? personId, AttendanceTypeEnum? type, DateTime date);
        ClassAttendanceDetails SwipeCard(Guid personId, DateTime dateTime, Guid classPeriodId);


        int PossibleAttendanceCount(Guid markingPeriodId, Guid classId, DateTime? tillDate);

        IDictionary<AttendanceTypeEnum, int> CalcAttendanceTotalPerTypeForStudent(Guid studentId, Guid? schoolYearId, Guid? markingPeriodId, DateTime? fromDate, DateTime? toDate);
        IList<PersonAttendanceTotalPerType> CalcAttendanceTotalPerTypeForStudents(IList<Guid> studentsIds, Guid? schoolYearId, Guid? markingPeriodId, DateTime? fromDate, DateTime? toDate);
        IDictionary<Guid, int> CalcAttendanceTotalForStudents(IList<Guid> studentsIds, Guid? schoolYearId, Guid? markingPeriodId, DateTime? fromDate, DateTime? toDate, AttendanceTypeEnum type); 
        
        IDictionary<DateTime, int> GetStudentCountAbsentFromDay(DateTime fromDate, DateTime toDate, IList<Guid> gradeLevelIds);
        IList<Guid> GetStudentsAbsentFromDay(DateTime date, IList<Guid> gradeLevelsIds); 
        IList<StudentCountAbsentFromPeriod> GetStudentCountAbsentFromPeriod(DateTime fromDate, DateTime toDate, IList<Guid> gradeLevelsIds, int fromPeriodOrder, int toPeriodOrder);
        IList<StudentAbsentFromPeriod> GetStudentsAbsentFromPeriod(DateTime date, IList<Guid> gradeLevelsIds, int periodOrder);

        IDictionary<int, IList<AttendanceTotalPerType>> CalcAttendanceTotalPerPeriod(DateTime fromDate, DateTime toDate, int fromPeriodOrder, int toPeriodOrder, AttendanceTypeEnum type, IList<Guid> gradeLevelsIds);
        IDictionary<DateTime, IList<AttendanceTotalPerType>> CalcAttendanceTotalPerDate(DateTime fromDate, DateTime toDate, AttendanceTypeEnum type, IList<Guid> gradeLevelsIds);
    }

    public class AttendanceService : SchoolServiceBase, IAttendanceService
    {
        public AttendanceService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }


        public ClassAttendance SetClassAttendance(Guid classPersonId, Guid classPeriodId, DateTime date, AttendanceTypeEnum type, Guid? attendanceReasonId = null, int? sisId = null)
        {
            IList<ClassPerson> classPersons = new List<ClassPerson>();
            using (var uow = Read())
            {
                classPersons.Add(new ClassPersonDataAccess(uow).GetById(classPersonId));
            }
           return SetAttendanceForStudents(classPersons, classPeriodId, date, type, attendanceReasonId, sisId).First();
        }

        public IList<ClassAttendance> SetAttendanceForClass(Guid classPeriodId, DateTime date, AttendanceTypeEnum type, Guid? attendanceReasonId = null, int? sisId = null)
        {
            IList<ClassPerson> classPersons;
            using (var uow = Read())
            {
                var cp = new ClassPeriodDataAccess(uow).GetById(classPeriodId);
                classPersons = new ClassPersonDataAccess(uow).GetClassPersons(new ClassPersonQuery {ClassId = cp.ClassRef});
            }
            return SetAttendanceForStudents(classPersons, classPeriodId, date, type, attendanceReasonId, sisId);
        }

        private IList<ClassAttendance> SetAttendanceForStudents(IList<ClassPerson> classPersons, Guid classPeriodId, DateTime date, AttendanceTypeEnum type, Guid? attendanceReasonId = null, int? sisId = null)
        {
            using (var uow = Update())
            {
                var cPeriodDa = new ClassPeriodDataAccess(uow);
                var cPeriod = cPeriodDa.GetClassPeriods(new ClassPeriodQuery {Id = classPeriodId}).First();
                var c = new ClassDataAccess(uow).GetById(cPeriod.ClassRef);
                if (!BaseSecurity.IsAdminEditorOrClassTeacher(c, Context))
                    throw new ChalkableSecurityException();

                if (attendanceReasonId.HasValue)
                {
                    var attendanceReason = ServiceLocator.AttendanceReasonService.Get(attendanceReasonId.Value);
                    if (attendanceReason.AttendanceType != type)
                        throw new ChalkableException(ChlkResources.ERR_NO_ATTENDANCE_REASONS_FOR_CURRENT_TYPE);
                }
                var dateDa = new DateDataAccess(uow);
                if (!dateDa.Exists(new DateQuery{ToDate = date, FromDate = date, SectionRef = cPeriod.Period.SectionRef}))
                    throw new ChalkableException(ChlkResources.ERR_CLASS_IS_NOT_SCHEDULED_FOR_DAY);
                if (classPersons.Any(x => x.ClassRef != cPeriod.ClassRef))
                    throw new ChalkableException(ChlkResources.ERR_USER_IS_NOT_FROM_THIS_CLASS);

                var attDa = new ClassAttendanceDataAccess(uow);
                var res = attDa.SetAttendance(new ClassAttendance
                    {
                        AttendanceReasonRef = attendanceReasonId,
                        ClassPeriodRef = classPeriodId,
                        Date = date,
                        LastModified = Context.NowSchoolTime,
                        Type = type,
                        SisId = sisId
                    }, classPersons.Select(x=>x.Id).ToList());
                uow.Commit();
                return res;
            }      
        }

        public StudentDailyAttendance SetDailyAttendance(DateTime date, Guid personId, int? timeIn, int? timeOut)
        {
            if(!AttendanceSecurity.CanSetDailyAttendance(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new StudentDailyAttendanceDataAccess(uow);
                var res = da.SetStudentDailyAttendance(personId, date, timeIn, timeOut);
                uow.Commit();
                return res;
            }
        }

        public StudentDailyAttendance GetDailyAttendance(DateTime date, Guid personId)
        {
            if (!(BaseSecurity.IsAdminViewer(Context) || Context.UserId == personId))
                throw new ChalkableSecurityException();
               
            using (var uow = Read())
            {
                var dailyAttDa = new StudentDailyAttendanceDataAccess(uow);
                var res = dailyAttDa.GetDailyAttendanceOrNull(new StudentDailyAttendanceQuery
                    {
                        Date = date,
                        PersonRef = personId
                    });
                if (res == null || res.Arrival == null)
                {
                    var firstPresentOrLate = GetClassAttendanceDetails(new ClassAttendanceQuery
                        {
                            StudentId = personId,
                            FromDate = date,
                            ToDate = date,
                            Type = AttendanceTypeEnum.Late | AttendanceTypeEnum.Present
                        }).OrderBy(x=>x.ClassPeriod.Period.StartTime).FirstOrDefault();

                    if (firstPresentOrLate != null)
                    {
                        var generalPeriod = firstPresentOrLate.ClassPeriod.Period;
                        var lastModified = (int)(firstPresentOrLate.LastModified - firstPresentOrLate.LastModified.Date).TotalMinutes;
                        var arrivaleTime = generalPeriod.StartTime;
                        if (firstPresentOrLate.Type == AttendanceTypeEnum.Late &&
                            generalPeriod.StartTime <= lastModified && lastModified <= generalPeriod.EndTime)
                        {
                            arrivaleTime = lastModified;
                        }
                        if (res == null)
                        {
                            var person = ServiceLocator.PersonService.GetPerson(personId);
                            res = new StudentDailyAttendanceDetails
                            {
                                TimeIn = arrivaleTime,
                                PersonRef = personId,
                                Date = date,
                                Person = person
                            };
                        }
                        res.Arrival = arrivaleTime;
                    }
                }
                return res;
            }
        }

        public IList<StudentDailyAttendance> GetDailyAttendances(DateTime date)
        {
            throw new NotImplementedException();
        }

        public IList<ClassAttendanceDetails> GetClassAttendanceDetails(ClassAttendanceQuery attendanceQuery, IList<Guid> gradeLevelIds = null)
        {
            using (var uow = Read())
            {
                var res = new ClassAttendanceDataAccess(uow).GetAttendance(attendanceQuery);
                if (gradeLevelIds != null && gradeLevelIds.Count > 0)
                    res = res.Where(x => gradeLevelIds.Contains(x.Class.GradeLevelRef)).ToList();
                return res;
            }
        }

        public IList<ClassAttendanceDetails> GetClassAttendanceDetails(Guid? schoolYearId, Guid? markingPeriodId, Guid? classId, Guid? personId, AttendanceTypeEnum? type, DateTime date)
        {
            return GetClassAttendanceDetails(new ClassAttendanceQuery
                {
                    SchoolYearId = schoolYearId,
                    MarkingPeriodId = markingPeriodId,
                    ClassId = classId,
                    StudentId = personId,
                    Type = type,
                    FromDate = date,
                    ToDate = date
                });
        }

        public ClassAttendanceDetails SwipeCard(Guid personId, DateTime dateTime, Guid classPeriodId)
        {
            using (var uow = Update())
            {
                var cpDa = new ClassPeriodDataAccess(uow);
                var classPeriod = cpDa.GetClassPeriods(new ClassPeriodQuery {Id = classPeriodId}).First();
                var c = ServiceLocator.ClassService.GetClassById(classPeriod.Id);
                if (!BaseSecurity.IsAdminEditorOrClassTeacher(c, Context))
                    throw new ChalkableSecurityException();
                var have = (int)(dateTime - dateTime.Date).TotalMinutes;
                var must = classPeriod.Period.StartTime;
                var classPerson = new ClassPersonDataAccess(uow).GetClassPerson(new ClassPersonQuery { ClassId = c.Id, PersonId = personId});
                var res = SetClassAttendance(classPerson.Id, classPeriodId, dateTime, have <= must ? AttendanceTypeEnum.Present : AttendanceTypeEnum.Late);
                uow.Commit();
                return GetClassAttendanceDetails(new ClassAttendanceQuery { Id = res.Id, MarkingPeriodId = classPeriod.Period.MarkingPeriodRef }).First();
            }
        }

        //TODO: needs test
        public ClassAttendanceDetails GetClassAttendanceDetailsById(Guid classAttendanceId)
        {
           return GetClassAttendanceDetails(new ClassAttendanceQuery
                {
                    Id = classAttendanceId,
                }).First();
        }

        //TODO: needs security ... needs test
        public int PossibleAttendanceCount(Guid markingPeriodId, Guid classId, DateTime? tillDate)
        {
            using (var uow = Read())
            {
                return new ClassAttendanceDataAccess(uow).PossibleAttendanceCount(markingPeriodId, classId, tillDate);
            }
        }
        //TODO: needs test
        private IEnumerable<PersonAttendanceTotalPerType> CalcAttendanceTotalPerType(Guid? schoolYearId, Guid? markingPeriodId, Guid? studentId, DateTime? fromDate, DateTime? toDate)
        {
            using (var uow = Read())
            {
               return new ClassAttendanceDataAccess(uow).CalcAttendanceTypeTotal(markingPeriodId,
                    schoolYearId, studentId, fromDate, toDate);
            }
        }

        public IDictionary<AttendanceTypeEnum, int> CalcAttendanceTotalPerTypeForStudent(Guid studentId, Guid? schoolYearId, Guid? markingPeriodId, DateTime? fromDate, DateTime? toDate)
        {
            var res = CalcAttendanceTotalPerType(schoolYearId, markingPeriodId, studentId, fromDate, toDate);
            return res.GroupBy(x => x.AttendanceType).ToDictionary(x => x.Key, x => x.Sum(y => y.Total));
        }

        public IList<PersonAttendanceTotalPerType> CalcAttendanceTotalPerTypeForStudents(IList<Guid> studentsIds, Guid? schoolYearId, Guid? markingPeriodId, DateTime? fromDate, DateTime? toDate)
        {
            var res = CalcAttendanceTotalPerType(schoolYearId, markingPeriodId, null, fromDate, toDate);
            return res.Where(x => studentsIds.Contains(x.PersonId)).ToList();
        }


        public IDictionary<Guid, int> CalcAttendanceTotalForStudents(IList<Guid> studentsIds, Guid? schoolYearId, Guid? markingPeriodId, DateTime? fromDate, DateTime? toDate, AttendanceTypeEnum type)
        {
            var res = CalcAttendanceTotalPerTypeForStudents(studentsIds, schoolYearId, markingPeriodId, fromDate, toDate);
            res = res.Where(x => (x.AttendanceType & type) != 0).ToList(); //TODO move it to stored procedure
            return res.GroupBy(x => x.PersonId).ToDictionary(x => x.Key, x => x.Sum(y => y.Total));
        }

        public ClassAttendanceDetails GetClassAttendanceDetails(Guid studentId, Guid classPeriodId, DateTime date)
        {
            return GetClassAttendanceDetails(new ClassAttendanceQuery
                {
                    ClassPeriodId = classPeriodId,
                    StudentId = studentId,
                    FromDate = date,
                    ToDate = date
                }).FirstOrDefault();
        }


        public IDictionary<DateTime, int> GetStudentCountAbsentFromDay(DateTime fromDate, DateTime toDate, IList<Guid> gradeLevelIds)
        {
            using (var uow = Read())
            {
                return new  ClassAttendanceDataAccess(uow).GetStudentCountAbsentFromDay(fromDate, toDate, gradeLevelIds);
            }
        }

        public IList<Guid> GetStudentsAbsentFromDay(DateTime date, IList<Guid> gradeLevelsIds) 
       {
            using (var uow = Read())
            {
                var res = new ClassAttendanceDataAccess(uow).GetStudentAbsentFromDay(date, date, gradeLevelsIds);
                return res.ContainsKey(date) ? res[date] : new List<Guid>();
            }
        }

        public IList<StudentCountAbsentFromPeriod> GetStudentCountAbsentFromPeriod(DateTime fromDate, DateTime toDate, IList<Guid> gradeLevelsIds, int fromPeriodOrder, int toPeriodOrder)
        {
            using (var uow = Read())
            {
                return new ClassAttendanceDataAccess(uow).GetStudentCountAbsentFromPeriod(fromDate, toDate, fromPeriodOrder, toPeriodOrder, gradeLevelsIds);
            }
        }

        public IList<StudentAbsentFromPeriod> GetStudentsAbsentFromPeriod(DateTime date, IList<Guid> gradeLevelsIds, int periodOrder)
        {
            using (var uow = Read())
            {
                return new ClassAttendanceDataAccess(uow).GetStudentAbsentFromPeriod(date, gradeLevelsIds, periodOrder);
            }
        }

        public IDictionary<int, IList<AttendanceTotalPerType>> CalcAttendanceTotalPerPeriod(DateTime fromDate, DateTime toDate,
            int fromPeriodOrder, int toPeriodOrder, AttendanceTypeEnum type, IList<Guid> gradeLevelsIds)
        {
            using (var uow = Read())
            {
                var res = new ClassAttendanceDataAccess(uow).CalcAttendanceTotalPerPeriod(fromDate, toDate, fromPeriodOrder, toPeriodOrder, type, gradeLevelsIds);
                return res;
            }
        }

        public IDictionary<DateTime, IList<AttendanceTotalPerType>> CalcAttendanceTotalPerDate(DateTime fromDate, DateTime toDate
            , AttendanceTypeEnum type, IList<Guid> gradeLevelsIds)
        {
            using (var uow = Read())
            {
                return new ClassAttendanceDataAccess(uow).CalcAttendanceTotalPerDate(fromDate, toDate, type, gradeLevelsIds);
            }
        }
    }
}
