using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class MarkingPeriodClassDataAccess : DataAccessBase<MarkingPeriodClass>
    {

        public MarkingPeriodClassDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        
        public void Delete(MarkingPeriodClassQuery query)
        {
            SimpleDelete(BuildConditions(query));
        }

        public MarkingPeriodClass GetMarkingPeriodClassOrNull(MarkingPeriodClassQuery query)
        {
            return SelectOneOrNull<MarkingPeriodClass>(BuildConditions(query));
        }
        public MarkingPeriodClass GetMarkingPeriodClass(MarkingPeriodClassQuery query)
        {
            return SelectOne<MarkingPeriodClass>(BuildConditions(query));
        }
        public IList<MarkingPeriodClass> GetList(MarkingPeriodClassQuery query)
        {
            return SelectMany<MarkingPeriodClass>(BuildConditions(query));
        } 

        public bool Exists(MarkingPeriodClassQuery query)
        {
            return Exists<MarkingPeriodClass>(BuildConditions(query));
        }
        public bool Exists(IList<int> markingPeriodIds)
        {
            if (markingPeriodIds.Count == 0)
                return false;
            var mpIdsString = markingPeriodIds.Select(x => "'" + x.ToString(CultureInfo.InvariantCulture) + "'").JoinString(",");
            var query = new DbQuery();
            query.Sql.Append(string.Format(@"select * from MarkingPeriodClass where MarkingPeriodRef in ({0})", mpIdsString));
            return Exists(query);
        }

        private QueryCondition BuildConditions(MarkingPeriodClassQuery query)
        {
            var res = new AndQueryCondition();
            if(query.ClassId.HasValue)
                res.Add(MarkingPeriodClass.CLASS_REF_FIELD, query.ClassId);
            if(query.MarkingPeriodId.HasValue)
                res.Add(MarkingPeriodClass.MARKING_PERIOD_REF_FIELD, query.MarkingPeriodId);
            return res;
        } 
    }

    public class MarkingPeriodClassQuery
    {
        public int? ClassId { get; set; }
        public int? MarkingPeriodId { get; set; }
    }
}
