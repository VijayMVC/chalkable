using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAttendanceService
    {
        ClassAttendance SetClassAttendance(Guid classPersonId, Guid classPeriodId, DateTime date, AttendanceTypeEnum type, Guid? attendanceReasonId = null);
        IList<ClassAttendance> SetAttendanceForClass(Guid classPeriodId, DateTime date, AttendanceTypeEnum type, Guid? attendanceReasonId = null);
  
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


        public ClassAttendance SetClassAttendance(Guid classPersonId, Guid classPeriodId, DateTime date, AttendanceTypeEnum type, Guid? attendanceReasonId = null)
        {
            throw new NotImplementedException();
        }

        public IList<ClassAttendance> SetAttendanceForClass(Guid classPeriodId, DateTime date, AttendanceTypeEnum type, Guid? attendanceReasonId = null)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public IList<ClassAttendanceComplex> GetClassAttendanceComplex(Guid? schoolYearId, Guid? markingPeriodId, Guid? classId, Guid? personId, AttendanceTypeEnum? type, DateTime date)
        {
            throw new NotImplementedException();
        }

        public ClassAttendanceComplex SwipeCard(Guid personId, DateTime dateTime, Guid classPeriodId)
        {
            throw new NotImplementedException();
        }
    }
}
