using System;
using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{

    public class ClassPeriodDataAccess : DataAccessBase<ClassPeriod>
    {
        public ClassPeriodDataAccess(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public Class CurrentClassForTeacher(int schoolYearId, int personId, DateTime date, int time)
        {
            IDictionary<string, object> ps = new Dictionary<string, object>
            {
                {"@schoolYearId", schoolYearId},
                {"@teacherId", personId},
                {"@date", date},
                {"@time", time},
            };
            using (var reader = ExecuteStoredProcedureReader("spCurrentClassForTeacher", ps))
            {
                return reader.ReadOrNull<Class>();
            }
        }
    }
}
