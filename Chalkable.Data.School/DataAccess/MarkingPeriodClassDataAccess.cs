using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class MarkingPeriodClassDataAccess : BaseSchoolDataAccess<MarkingPeriodClass>
    {

        public MarkingPeriodClassDataAccess(UnitOfWork unitOfWork, int? schoolId) : base(unitOfWork, schoolId)
        {
        }
        
        //TODO: build generel method for list deleting 
        public void Delete(IList<MarkingPeriodClass> markingPeriodClasses)
        {
            SimpleDelete<MarkingPeriodClass>(markingPeriodClasses);
        }
       
        public void Delete(MarkingPeriodClassQuery query)
        {
            SimpleDelete<MarkingPeriodClass>(BuildConditions(query));
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
            var mpIdsString = markingPeriodIds.Select(x => "'" + x.ToString() + "'").JoinString(",");
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
