using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.DataAccess;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IClassPeriodService
    {
        ClassPeriod Add(int periodId, int classId, int? roomId, int dateTypeId);
        void Add(IList<ClassPeriod> classPeriods);
        void Delete(int periodId, int classId, int dayTypeId);
        IList<ClassPeriod> GetClassPeriods(int schoolYearId,  int? markingPeriodId, int? classId, int? roomId, int? periodId, int? dateTypeId, int? studentId = null, int? teacherId = null, int? time = null);
        IList<Class> GetAvailableClasses(int periodId);
        IList<Room> GetAvailableRooms(int periodId);


        ClassPeriod GetClassPeriodForSchoolPersonByDate(int personId, DateTime dateTime);
        ClassPeriod GetNearestClassPeriod(int? classId, DateTime dateTime);
        IList<ClassPeriod> GetClassPeriods(DateTime date, int? classId, int? roomId, int? studentId, int? teacherId, int? time = null);
  
    }

    public class ClassPeriodService : SchoolServiceBase, IClassPeriodService
    {
        public ClassPeriodService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }
        
        public ClassPeriod Add(int periodId, int classId, int? roomId, int dateTypeId)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new ClassPeriodDataAccess(uow, Context.SchoolLocalId);
                var res = new ClassPeriod
                {
                    ClassRef = classId,
                    PeriodRef = periodId,
                    DayTypeRef = dateTypeId,
                    SchoolRef = new PeriodDataAccess(uow, Context.SchoolLocalId).GetById(periodId).SchoolRef
                };
                da.Insert(res);
                uow.Commit();
                return res;
            }
        }

        public void Add(IList<ClassPeriod> classPeriods)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new ClassPeriodDataAccess(uow, Context.SchoolLocalId);
                da.Insert(classPeriods);
                uow.Commit();
            }
        }

        public void Delete(int periodId, int classId, int dayTypeId)
        {
            if(!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                new ClassPeriodDataAccess(uow, Context.SchoolLocalId).FullDelete(periodId, classId, dayTypeId);
                uow.Commit();
            }
        }

        public IList<ClassPeriod> GetClassPeriods(int schoolYearId, int? markingPeriodId, int? classId, int? roomId, int? periodId, int? sectionId,
                                     int? studentId = null, int? teacherId = null, int? time = null)
        {
            using (var uow = Read())
            {
                var classIds = new List<int>();
                if(classId.HasValue)
                    classIds.Add(classId.Value);

                return new ClassPeriodDataAccess(uow, Context.SchoolLocalId)
                            .GetClassPeriods(new ClassPeriodQuery
                                {
                                    SchoolYearId = schoolYearId,
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
        }

        public IList<Class> GetAvailableClasses(int periodId)
        {
            using (var uow = Read())
            {
                return new ClassPeriodDataAccess(uow, Context.SchoolLocalId).GetAvailableClasses(periodId);
            }
        }

        public IList<Room> GetAvailableRooms(int periodId)
        {
            using (var uow = Read())
            {
                return new ClassPeriodDataAccess(uow, Context.SchoolLocalId).GetAvailableRooms(periodId);
            }
        }

        public ClassPeriod GetClassPeriodForSchoolPersonByDate(int personId, DateTime dateTime)
        {
            using (var uow = Read())
            {
                var personDa = new PersonDataAccess(uow, Context.SchoolLocalId);
                var person = personDa.GetPerson(new PersonQuery
                    {
                        CallerId = Context.PersonId,
                        CallerRoleId = Context.Role.Id,
                        PersonId = personId
                    });
                var teacherId = person.RoleRef == CoreRoles.TEACHER_ROLE.Id ? person.Id : default(int?);
                var studentId = person.RoleRef == CoreRoles.STUDENT_ROLE.Id ? person.Id : default(int?);
                var time = (int)(dateTime - dateTime.Date).TotalMinutes;
                return GetClassPeriods(dateTime.Date, null, null, studentId, teacherId, time).FirstOrDefault();    
            }
        }

        public ClassPeriod GetNearestClassPeriod(int? classId, DateTime dateTime)
        {
            int? studentId = null, teacherId = null;
            if (!classId.HasValue)
            {
                if (Context.Role == CoreRoles.STUDENT_ROLE)
                    studentId = Context.PersonId;
                if (Context.Role == CoreRoles.TEACHER_ROLE)
                    teacherId = Context.PersonId;
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
