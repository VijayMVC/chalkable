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

    public class ClassPeriodDataAccess : DataAccessBase
    {
        public ClassPeriodDataAccess(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public void Create(ClassPeriod classPeriod)
        {
            SimpleInsert(classPeriod);
        }

        public void Delete(Guid id)
        {
            SimpleDelete(new Dictionary<string, object> { { "Id", id } });
        }

        public bool Exists(ClassPeriodQuery query)
        {
            return Exists(BuildGetClassPeriodsQuery(query));
        }

        public bool HasStudentsTwoClassesInPeriod(Guid periodId, Guid classId)
        {
            var sql = @"select * from ClassPerson
                        where ClassRef = @classId and PersonRef in (
	                        select csp.PersonRef from ClassPerson csp
	                        join ClassPeriod cgp on cgp.ClassRef = csp.ClassRef and cgp.PeriodRef = @periodId)";

            var conds = new Dictionary<string, object> {{"periodId", periodId}, {"classId", classId}};
            var query = new DbQuery{Sql = sql, Parameters = conds};
            return Exists(query);
        }


        public IList<ClassPeriod> GetClassPeriods(ClassPeriodQuery query)
        {
            var res = BuildGetClassPeriodsQuery(query);
            return ReadMany<ClassPeriod>(res, true);
        } 

        public DbQuery BuildGetClassPeriodsQuery(ClassPeriodQuery query)
        {
            var b = new StringBuilder();
            var types = new List<Type> {typeof (ClassPeriod), typeof (Period)};
            b.AppendFormat(@"select {0} from ClassPeriod 
                             join Period on Period.Id = ClassPeriod.PeriodRef "
                           , Orm.ComplexResultSetQuery(types));
            return BuildGetClassPeriodsConditions(b, query);
        }

        private DbQuery BuildGetClassPeriodsConditions(StringBuilder builder, ClassPeriodQuery query)
        {
            var conds = new Dictionary<string, object>();
            string where = " where ";
            if (query.Id.HasValue)
            {
                conds.Add("Id", query.Id);
                builder.AppendFormat(" {0} ClassPeriod.Id = @id", where);
                where = " and ";
            }
            if (query.MarkingPeriodId.HasValue)
            {
                conds.Add("markingPeriodId", query.Id);
                builder.AppendFormat(" {0} Period.MarkingPeriodRef = @markingPeriodId", where);
                where = " and ";    
            }
            if (query.PeriodId.HasValue)
            {
                conds.Add("periodId", query.PeriodId);
                builder.AppendFormat(" {0} ClassPeriod.PeriodRef = @periodId", where);
                where = " and ";
            }
            if (query.RoomId.HasValue)
            {
                conds.Add("roomId", query.RoomId);
                builder.AppendFormat(" {0} ClassPeriod.RoomRef = @roomId", where);
                where = " and ";
            }
            if (query.SectionId.HasValue)
            {
                conds.Add("sectionId", query.SectionId);
                builder.AppendFormat(" {0} Period.SectionRef = @sectionId", where);
                where = " and ";
            }
            if (query.StudentId.HasValue)
            {
                conds.Add("studentId", query.StudentId);
                builder.AppendFormat(" {0} ClassPeriod.ClassRef in (select ClassRef from ClassPerson where PersonRef = @studentId)", where);
                where = " and ";
            }
            if (query.StudentId.HasValue)
            {
                conds.Add("studentId", query.StudentId);
                builder.AppendFormat(" {0} ClassPeriod.ClassRef in (select Id from Class where TeacherRef = @studentId)", where);
                where = " and ";
            }
            if (query.Time.HasValue)
            {
                conds.Add("time", query.Time);
                builder.AppendFormat(" {0} Period.StartTime <= @time and Period.EndTime >= time", where);
                where = " and ";
            }
            if (query.ClassIds != null && query.ClassIds.Count > 0)
            {
                builder.AppendFormat(" {0} ClassPeriod.ClassRef in ({1})", where, query.ClassIds.Select(x => x.ToString()).JoinString(","));
            }
            return new DbQuery {Sql = builder.ToString(), Parameters = conds};
        }

    }

    public class ClassPeriodQuery
    {
        public Guid? Id { get; set; }
        public Guid? MarkingPeriodId { get; set; }
        public List<Guid> ClassIds { get; set; }
        public Guid? RoomId { get; set; }
        public Guid? PeriodId { get; set; }
        public Guid? StudentId { get; set; }
        public Guid? TeacherId { get; set; }
        public Guid? SectionId { get; set; }
        public int? Time { get; set; }
    }
}
