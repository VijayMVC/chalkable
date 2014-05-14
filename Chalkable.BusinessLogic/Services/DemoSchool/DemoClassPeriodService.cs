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
        
        public ClassPeriod Add(int periodId, int classId, int? roomId, int dateTypeId)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            var res = new ClassPeriod
            {
                ClassRef = classId,
                PeriodRef = periodId,
                DayTypeRef = dateTypeId,
                SchoolRef = Storage.PeriodStorage.GetById(periodId).SchoolRef
            };
            Storage.ClassPeriodStorage.Add(res);
            return res;
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

        public IList<Class> GetAvailableClasses(int periodId)
        {
            return Storage.ClassPeriodStorage.GetAvailableClasses(periodId);
        }

        public IList<Room> GetAvailableRooms(int periodId)
        {
            return Storage.ClassPeriodStorage.GetAvailableRooms(periodId);
        }

        public ClassPeriod GetClassPeriodForSchoolPersonByDate(int personId, DateTime dateTime)
        {
            var person = Storage.PersonStorage.GetPerson(new PersonQuery
            {
                CallerId = Context.UserLocalId,
                CallerRoleId = Context.Role.Id,
                PersonId = personId
            });
            var teacherId = person.RoleRef == CoreRoles.TEACHER_ROLE.Id ? person.Id : default(int?);
            var studentId = person.RoleRef == CoreRoles.STUDENT_ROLE.Id ? person.Id : default(int?);
            var time = (int)(dateTime - dateTime.Date).TotalMinutes;
            return GetClassPeriods(dateTime.Date, null, null, studentId, teacherId, time).FirstOrDefault();  
        }

        public ClassPeriod GetNearestClassPeriod(int? classId, DateTime dateTime)
        {
            int? studentId = null, teacherId = null;
            if (!classId.HasValue)
            {
                if (Context.Role == CoreRoles.STUDENT_ROLE)
                    studentId = Context.UserLocalId;
                if (Context.Role == CoreRoles.TEACHER_ROLE)
                    teacherId = Context.UserLocalId;
            }
            var classPeriods = GetClassPeriods(dateTime, classId, null, studentId, teacherId);

            var nearest = int.MaxValue;
            var time = (int)((dateTime - dateTime.Date).TotalMinutes);
            ClassPeriod result = null;
            foreach (var classPeriod in classPeriods)
            {
                if (classPeriod.Period.EndTime > time && classPeriod.Period.EndTime < nearest)
                {
                    nearest = classPeriod.Period.EndTime;
                    result = classPeriod;
                }
            }
            return result;
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
