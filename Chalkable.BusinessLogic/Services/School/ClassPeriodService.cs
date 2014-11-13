using System;
using System.Collections.Generic;
using System.Diagnostics;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.DataAccess;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IClassPeriodService
    {
        void Add(IList<ClassPeriod> classPeriods);
        void Delete(int periodId, int classId, int dayTypeId);
        IList<ClassPeriod> GetClassPeriods(int schoolYearId,  int? markingPeriodId, int? classId, int? roomId, int? periodId, int? dateTypeId, int? studentId = null, int? teacherId = null, int? time = null);

        Class CurrentClassForTeacher(int personId, DateTime dateTime);
        IList<ClassPeriod> GetClassPeriods(DateTime date, int? classId, int? roomId, int? studentId, int? teacherId, int? time = null);
    }

    public class ClassPeriodService : SchoolServiceBase, IClassPeriodService
    {
        public ClassPeriodService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }
        
        public void Add(IList<ClassPeriod> classPeriods)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            DoUpdate(u => new ClassPeriodDataAccess(u, Context.SchoolLocalId).Insert(classPeriods));
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

        public Class CurrentClassForTeacher(int personId, DateTime dateTime)
        {
            Trace.Assert(Context.SchoolYearId.HasValue);
            var date = dateTime.Date;
            var time = (int) (dateTime - date).TotalMinutes;
            using (var uow = Read())
            {
                return new ClassPeriodDataAccess(uow, null).CurrentClassForTeacher(Context.SchoolYearId.Value, personId,date, time);
            }  
        }

        public IList<ClassPeriod> GetClassPeriods(DateTime date, int? classId, int? roomId, int? studentId, int? teacherId, int? time = null)
        {
            var d = ServiceLocator.CalendarDateService.GetCalendarDateByDate(date.Date);
            var mp = ServiceLocator.MarkingPeriodService.GetLastMarkingPeriod(date.Date);
            if (mp == null || d == null || !d.DayTypeRef.HasValue)
                return new List<ClassPeriod>();            

            if (d.SchoolYearRef != mp.SchoolYearRef)
                throw new Exception("d.SchoolYearRef != mp.SchoolYearRef");

            return GetClassPeriods(mp.SchoolYearRef, mp.Id, classId, roomId, null, d.DayTypeRef, studentId, teacherId, time);
        }
    }
}
