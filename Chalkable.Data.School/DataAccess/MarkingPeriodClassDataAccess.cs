using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class MarkingPeriodClassDataAccess : DataAccessBase<MarkingPeriodClass>
    {

        public MarkingPeriodClassDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        
        //TODO: build generel method for list deleting 
        public void Delete(List<MarkingPeriodClass> markingPeriodClasses)
        {
            var b = new StringBuilder();
            foreach (var markingPeriodClass in markingPeriodClasses)
            {
                b.AppendFormat("delete from MarkingPeriodClass where Id = '{0}' ", markingPeriodClass.Id);
            }
            ExecuteNonQueryParametrized(b.ToString(), new Dictionary<string, object>());
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
        public bool Exists(IList<Guid> markingPeriodIds)
        {
            var sql = @"select * from MarkingPeriodClass where MarkingPeriodRef in ({0})";
            var mpIdsString = markingPeriodIds.Select(x => "'" + x.ToString() + "'").JoinString(",");
            return Exists(new DbQuery
                    {
                        Parameters = new Dictionary<string, object>(),
                        Sql = string.Format(sql, mpIdsString)
                    });
        }

        private Dictionary<string, object> BuildConditions(MarkingPeriodClassQuery query)
        {
            var res = new Dictionary<string, object>();
            if(query.Id.HasValue)
                res.Add("Id", query.Id);
            if(query.ClassId.HasValue)
                res.Add(MarkingPeriodClass.CLASS_REF_FIELD, query.ClassId);
            if(query.MarkingPeriodId.HasValue)
                res.Add(MarkingPeriodClass.MARKING_PERIOD_REF_FIELD, query.MarkingPeriodId);
            return res;
        } 
    }

    public class MarkingPeriodClassQuery
    {
        public Guid? Id { get; set; }
        public Guid? ClassId { get; set; }
        public Guid? MarkingPeriodId { get; set; }
    }
}
