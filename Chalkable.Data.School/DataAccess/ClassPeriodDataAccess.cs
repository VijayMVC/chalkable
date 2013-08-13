using System;
using System.Collections.Generic;
using System.Text;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{

    public class ClassPeriodDataAccess : DataAccessBase<ClassPeriod>
    {
        public ClassPeriodDataAccess(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public void FullDelete(Guid id)
        {
            var b = new StringBuilder();
            var conds = new AndQueryCondition {{ClassAttendance.CLASS_PERIOD_REF_FIELD, id}};
            var deleteAttQ = Orm.SimpleDelete<ClassAttendance>(conds);

            b.Append(deleteAttQ.Sql).Append(" ");
            b.Append(Orm.SimpleDelete<ClassDiscipline>(conds).Sql).Append(" ");

            conds.Add(ClassPeriod.ID_FIELD, id);
            var classPeriodQ = Orm.SimpleDelete<ClassPeriod>(new AndQueryCondition {{ClassPeriod.ID_FIELD, id}});
            b.Append(classPeriodQ.Sql);
            
            var allParams = deleteAttQ.Parameters;
            foreach (var parameter in classPeriodQ.Parameters)
            {
                allParams.Add(parameter);    
            }
            
            ExecuteNonQueryParametrized(b.ToString(), allParams);
        }

        public bool Exists(ClassPeriodQuery query)
        {
            return Exists(BuildGetClassPeriodsQuery(query));
        }

        public bool Exists(IList<Guid> markingPeriodIds)
        {
            var sql = @"select cd.Id from ClassPeriod cd 
                        join Period p on p.Id = cd.PeriodRef
                        where p.MarkingPeriodRef in ({0})";
            var mpDic = new Dictionary<string, object>();
            for (var i = 0; i < markingPeriodIds.Count; i++)
            {
                mpDic.Add("@markingPeriodId_" + i, markingPeriodIds[i]);
            }
            sql = string.Format(sql, mpDic.Keys.JoinString(","));
            return Exists(new DbQuery(sql, mpDic));
        }

        public bool IsClassStudentsAssignedToPeriod(Guid periodId, Guid classId)
        {
            var sql = @"select * from ClassPerson
                        where ClassRef = @classId and PersonRef in (
	                        select csp.PersonRef from ClassPerson csp
	                        join ClassPeriod cgp on cgp.ClassRef = csp.ClassRef and cgp.PeriodRef = @periodId)";

            var conds = new Dictionary<string, object> {{"periodId", periodId}, {"classId", classId}};
            var query = new DbQuery(sql, conds);
            return Exists(query);
        }

        public bool IsStudentAlreadyAssignedToClassPeriod(Guid personId, Guid classId)
        {
            var sql = @"select * from ClassPeriod cPeriod
                        where cPeriod.ClassRef = @classId and cPeriod.PeriodRef in (
	                        select cPeriod.PeriodRef from ClassPeriod cPeriod
	                        join ClassPerson cPerson on cPerson.ClassRef = cPeriod.ClassRef and cPerson.PersonRef = @personId)";

            var conds = new Dictionary<string, object> { { "personId", personId }, { "classId", classId } };
            var query = new DbQuery(sql, conds);
            return Exists(query);
        }


        public IList<Class> GetAvailableClasses(Guid periodId)
        {
            var dbQuery = new DbQuery();
            dbQuery.Sql.Append(@"select c.* from ClassPeriod 
                                 join Class c on c.Id = ClassPeriod.ClassRef");
            var conds = new AndQueryCondition { { ClassPeriod.PERIOD_REF_FIELD, periodId } };
            conds.BuildSqlWhere(dbQuery, typeof(ClassPeriod).Name);
            return ReadMany<Class>(dbQuery);
        } 
        public IList<Room> GetAvailableRooms(Guid periodId)
        {
            var dbQuery = new DbQuery();
            dbQuery.Sql.Append(@"select r.* from ClassPeriod  
                                 join Room r on r.Id = ClassPeriod.RoomRef");
            var conds = new AndQueryCondition {{ClassPeriod.PERIOD_REF_FIELD, periodId}};
            conds.BuildSqlWhere(dbQuery, typeof(ClassPeriod).Name);
            return ReadMany<Room>(dbQuery);
        } 


        public IList<ClassPeriod> GetClassPeriods(ClassPeriodQuery query)
        {
            var res = BuildGetClassPeriodsQuery(query);
            return ReadMany<ClassPeriod>(res, true);
        } 

//        public IList<ClassPeriodDetails> GetClassPeriodsDetails(ClassPeriodQuery query)
//        {
//            var b = new StringBuilder();
//            var types = new List<Type> { typeof(ClassPeriod), typeof(Period), typeof(Room)};
//            b.AppendFormat(@"select {0} from ClassPeriod 
//                             join Period on Period.Id = ClassPeriod.PeriodRef
//                             join Room on Room.Id = ClassPeriod.RoomRef"
//                           , Orm.ComplexResultSetQuery(types));
//            return ReadMany<ClassPeriodDetails>(BuildGetClassPeriodsConditions(b, query),true);
//        }

        public DbQuery BuildGetClassPeriodsQuery(ClassPeriodQuery query)
        {
            var b = new StringBuilder();
            var types = new List<Type> {typeof (ClassPeriod), typeof (Period)};
            b.AppendFormat(@"select {0} from ClassPeriod 
                             join Period on Period.Id = ClassPeriod.PeriodRef"
                           , Orm.ComplexResultSetQuery(types));
            return BuildGetClassPeriodsConditions(b, query);
        }
        private DbQuery BuildGetClassPeriodsConditions(StringBuilder builder, ClassPeriodQuery query)
        {
            var conds = new Dictionary<string, object>();
            builder.Append(" where 1=1 ");
            if (query.Id.HasValue)
            {
                conds.Add("Id", query.Id);
                builder.AppendFormat(" and ClassPeriod.Id = @id");
            }
            if (query.MarkingPeriodId.HasValue)
            {
                conds.Add("markingPeriodId", query.MarkingPeriodId);
                builder.AppendFormat(" and Period.MarkingPeriodRef = @markingPeriodId");
            }
            if (query.PeriodId.HasValue)
            {
                conds.Add("periodId", query.PeriodId);
                builder.AppendFormat(" and ClassPeriod.PeriodRef = @periodId");
            }
            if (query.RoomId.HasValue)
            {
                conds.Add("roomId", query.RoomId);
                builder.AppendFormat(" and ClassPeriod.RoomRef = @roomId");
            }
            if (query.SectionId.HasValue)
            {
                conds.Add("sectionId", query.SectionId);
                builder.AppendFormat(" and Period.SectionRef = @sectionId");
            }
            if (query.StudentId.HasValue)
            {
                conds.Add("studentId", query.StudentId);
                builder.AppendFormat(" and ClassPeriod.ClassRef in (select ClassRef from ClassPerson where PersonRef = @studentId)");
            }
            if (query.TeacherId.HasValue)
            {
                conds.Add("teacherId", query.TeacherId);
                builder.AppendFormat(" and ClassPeriod.ClassRef in (select Id from Class where TeacherRef = @teacherId)");
            }
            if (query.Time.HasValue)
            {
                conds.Add("time", query.Time);
                builder.AppendFormat(" and Period.StartTime <= @time and Period.EndTime >= @time");
            }
            if (query.ClassIds != null && query.ClassIds.Count > 0)
            {
                var classIdsParams = new List<string>();
                for (int i = 0; i < query.ClassIds.Count; i++)
                {
                    var classIdParam = "@classId_" + i;
                    classIdsParams.Add(classIdParam);
                    conds.Add(classIdParam, query.ClassIds[i]);
                }
                builder.AppendFormat(" and ClassPeriod.ClassRef in ({0})",  classIdsParams.JoinString(","));
            }
            return new DbQuery(builder.ToString(), conds);
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
