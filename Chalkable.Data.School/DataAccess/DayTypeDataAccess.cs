using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class DayTypeDataAccess : DataAccessBase<DayType, int>
    {
        public DayTypeDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Delete(IList<int> ids)
        {
            var res = new StringBuilder();
            var sqlFormat = " delete from [{0}] where [{0}].[{1}] in ({2}) ";
            var idsS = ids.Select(x => x.ToString()).JoinString(",");
            res.AppendFormat(sqlFormat, "ClassPeriod", ClassPeriod.DAY_TYPE_REF_FIELD, idsS)
               .AppendFormat(sqlFormat, "Date", Date.DATE_TYPE_REF_FIELD, idsS)
               .AppendFormat(sqlFormat, "DayType", DayType.ID_FIELD, idsS);
            ExecuteNonQueryParametrized(res.ToString(), new Dictionary<string, object>());
        }

        public void Delete(DayType dayType)
        {
            //var cpQuery = Orm.SimpleDelete<ClassPeriod>(new AndQueryCondition {{ClassPeriod.DAY_TYPE_REF_FIELD, dayType.Id}});
            //var updateParams = new Dictionary<string, object> {{Date.DATE_TIME_FIELD, null}};
            //var updateDateQuery = Orm.SimpleUpdate<Date>(updateParams, new AndQueryCondition { { Date.DATE_TYPE_REF_FIELD, dayType.Id} });
            //var deleteSectionQuery = Orm.SimpleDelete(dayType);
            //var query = new DbQuery(new List<DbQuery> { cpQuery, updateDateQuery, deleteSectionQuery });
            //ExecuteNonQueryParametrized(query.Sql.ToString(), query.Parameters);
            Delete(new List<int> {dayType.Id});
        }

        public IList<DayType> GetDateTypes(IList<int> ids)
        {
            if (ids.Count == 0)
                return new List<DayType>();
            var sql = string.Format("select * from [DayType] where Id in ({0})",
                                    ids.Select(x => x.ToString()).JoinString(","));
            return ReadMany<DayType>(new DbQuery(sql, new Dictionary<string, object>()));
        } 

        public IList<DayType> GetDateTypes(int schoolYearId, int? fromNumber = null, int? toNumber = null)
        {
            var conds = new AndQueryCondition { { DayType.SCHOOL_YEAR_REF, schoolYearId } };
            if (fromNumber.HasValue)
                conds.Add(DayType.NUMBER_FIELD, "fromNumber", fromNumber.Value, ConditionRelation.GreaterEqual);
            if (toNumber.HasValue)
                conds.Add(DayType.NUMBER_FIELD, "tillNumber", toNumber.Value, ConditionRelation.LessEqual);
            var dbQuery = Orm.SimpleSelect<DayType>(conds);
            dbQuery.Sql.AppendFormat("  order by [{0}].[{1}] ", "DayType", DayType.NUMBER_FIELD);
            return ReadMany<DayType>(dbQuery);
        } 

        public bool Exists(int schoolYearId)
        {
            return Exists<DayType>(new AndQueryCondition {{DayType.SCHOOL_YEAR_REF, schoolYearId}});
        }
    }
}
