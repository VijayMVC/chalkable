﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class PeriodDataAccess : DataAccessBase
    {
        public PeriodDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Create(Period period)
        {
            SimpleInsert(period);
        }
        public void Create(IList<Period> periods)
        {
            SimpleInsert(periods);
        }

        public void Update(Period period)
        {
            SimpleUpdate(period);
        }
        public void Delete(Guid id)
        {
             SimpleDelete<Period>(new Dictionary<string, object>{{"Id", id}});
        }

         public void Delete(IList<Guid> markingPeriodIds)
         {
             var mpIds = markingPeriodIds.Select(x => x.ToString()).JoinString(",");
             var sql = string.Format("delete from Period where MarkingPeriodRef in ({0})", mpIds);
             ExecuteNonQueryParametrized(sql, new Dictionary<string, object>());
         }


        public Period GeComplextById(Guid id)
        {
            return GetComplexPeriods(new Dictionary<string, object> {{"Id", id}}).First();
        }
        public IList<Period> GetPeriods(Guid sectionId)
        {
            var conds = new Dictionary<string, object> {{"SectionRef", sectionId}};
            return SelectMany<Period>(conds);
        } 

        public Period GetPeriodOrNull(Guid sectionId, int time)
        {
            var sql = "select * from Period where SectionRef = @sectionId and StartTime <= @time and EndTime >= @time";
            var conds = new Dictionary<string, object> {{"sectionId", sectionId}, {"time", time}};
            using (var reader = ExecuteReaderParametrized(sql, conds))
            {
                return reader.ReadOrNull<Period>();
            }
        } 

        public IList<Period> GetComplexPeriods(Guid? sectionId, Guid markingPeriodId)
        {
            var conds = new Dictionary<string, object> {{"markingPeriodRef", markingPeriodId}};
            if (sectionId.HasValue)
                conds.Add("sectionRef", sectionId);
            return GetComplexPeriods(conds);
        } 
        private IList<Period> GetComplexPeriods(Dictionary<string, object> conds)
        {
            var sql = @"select {0} from Period 
                        join ScheduleSectioin on ScheduleSection.Id = Period.SectionRef";
            var b = new StringBuilder();
            var types = new List<Type> {typeof (Period), typeof (ScheduleSection)};
            b.AppendFormat(sql, Orm.ComplexResultSetQuery(types));
            b = Orm.BuildSqlWhere(b, types[0], conds);
            b.Append(" order by Period.StartTime, ScheduleSection.Number");
            using (var reader = ExecuteReaderParametrized(b.ToString(), conds))
            {
                return reader.ReadList<Period>(true);
            }
        }
    }
}
