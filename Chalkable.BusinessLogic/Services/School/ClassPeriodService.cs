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
        ClassPeriod Add(int id, int periodId, int classId, int? roomId, int dateTypeId);
        void Delete(int id);
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


        public ClassPeriod Add(int id, int periodId, int classId, int? roomId, int dateTypeId)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new ClassPeriodDataAccess(uow, Context.SchoolLocalId);
                if (da.Exists( new ClassPeriodQuery{ PeriodId = periodId, RoomId = roomId, DateTypeId = dateTypeId}))
                    throw new ChalkableException(ChlkResources.ERR_OTHER_CLASS_ALREADY_ASSIGNED_TO_ROOM);
                if (da.Exists(new ClassPeriodQuery { ClassIds = new List<int> {classId}, PeriodId = periodId, DateTypeId = dateTypeId}))
                    throw new ChalkableException(ChlkResources.ERR_CLASS_ALREADY_ASSIGNED_TO_PERIOD);
                if (da.IsClassStudentsAssignedToPeriod(periodId, classId, dateTypeId))
                    throw new ChalkableException(ChlkResources.ERR_STUDENT_BAD_CLASS_PERIOD);

                var res = new ClassPeriod
                {
                    Id = id,
                    ClassRef = classId,
                    PeriodRef = periodId,
                    RoomRef = roomId,
                    DayTypeRef = dateTypeId
                };
                da.Insert(res);
                uow.Commit();
                return res;
            }
        }

        public void Delete(int id)
        {
            if(!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                new ClassPeriodDataAccess(uow, Context.SchoolLocalId).FullDelete(id);
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
                        CallerId = Context.UserLocalId,
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
