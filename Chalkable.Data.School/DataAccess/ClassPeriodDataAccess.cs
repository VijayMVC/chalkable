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
            conds.Add(ClassPeriod.PERIOD_REF_FIELD, periodId);
            conds.Add(ClassPeriod.DAY_TYPE_REF_FIELD, dayTypeId);
            SimpleDelete(conds);
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
            dbQuery.Sql.AppendFormat(@"select distinct {0} from [{1}] 
                                       join [{2}] on [{2}].[{3}] = [{1}].[{4}]
                                       join [{5}] on [{5}].[{6}] = [{1}].[{7}] "
                                     , Orm.ComplexResultSetQuery(types), types[0].Name
                                     , types[1].Name, Period.ID_FIELD, ClassPeriod.PERIOD_REF_FIELD
                                     , typeof(MarkingPeriodClass).Name, MarkingPeriodClass.CLASS_REF_FIELD, ClassPeriod.CLASS_REF_FIELD);
            return BuildGetClassPeriodsConditions(dbQuery, query);
        }
       
        private DbQuery BuildGetClassPeriodsConditions(DbQuery dbQuery, ClassPeriodQuery query)
        {
            var conds = new AndQueryCondition();
            var classPeriodTName = "ClassPeriod";
            if (query.PeriodId.HasValue)
                conds.Add(ClassPeriod.PERIOD_REF_FIELD, query.PeriodId);
            if (query.DateTypeId.HasValue)
                conds.Add(ClassPeriod.DAY_TYPE_REF_FIELD, query.DateTypeId);

            FilterBySchool(conds).BuildSqlWhere(dbQuery, classPeriodTName);

            if (query.RoomId.HasValue)
            {
                conds.Add("roomId", query.RoomId);
                dbQuery.Sql.AppendFormat(" and [{0}].[{1}] in (select [{2}].[{4}] from [{2}] where [{2}].[{3}] = @roomId)"
                    , classPeriodTName, ClassPeriod.CLASS_REF_FIELD, "Class", Class.ROOM_REF_FIELD, Class.ID_FIELD);
            }
            
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
                    , classPeriodTName, ClassPeriod.CLASS_REF_FIELD, "ClassTeacher", ClassTeacher.PERSON_REF_FIELD, ClassTeacher.CLASS_REF_FIELD);
            }

            if (query.SchoolYearId.HasValue)
            {
                dbQuery.Parameters.Add(Period.SCHOOL_YEAR_REF, query.SchoolYearId);
                dbQuery.Sql.AppendFormat(" and [{0}].[{1}] = @{1}", "Period", Period.SCHOOL_YEAR_REF);
            }

            if (query.MarkingPeriodId.HasValue)
            {
                dbQuery.Parameters.Add(MarkingPeriodClass.MARKING_PERIOD_REF_FIELD, query.MarkingPeriodId);
                dbQuery.Sql.AppendFormat(" and [{0}].[{1}] = @{1}", typeof(MarkingPeriodClass).Name, MarkingPeriodClass.MARKING_PERIOD_REF_FIELD);
            }

            if (query.Time.HasValue)
            {
                dbQuery.Parameters.Add("time", query.Time);
                dbQuery.Sql.AppendFormat(" and [{0}].[{1}] <= @time and [{0}].[{2}] >= @time"
                    , "Period", "StartTime", "EndTime");
                throw new NotImplementedException();
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

        public Class CurrentClassForTeacher(int schoolYearId, int personId, DateTime date, int time)
        {
            IDictionary<string, object> ps = new Dictionary<string, object>
            {
                {"@schoolYearId", schoolYearId},
                {"@teacherId", personId},
                {"@date", date},
                {"@time", time},
            };
            using (var reader = ExecuteStoredProcedureReader("spCurrentClassForTeacher", ps))
            {
                return reader.ReadOrNull<Class>();
            }
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
