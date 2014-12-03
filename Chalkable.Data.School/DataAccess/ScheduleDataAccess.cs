using System;
using System.Collections.Generic;
using System.Diagnostics;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class ScheduleDataAccess : BaseSchoolDataAccess<ScheduleItem>
    {
        public ScheduleDataAccess(UnitOfWork unitOfWork, int? localSchoolId) : base(unitOfWork, localSchoolId)
        {
        }

        public IList<ScheduleItem> GetSchedule(int schoolYearId, int? teacherId, int? studentId, int? classId, DateTime from, DateTime to)
        {
            Trace.Assert(teacherId.HasValue || studentId.HasValue || classId.HasValue);
            IDictionary<string, object> ps = new Dictionary<string, object>
            {
                {"@SchoolYearId", schoolYearId},
                {"@teacherId", teacherId},
                {"@studentId", studentId},
                {"@classId", classId},
                {"@from", from.Date},
                {"@to", to.Date},
            };
            return ExecuteStoredProcedureList<ScheduleItem>("spGetSchedule", ps);
        }

    }
}