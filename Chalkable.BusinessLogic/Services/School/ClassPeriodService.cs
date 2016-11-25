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
        void Delete(IList<ClassPeriod> classPeriods);
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
            if (!BaseSecurity.IsDistrictAdmin(Context))
                throw new ChalkableSecurityException();
            DoUpdate(u => new ClassPeriodDataAccess(u).Insert(classPeriods));
        }

        public void Delete(IList<ClassPeriod> classPeriods)
        {
            if(!BaseSecurity.IsDistrictAdmin(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                new ClassPeriodDataAccess(uow).Delete(classPeriods);
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
                return new ClassPeriodDataAccess(uow).CurrentClassForTeacher(Context.SchoolYearId.Value, personId,date, time);
            }  
        }

        public IList<ScheduleItem> GetSchedule(int? teacherId, int? studentId, int? classId, DateTime @from, DateTime to)
        {
            Trace.Assert(Context.SchoolYearId.HasValue);
            return DoRead( u => new ScheduleDataAccess(u).GetSchedule(Context.SchoolYearId.Value, teacherId, studentId, classId, Context.PersonId, from, to));

        }
    }
}
