using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class PeriodDataAccess : DataAccessBase<Period>
    {
        public PeriodDataAccess(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public void Delete(IList<Guid> markingPeriodIds)
        {
            var mpIds = markingPeriodIds.Select(x => "'" + x.ToString() + "'").JoinString(",");
            var sql = string.Format("delete from Period where {0} in ({1})", Period.MARKING_PERIOD_REF_FIELD, mpIds);
            ExecuteNonQueryParametrized(sql, new Dictionary<string, object>());
        }


        public Period GeComplextById(Guid id)
        {
            return GetComplexPeriods(new AndQueryCondition { { Period.ID_FIELD, id } }).First();
        }
        public IList<Period> GetPeriods(Guid sectionId)
        {
            var conds = new AndQueryCondition { { Period.SECTION_REF, sectionId } };
            return SelectMany<Period>(conds);
        }

        public Period GetPeriodOrNull(Guid sectionId, int time)
        {
            const string timeParamName = "time";
            var conds = new AndQueryCondition
                {
                    {Period.SECTION_REF, sectionId},
                    {Period.START_TIME_FIELD, timeParamName, ConditionRelation.LessEqual},
                    {Period.END_TIME_FIELD, timeParamName, ConditionRelation.GreaterEqual}
                };
            return SelectOneOrNull<Period>(conds);
        }

        public IList<Period> GetComplexPeriods(Guid? sectionId, Guid? markingPeriodId)
        {
            var conds = new AndQueryCondition();
            if (markingPeriodId.HasValue)
                conds.Add(Period.MARKING_PERIOD_REF_FIELD, markingPeriodId);
            if (sectionId.HasValue)
                conds.Add(Period.SECTION_REF, sectionId);
            return GetComplexPeriods(conds);
        }
        private IList<Period> GetComplexPeriods(QueryCondition conds)
        {
            var sql = @"select {0} from Period 
                        join ScheduleSection on ScheduleSection.Id = Period.SectionRef";
            var dbQuery = new DbQuery(); 
            var types = new List<Type> { typeof(Period), typeof(ScheduleSection) };
            dbQuery.Sql.AppendFormat(sql, Orm.ComplexResultSetQuery(types));
            conds.BuildSqlWhere(dbQuery, types[0].Name);
            dbQuery.Sql.AppendFormat(" order by Period.{0}, ScheduleSection.{1}"
                    , Period.START_TIME_FIELD, ScheduleSection.NUMBER_FIELD);

            using (var reader = ExecuteReaderParametrized(dbQuery.Sql.ToString(), dbQuery.Parameters))
            {
                var res = new List<Period>();
                while (reader.Read())
                {
                    var period = reader.Read<Period>(true);
                    period.Section = reader.Read<ScheduleSection>(true);
                    res.Add(period);
                }
                return res;
            }
        }
    }
}
