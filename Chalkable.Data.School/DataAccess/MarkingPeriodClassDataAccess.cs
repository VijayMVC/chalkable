﻿using System;
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

        private const string MARKING_PERIOD_REF_FIELD = "markingPeriodRef";
        private const string CLASS_REF_FIELD = "classRef";
       
        public void Create(MarkingPeriodClass markingPeriodClass)
        {
            SimpleInsert(markingPeriodClass);
        }
        public void Create(IList<MarkingPeriodClass> markingPeriodClasses)
        {
            SimpleInsert(markingPeriodClasses);
        }
        
        public void Update(MarkingPeriodClass markingPeriodClass)
        {
            SimpleUpdate(markingPeriodClass);
        }
        public void Delete(MarkingPeriodClass markingPeriodClass)
        {
            SimpleDelete(markingPeriodClass);
        }
        //TODO: build generel method for list deleting 
        public void Delete(List<MarkingPeriodClass> markingPeriodClasses)
        {
            var b = new StringBuilder();
            foreach (var markingPeriodClass in markingPeriodClasses)
            {
                b.AppendFormat("delete from MarkingPeriodClass where Id = {0} ", markingPeriodClass.Id);
            }
            ExecuteNonQueryParametrized(b.ToString(), new Dictionary<string, object>());
        }


        public void Delete(Guid classId)
        {
            var conds = new Dictionary<string, object> { {CLASS_REF_FIELD, classId } };
            SimpleDelete<MarkingPeriodClass>(conds);
        }
        
        public void Delete(Guid classId, Guid markingPeriodId)
        {
            var conds = new Dictionary<string, object>
                {
                    {CLASS_REF_FIELD, classId},
                    {MARKING_PERIOD_REF_FIELD, markingPeriodId}
                };
           SimpleDelete<MarkingPeriodClass>(conds);
        }
        
        public MarkingPeriodClass Get(Guid classId, Guid markingPeriodId)
        {
            var conds = new Dictionary<string, object>
                {
                    {CLASS_REF_FIELD, classId},
                    {MARKING_PERIOD_REF_FIELD, markingPeriodId}
                };
            return SelectOne<MarkingPeriodClass>(conds);
        }

        public IList<MarkingPeriodClass> GetList(Guid classId)
        {
            var conds = new Dictionary<string, object> { { CLASS_REF_FIELD, classId } };
            return SelectMany<MarkingPeriodClass>(conds);
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
                    {CLASS_REF_FIELD, classId},
                    {MARKING_PERIOD_REF_FIELD, markingPeriodId}
                });
        }
    }
}
