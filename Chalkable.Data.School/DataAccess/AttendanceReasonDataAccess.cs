using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
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
            SimpleDelete(ids.Select(x=>new AttendanceReason{Id = x}).ToList());
        }

        public override IList<AttendanceReason> GetAll(QueryCondition conditions = null)
        {
            if (conditions != null)
                throw new NotImplementedException();
            return GetAttendanceReasons(null);
        }
        
        public override AttendanceReason GetById(int key)
        {
            return GetAttendanceReasons(key).First();
        }
        
        private IList<AttendanceReason> GetAttendanceReasons(int? id)
        {
            IDictionary<string, object> ps = new Dictionary<string, object>
            {
                {"@id", id}
            };
            using (var reader = ExecuteStoredProcedureReader("spGetAttendanceReasons", ps))
            {
                return ReadGetAttendanceReasonResult(reader);
            }
        }

        public static IList<AttendanceReason> ReadGetAttendanceReasonResult(DbDataReader reader)
        {
            IDictionary<int, AttendanceReason> attReasonDic = new Dictionary<int, AttendanceReason>();
            var tableName = typeof (AttendanceLevelReason).Name;
            while (reader.Read())
            {
                var reason = reader.Read<AttendanceReason>(true);
                if (!attReasonDic.ContainsKey(reason.Id))
                {
                    reason.AttendanceLevelReasons = new List<AttendanceLevelReason>();
                    attReasonDic.Add(reason.Id, reason);
                }
                if (!reader.IsDBNull(reader.GetOrdinal(string.Format("{0}_{1}", tableName, AttendanceLevelReason.ID_FIELD))))
                {
                    var attLevelReason = reader.Read<AttendanceLevelReason>(true);
                    if (attLevelReason != null && attLevelReason.Id > 0)
                    {
                        attLevelReason.AttendanceReason = reason;
                        attReasonDic[reason.Id].AttendanceLevelReasons.Add(attLevelReason);
                    }
                }

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
            IList<AttendanceLevelReason> toDelete = ids.Select(x => new AttendanceLevelReason {Id = x}).ToList();
            SimpleDelete(toDelete);
        }
    }
}
