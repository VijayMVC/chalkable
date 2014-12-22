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
        Class CurrentClassForTeacher(int personId, DateTime dateTime);
        IList<ScheduleItem> GetSchedule(int? teacherId, int? studentId, int? classId, DateTime from, DateTime to);
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

        public IList<ScheduleItem> GetSchedule(int? teacherId, int? studentId, int? classId, DateTime @from, DateTime to)
        {
            Trace.Assert(Context.SchoolYearId.HasValue);
            return DoRead( u => new ScheduleDataAccess(u, null).GetSchedule(Context.SchoolYearId.Value, teacherId, studentId, classId, from, to));

        }
    }
}
