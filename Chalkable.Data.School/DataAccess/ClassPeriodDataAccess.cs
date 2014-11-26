using System;
using System.Collections.Generic;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{

    public class ClassPeriodDataAccess : BaseSchoolDataAccess<ClassPeriod>
    {
        public ClassPeriodDataAccess(UnitOfWork unitOfWork, int? schoolId)
            : base(unitOfWork, schoolId)
        {
        }

        public void FullDelete(int periodId, int classId, int dayTypeId)
        {
            var conds = new AndQueryCondition();
            conds.Add(ClassPeriod.CLASS_REF_FIELD, classId);
            conds.Add(ClassPeriod.PERIOD_REF_FIELD, periodId);
            conds.Add(ClassPeriod.DAY_TYPE_REF_FIELD, dayTypeId);
            SimpleDelete(conds);
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
