using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.DataAccess;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IClassPeriodService
    {
        ClassPeriod Add(Guid periodId, Guid classId, Guid roomId);
        void Delete(Guid id);
        IList<ClassPeriod> GetClassPeriods(Guid markingPeriodId, Guid? classId, Guid? roomId, Guid? periodId, Guid? sectionId, Guid? studentId = null, Guid? teacherId = null, int? time = null);
        IList<Class> GetAvailableClasses(Guid periodId);
        IList<Room> GetAvailableRooms(Guid periodId);

        ClassPeriod GetClassPeriodForSchoolPersonByDate(Guid personId, DateTime dateTime);
        ClassPeriod GetNearestClassPeriod(Guid? classId, DateTime dateTime);
        IList<ClassPeriod> GetClassPeriods(DateTime date, Guid? classId, Guid? roomId, Guid? studentId, Guid? teacherId, int? time = null);
  
    }

    public class ClassPeriodService : SchoolServiceBase, IClassPeriodService
    {
        public ClassPeriodService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }


        public ClassPeriod Add(Guid periodId, Guid classId, Guid roomId)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new ClassPeriodDataAccess(uow);
                if (da.Exists( new ClassPeriodQuery{ PeriodId = periodId, RoomId = roomId}))
                    throw new ChalkableException(ChlkResources.ERR_OTHER_CLASS_ALREADY_ASSIGNED_TO_ROOM);
                if (da.Exists(new ClassPeriodQuery { ClassIds = new List<Guid> {classId}, PeriodId = periodId }))
                    throw new ChalkableException(ChlkResources.ERR_CLASS_ALREADY_ASSIGNED_TO_PERIOD);
                if (da.IsClassStudentsAssignedToPeriod(periodId, classId))
                    throw new ChalkableException(ChlkResources.ERR_STUDENT_BAD_CLASS_PERIOD);

                var res = new ClassPeriod
                {
                    Id = Guid.NewGuid(),
                    ClassRef = classId,
                    PeriodRef = periodId,
                    RoomRef = roomId
                };
                da.Insert(res);
                uow.Commit();
                return res;
            }
        }

        public void Delete(Guid id)
        {
            if(BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                new ClassPeriodDataAccess(uow).Delete(id);
                uow.Commit();
            }
        }

        public IList<ClassPeriod> GetClassPeriods(Guid markingPeriodId, Guid? classId, Guid? roomId, Guid? periodId, Guid? sectionId,
                                     Guid? studentId = null, Guid? teacherId = null, int? time = null)
        {
            using (var uow = Read())
            {
                var classIds = new List<Guid>();
                if(classId.HasValue)
                    classIds.Add(classId.Value);

                return new ClassPeriodDataAccess(uow)
                            .GetClassPeriods(new ClassPeriodQuery
                                {
                                    MarkingPeriodId = markingPeriodId,
                                    ClassIds = classIds,
                                    PeriodId = periodId,
                                    RoomId = roomId,
                                    SectionId = sectionId,
                                    StudentId = studentId,
                                    TeacherId = teacherId,
                                    Time = time
                                });
            }
        }

        public IList<Class> GetAvailableClasses(Guid periodId)
        {
            using (var uow = Read())
            {
                return new ClassPeriodDataAccess(uow).GetAvailableClasses(periodId);
            }
        }

        public IList<Room> GetAvailableRooms(Guid periodId)
        {
            using (var uow = Read())
            {
                return new ClassPeriodDataAccess(uow).GetAvailableRooms(periodId);
            }
        }

        public ClassPeriod GetClassPeriodForSchoolPersonByDate(Guid personId, DateTime dateTime)
        {
            using (var uow = Read())
            {
                var personDa = new PersonDataAccess(uow);
                var person = personDa.GetById(personId);
                var teacherId = person.RoleRef == CoreRoles.TEACHER_ROLE.Id ? person.Id : default(Guid?);
                var studentId = person.RoleRef == CoreRoles.STUDENT_ROLE.Id ? person.Id : default(Guid?);
                var time = (int)(dateTime - dateTime.Date).TotalMinutes;
                return GetClassPeriods(dateTime, null, null, studentId, teacherId, time).FirstOrDefault();    
            }
        }

        public ClassPeriod GetNearestClassPeriod(Guid? classId, DateTime dateTime)
        {
            Guid? studentId = null, teacherId = null;
            if (!classId.HasValue)
            {
                if (Context.Role == CoreRoles.STUDENT_ROLE)
                    studentId = Context.UserId;
                if (Context.Role == CoreRoles.TEACHER_ROLE)
                    teacherId = Context.UserId;
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

        public IList<ClassPeriod> GetClassPeriods(DateTime date, Guid? classId, Guid? roomId, Guid? studentId, Guid? teacherId, int? time = null)
        {
            var d = ServiceLocator.CalendarDateService.GetCalendarDateByDate(date);
            if (d == null || !d.ScheduleSectionRef.HasValue || !d.MarkingPeriodRef.HasValue)
                return new List<ClassPeriod>();

            return GetClassPeriods(d.MarkingPeriodRef.Value, classId, roomId, null, d.ScheduleSectionRef, studentId, teacherId, time);
        }
    }
}
