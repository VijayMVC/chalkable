using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class MarkingPeriodClassDataAccess : DataAccessBase
    {
        public MarkingPeriodClassDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        public void Create(MarkingPeriodClass markingPeriodClass)
        {
            SimpleInsert(markingPeriodClass);
        }
        public void Update(MarkingPeriodClass markingPeriodClass)
        {
            SimpleUpdate(markingPeriodClass);
        }
        public void Delete(MarkingPeriodClass markingPeriodClass)
        {
            SimpleDelete(markingPeriodClass);
        }

        public void Delete(Guid classId)
        {
            var conds = new Dictionary<string, object> {{"@classId", classId}};
            SimpleDelete<MarkingPeriodClass>(conds);
        }
        
        public void Delete(Guid classId, Guid markingPeriodId)
        {
            var conds = new Dictionary<string, object>
                {
                    {"@classId", classId},
                    {"@markingPeriodId", markingPeriodId}
                };
           SimpleDelete<MarkingPeriodClass>(conds);
        }
        
        
        public string BuildSelectCommand(string resultSet, IDictionary<string, object> conditions)
        {
            var sql = new StringBuilder();
            sql.AppendFormat("select {0} from MarkingPeriodClass ", resultSet);
            if (conditions.Count > 0)
                sql.Append("where");
            bool isFirst = true;
            foreach (var cond in conditions)
            {
                if (isFirst) isFirst = false;
                else
                {
                    sql.Append(" and ");
                }
                sql.AppendFormat("{0}=@{0}", cond.Key);
            }
            return sql.ToString();
        }

        public bool Exists(Dictionary<string, object> conds)
        {
            var sql = new StringBuilder();
            var sqlCommand = BuildSelectCommand("count(*) as [Count]", conds);
            using (var reader = ExecuteReaderParametrized(sqlCommand, conds))
            {
                return reader.Read() && SqlTools.ReadInt32(reader, "[Count]") > 0;
            }
        }
        
        public bool Exists(Guid classId, Guid markingPeriodId)
        {
            return Exists(new Dictionary<string, object>
                {
                    {"@classId", classId},
                    {"@markingPeriodId", markingPeriodId}
                });
        }
    }
}
