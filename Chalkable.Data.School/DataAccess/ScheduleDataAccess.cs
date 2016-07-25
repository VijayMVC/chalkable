using System;
using System.Collections.Generic;
using System.Diagnostics;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class ScheduleDataAccess : DataAccessBase<ScheduleItem>
    {
        public ScheduleDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<ScheduleItem> GetSchedule(int schoolYearId, int? teacherId, int? studentId, int? classId, int? callerId, DateTime from, DateTime to)
        {
            Trace.Assert(teacherId.HasValue || studentId.HasValue || classId.HasValue);
            IDictionary<string, object> ps = new Dictionary<string, object>
            {
                {"@SchoolYearId", schoolYearId},
                {"@teacherId", teacherId},
                {"@studentId", studentId},
                {"@classId", classId},
                {"@callerId", callerId},
                {"@from", from.Date},
                {"@to", to.Date},
            };
            return ExecuteStoredProcedureList<ScheduleItem>("spGetSchedule", ps);
        }

    }
}