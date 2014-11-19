﻿using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class PeriodDataAccess : BaseSchoolDataAccess<Period>
    {
        public PeriodDataAccess(UnitOfWork unitOfWork, int? schoolId)
            : base(unitOfWork, schoolId)
        {
        }

        public void DeleteBySchoolYearId(int? schoolYearId)
        {
            SimpleDelete(GetCondsBySchoolYear(schoolYearId));
        }

        public void Delete(IList<int> ids)
        {
            SimpleDelete<Period>(ids.Select(x => new Period {Id = x}).ToList());
        }

        public IList<Period> GetPeriods(int schoolYearId)
        {
            return SelectMany<Period>(GetCondsBySchoolYear(schoolYearId));
        }

        public Period GetPeriodOrNull(int time, int schoolYearId)
        {
            return SelectOneOrNull<Period>(new AndQueryCondition
                {
                    {Period.START_TIME_FIELD, time, ConditionRelation.LessEqual},
                    {Period.END_TIME_FIELD, time, ConditionRelation.GreaterEqual},
                    GetCondsBySchoolYear(schoolYearId)
                });
        }
        private QueryCondition GetCondsBySchoolYear(int? schoolYearId)
        {
            var res = new AndQueryCondition();
            if (schoolYearId.HasValue)
                res.Add(Period.SCHOOL_YEAR_REF, schoolYearId);
            return res;
        }
    }
}
