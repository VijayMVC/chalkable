using System;
using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAttendanceMonthService
    {
        void Add(IList<AttendanceMonth> attendanceMonths);
        void Edit(IList<AttendanceMonth> attendanceMonths);
        void Delete(IList<AttendanceMonth> attendanceMonths);
        IList<AttendanceMonth> GetAttendanceMonths(int schoolYearId, DateTime? fromDate = null, DateTime? endDate = null);
    }

    public class AttendanceMonthService : SchoolServiceBase, IAttendanceMonthService
    {
        public AttendanceMonthService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void Add(IList<AttendanceMonth> attendanceMonths)
        {
            DoUpdate(u => new DataAccessBase<AttendanceMonth>(u).Insert(attendanceMonths));
        }

        public void Edit(IList<AttendanceMonth> attendanceMonths)
        {
            DoUpdate(u => new DataAccessBase<AttendanceMonth>(u).Update(attendanceMonths));
        }

        public void Delete(IList<AttendanceMonth> attendanceMonths)
        {
            DoUpdate(u => new DataAccessBase<AttendanceMonth>(u).Delete(attendanceMonths));
        }

        public IList<AttendanceMonth> GetAttendanceMonths(int schoolYearId, DateTime? fromDate = null, DateTime? endDate = null)
        {
            var conds = new AndQueryCondition {{AttendanceMonth.SCHOOL_YEAR_REF_FIELD, schoolYearId}};
            if (fromDate.HasValue)
                conds.Add(AttendanceMonth.END_DATE_FIELD, fromDate.Value, ConditionRelation.GreaterEqual);
            if (endDate.HasValue)
                conds.Add(AttendanceMonth.START_DATE_FIELD, endDate.Value, ConditionRelation.LessEqual);

            return DoRead(u => new DataAccessBase<AttendanceMonth>(u).GetAll(conds));
        }
    }
}
