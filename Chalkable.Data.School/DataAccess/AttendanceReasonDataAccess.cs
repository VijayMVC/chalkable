using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class AttendanceReasonDataAccess : DataAccessBase<AttendanceReason, int>
    {
        public AttendanceReasonDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        

        public void Delete(IList<int> ids)
        {
            if (ids != null && ids.Count > 0)
            {
                var sqlFormat = "delete from [{0}] where [{0}].[{1}] in ({2})";
                var idsS = ids.Select(x => x.ToString()).JoinString(",");
                var res = new StringBuilder();
                res.AppendFormat(sqlFormat, "AttendanceLevelReason", AttendanceLevelReason.ATTENDACNE_REASON_REF_FIELD, idsS)
                   .AppendFormat(sqlFormat, "AttendanceReason", AttendanceReason.ID_FIELD, idsS);
            }
        }

        public override IList<AttendanceReason> GetAll(QueryCondition conditions = null)
        {
            return Read(BuildGetReasonsDbQuery(conditions), ReadGetAttendanceReasonReasult);
        }

        public override AttendanceReason GetById(int key)
        {
            var conds = new AndQueryCondition {{AttendanceReason.ID_FIELD, key}};
            return Read(BuildGetReasonsDbQuery(conds), ReadGetAttendanceReasonReasult).First();
        }
        public override AttendanceReason GetByIdOrNull(int key)
        {
            var conds = new AndQueryCondition { { AttendanceReason.ID_FIELD, key } };
            return Read(BuildGetReasonsDbQuery(conds), ReadGetAttendanceReasonReasult).FirstOrDefault();
        }

        private DbQuery BuildGetReasonsDbQuery(QueryCondition attReasonConds, QueryCondition attLevelReasonConds = null)
        {
            var res = new DbQuery();
            var types = new List<Type> { typeof(AttendanceReason), typeof(AttendanceLevelReason) };
            res.Sql.AppendFormat(@"select {0} from [{1}] join [{3}] on [{3}].[{4}] = [{1}].[{2}]"
                                 , Orm.ComplexResultSetQuery(types), types[0].Name, AttendanceReason.ID_FIELD
                                 , types[1].Name, AttendanceLevelReason.ATTENDACNE_REASON_REF_FIELD);

            if (attReasonConds != null)
                attReasonConds.BuildSqlWhere(res, types[0].Name);
            if (attLevelReasonConds != null)
                attLevelReasonConds.BuildSqlWhere(res, types[1].Name, attReasonConds != null);
            return res;
        }

        private IList<AttendanceReason> ReadGetAttendanceReasonReasult(DbDataReader reader)
        {
            IDictionary<int, AttendanceReason> attReasonDic = new Dictionary<int, AttendanceReason>();
            while (reader.Read())
            {
                var reason = reader.Read<AttendanceReason>(true);
                if (!attReasonDic.ContainsKey(reason.Id))
                {
                    reason.AttendanceLevelReasons = new List<AttendanceLevelReason>();
                    attReasonDic.Add(reason.Id, reason);
                }
                var attLevelReason = reader.Read<AttendanceLevelReason>(true);
                attLevelReason.AttendanceReason = reason;
                attReasonDic[reason.Id].AttendanceLevelReasons.Add(attLevelReason);
            }
            return attReasonDic.Select(x=>x.Value).ToList();
        } 
    }

    public class AttendanceLevelReasonDataAccess : DataAccessBase<AttendanceLevelReason, int>
    {
        public AttendanceLevelReasonDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Delete(IList<int> ids)
        {
            SimpleDelete(ids.Select(x => new AttendanceLevelReason {Id = x}));
        }
    }
}
