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
  
        StudentDailyAttendance SetDailyAttendance(DateTime date, Guid personId, string attendanceNote, AttendanceTypeEnum type, int? timeIn, int? timeOut);
        StudentDailyAttendance GetDailyAttendance(DateTime date, Guid personId);
        IList<StudentDailyAttendance> GetDailyAttendances(DateTime date);
        IList<ClassAttendanceComplex> GetClassAttendanceComplex(ClassAttendanceQuery attendanceQuery, IList<Guid> gradeLevelIds);

        IList<ClassAttendanceComplex> GetClassAttendanceComplex(Guid? schoolYearId, Guid? markingPeriodId, Guid? classId, Guid? personId, AttendanceTypeEnum? type, DateTime date);
        ClassAttendanceComplex SwipeCard(Guid personId, DateTime dateTime, Guid classPeriodId);
       
   
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

        public StudentDailyAttendance SetDailyAttendance(DateTime date, Guid personId, string attendanceNote, AttendanceTypeEnum type, int? timeIn, int? timeOut)
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

        public IList<ClassAttendanceComplex> GetClassAttendanceComplex(ClassAttendanceQuery attendanceQuery, IList<Guid> gradeLevelIds)
        {
            using (var uow = Read())
            {
                var res = new ClassAttendanceDataAccess(uow).GetAttendance(attendanceQuery);
                if (gradeLevelIds != null && gradeLevelIds.Count > 0)
                    res = res.Where(x => gradeLevelIds.Contains(x.Class.GradeLevelRef)).ToList();
                return res;
            }
        }

        public IList<ClassAttendanceComplex> GetClassAttendanceComplex(Guid? schoolYearId, Guid? markingPeriodId, Guid? classId, Guid? personId, AttendanceTypeEnum? type, DateTime date)
        {
            return GetClassAttendanceComplex(new ClassAttendanceQuery
                {
                    SchoolYearId = schoolYearId,
                    MarkingPeriodId = markingPeriodId,
                    ClassId = classId,
                    StudentId = personId,
                    Type = type,
                    FromDate = date,
                    ToDate = date
                }, null);
        }

        public ClassAttendanceComplex SwipeCard(Guid personId, DateTime dateTime, Guid classPeriodId)
        {
            throw new NotImplementedException();
        }
    }
}
