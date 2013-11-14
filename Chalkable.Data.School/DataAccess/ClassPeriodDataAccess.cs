using System;
using System.Collections.Generic;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{

    public class ClassPeriodDataAccess : BaseSchoolDataAccess<ClassPeriod>
    {
        public ClassPeriodDataAccess(UnitOfWork unitOfWork, int? schoolId)
            : base(unitOfWork, schoolId)
        {
        }

        public void FullDelete(int periodId, int classId, int dayTypeId)
        {
            var conds = new AndQueryCondition();
            conds.Add(ClassPeriod.CLASS_REF_FIELD, classId);
            conds.Add(ClassPeriod.PERIOD_REF_FIELD, classId);
            conds.Add(ClassPeriod.DAY_TYPE_REF_FIELD, dayTypeId);
            SimpleDelete(conds);
        }

        public bool Exists(ClassPeriodQuery query)
        {
            return Exists(BuildGetClassPeriodsQuery(query));
        }

        public bool IsClassStudentsAssignedToPeriod(int periodId, int classId, int dateTypeId)
        {
            var sql = @"select * from ClassPerson
                        where ClassRef = @classId and PersonRef in (
	                        select csp.PersonRef from ClassPerson csp
	                        join ClassPeriod cgp on cgp.ClassRef = csp.ClassRef and cgp.PeriodRef = @periodId and cgp.DayTypeRef = @dayTypeId)";

            var conds = new Dictionary<string, object> { { "periodId", periodId }, { "classId", classId }, { "dayTypeId", dateTypeId } };
            var query = new DbQuery(sql, conds);
            return Exists(query);
        }

        public bool IsStudentAlreadyAssignedToClassPeriod(int personId, int classId)
        {
            var sql = @"select * from ClassPeriod cPeriod1 
                        where cPeriod1.ClassRef = @classId and exists(
	                        select * from ClassPeriod cPeriod2
	                        join ClassPerson cPerson on cPerson.ClassRef = cPeriod2.ClassRef and cPerson.PersonRef = @personId
                            where cPeriod2.PeriodRef = cPeriod1.PeriodRef and cPeriod1.DayTypeRef = cPeriod2.DayTypeRef)";

            var conds = new Dictionary<string, object> { { "personId", personId }, { "classId", classId } };
            var query = new DbQuery(sql, conds);
            return Exists(query);
        }


        public IList<Class> GetAvailableClasses(int periodId)
        {
            var dbQuery = new DbQuery();
            dbQuery.Sql.Append(@"select c.* from ClassPeriod 
                                 join Class c on c.Id = ClassPeriod.ClassRef");
            var conds = new AndQueryCondition { { ClassPeriod.PERIOD_REF_FIELD, periodId } };
            conds.BuildSqlWhere(dbQuery, typeof(ClassPeriod).Name);
            return ReadMany<Class>(dbQuery);
        }
        public IList<Room> GetAvailableRooms(int periodId)
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

        public DbQuery BuildGetClassPeriodsQuery(ClassPeriodQuery query)
        {
            var dbQuery = new DbQuery();
            var types = new List<Type> {typeof (ClassPeriod), typeof (Period)};
            dbQuery.Sql.AppendFormat(@"select {0} from [{1}] 
                                       join [{2}] on [{2}].[{3}] = [{1}].[{4}]"
                                     , Orm.ComplexResultSetQuery(types), types[0].Name, types[1].Name
                                     , Period.ID_FIELD, ClassPeriod.PERIOD_REF_FIELD);
            return BuildGetClassPeriodsConditions(dbQuery, query);
        }
        private DbQuery BuildGetClassPeriodsConditions(DbQuery dbQuery, ClassPeriodQuery query)
        {
            var conds = new AndQueryCondition();
            var classPeriodTName = "ClassPeriod";
            if (query.PeriodId.HasValue)
                conds.Add(ClassPeriod.PERIOD_REF_FIELD, query.PeriodId);
            if (query.RoomId.HasValue)
                conds.Add(ClassPeriod.ROOM_REF_FIELD, query.RoomId);
            if (query.DateTypeId.HasValue)
                conds.Add(ClassPeriod.DAY_TYPE_REF_FIELD, query.DateTypeId);

            FilterBySchool(conds).BuildSqlWhere(dbQuery, classPeriodTName);

            if (query.StudentId.HasValue)
            {
                dbQuery.Parameters.Add("studentId", query.StudentId);
                dbQuery.Sql.AppendFormat(" and [{0}].[{1}] in (select [{2}].[{4}] from [{2}] where [{2}].[{3}]  = @studentId)"
                    , classPeriodTName, ClassPeriod.CLASS_REF_FIELD, "ClassPerson", ClassPerson.PERSON_REF_FIELD, ClassPerson.CLASS_REF_FIELD);
            }
            if (query.TeacherId.HasValue)
            {
                dbQuery.Parameters.Add("teacherId", query.TeacherId);
                dbQuery.Sql.AppendFormat(" and [{0}].[{1}] in (select [{2}].[{4}] from [{2}] where [{2}].[{3}] = @teacherId)"
                    , classPeriodTName, ClassPeriod.CLASS_REF_FIELD, "Class", Class.TEACHER_REF_FIELD, Class.ID_FIELD);
            }

            if (query.SchoolYearId.HasValue)
            {
                dbQuery.Parameters.Add(Period.SCHOOL_YEAR_REF, query.SchoolYearId);
                dbQuery.Sql.AppendFormat(" and [{0}].[{1}] = @{1}", "Period", Period.SCHOOL_YEAR_REF);
            }

            if (query.Time.HasValue)
            {
                dbQuery.Parameters.Add("time", query.Time);
                dbQuery.Sql.AppendFormat(" and [{0}].[{1}] <= @time and [{0}].[{2}] >= @time"
                    , "Period", Period.START_TIME_FIELD, Period.END_TIME_FIELD);
            }
            if (query.ClassIds != null && query.ClassIds.Count > 0)
            {
                var classIdsParams = new List<string>();
                for (int i = 0; i < query.ClassIds.Count; i++)
                {
                    var classIdParam = "@classId_" + i;
                    classIdsParams.Add(classIdParam);
                    dbQuery.Parameters.Add(classIdParam, query.ClassIds[i]);
                }
                dbQuery.Sql.AppendFormat(" and [{0}].[{1}] in ({2})", classPeriodTName
                    , ClassPeriod.CLASS_REF_FIELD,  classIdsParams.JoinString(","));
            }
            return dbQuery;
        }

    }

    public class ClassPeriodQuery
    {
        public int? MarkingPeriodId { get; set; }
        public List<int> ClassIds { get; set; }
        public int? RoomId { get; set; }
        public int? PeriodId { get; set; }
        public int? StudentId { get; set; }
        public int? TeacherId { get; set; }
        public int? DateTypeId { get; set; }
        public int? Time { get; set; }
        public int? SchoolYearId { get; set; }
    }
}
