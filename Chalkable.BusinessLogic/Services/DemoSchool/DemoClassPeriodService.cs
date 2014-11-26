using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
  
    public class DemoClassPeriodService : DemoSchoolServiceBase, IClassPeriodService
    {
        public DemoClassPeriodService(IServiceLocatorSchool serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
        }
        
        public void Add(IList<ClassPeriod> classPeriods)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            Storage.ClassPeriodStorage.Add(classPeriods);
        }

        public void Delete(int periodId, int classId, int dayTypeId)
        {
            if(!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            Storage.ClassPeriodStorage.FullDelete(periodId, classId, dayTypeId);
        }

        public IList<ClassPeriod> GetClassPeriods(int schoolYearId, int? markingPeriodId, int? classId, int? roomId, int? periodId, int? sectionId,
                                     int? studentId = null, int? teacherId = null, int? time = null)
        {
            var classIds = new List<int>();
            if (classId.HasValue)
                classIds.Add(classId.Value);

            return Storage.ClassPeriodStorage.GetClassPeriods(new ClassPeriodQuery
            {
                MarkingPeriodId = markingPeriodId,
                ClassIds = classIds,
                PeriodId = periodId,
                RoomId = roomId,
                DateTypeId = sectionId,
                StudentId = studentId,
                TeacherId = teacherId,
                Time = time
            });
        }

        public Class CurrentClassForTeacher(int personId, DateTime dateTime)
        {
            var person = Storage.PersonStorage.GetPerson(new PersonQuery
            {
                CallerId = Context.PersonId,
                CallerRoleId = Context.Role.Id,
                PersonId = personId
            });
            var teacherId = person.RoleRef == CoreRoles.TEACHER_ROLE.Id ? person.Id : default(int?);
            var studentId = person.RoleRef == CoreRoles.STUDENT_ROLE.Id ? person.Id : default(int?);
            var time = (int)(dateTime - dateTime.Date).TotalMinutes;
            throw new NotImplementedException();
        }

        public IList<ScheduleItem> GetSchedule(int? teacherId, int? studentId, int? classId, DateTime @from, DateTime to)
        {
            var syId = ServiceLocator.SchoolYearService.GetCurrentSchoolYear().Id;
            return Storage.ClassPeriodStorage.GetSchedule(syId, teacherId, studentId, classId, from, to);
        }

        public IList<ClassPeriod> GetClassPeriods(DateTime date, int? classId, int? roomId, int? studentId, int? teacherId, int? time = null)
        {

            var d = ServiceLocator.CalendarDateService.GetCalendarDateByDate(date.Date);
            if (d == null || !d.DayTypeRef.HasValue)
                return new List<ClassPeriod>();

            return GetClassPeriods(d.SchoolYearRef, null, classId, roomId, null, d.DayTypeRef, studentId, teacherId, time);
        }
    }
}
