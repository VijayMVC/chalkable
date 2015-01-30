using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class StandardDataAccess : DataAccessBase<Standard, int>
    {
        public StandardDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Delete(IList<int> ids)
        {
            if (ids  != null && ids.Count > 0)
            {
                SimpleDelete<Standard>(ids.Select(x => new Standard { Id = x }).ToList());         
            }
        }

        public override void Delete(int key)
        {
            var q = new DbQuery(new List<DbQuery>
                {
                    Orm.SimpleDelete<AnnouncementStandard>(new AndQueryCondition { {AnnouncementStandard.STANDARD_REF_FIELD, key} }),
                    Orm.SimpleDelete<ClassStandard>(new AndQueryCondition {{ClassStandard.STANDARD_REF_FIELD, key}}),
                    Orm.SimpleDelete<Standard>(new AndQueryCondition {{Standard.ID_FIELD, key}})
                });
            ExecuteNonQueryParametrized(q.Sql.ToString(), q.Parameters);
        }

        public Standard GetStandardByABId(Guid id)
        {
            return SelectOne<Standard>(new AndQueryCondition
                {
                    {Standard.ACADEMIC_BENCHMARK_ID_FIELD, id}
                });
        }

        public IList<Standard> GetStandards(StandardQuery query)
        {
            var condition = new AndQueryCondition();
            if(query.StandardSubjectId.HasValue)
                condition.Add(Standard.STANDARD_SUBJECT_ID_FIELD, query.StandardSubjectId);
            if (query.GradeLavelId.HasValue)
            {
                condition.Add(Standard.LOWER_GRADE_LEVEL_REF_FIELD, query.GradeLavelId, ConditionRelation.LessEqual);
                condition.Add(Standard.UPPER_GRADE_LEVEL_REF_FIELD, query.GradeLavelId, ConditionRelation.GreaterEqual);
            }
            if(query.ParentStandardId.HasValue)
                condition.Add(Standard.PARENT_STANDARD_REF_FIELD, query.ParentStandardId);

            var dbQuery = new DbQuery();
            dbQuery.Sql.Append(@"select [Standard].* from [Standard]");
            condition.BuildSqlWhere(dbQuery, "Standard");

            if (query.ClassId.HasValue)
            {
                dbQuery.Parameters.Add("classId", query.ClassId);
              
                dbQuery.Sql.AppendFormat("and [{0}].[{1}] in (", "Standard", Standard.ID_FIELD);

                var subQuery = string.Format(@"select [{0}].[{1}] from [{0}] 
                                               join [{3}] on [{3}].[{4}] = [{0}].[{2}] or [{3}].[{5}] = [{0}].[{2}]
                                               where [{3}].[{4}] = @classId and [{3}].[{5}] is not null", 
                                               "ClassStandard", ClassStandard.STANDARD_REF_FIELD
                                             , ClassStandard.CLASS_REF_FIELD, "Class", Class.ID_FIELD
                                             , Class.COURSE_REF_FIELD);
                dbQuery.Sql.Append(subQuery).Append(")");

                if (!query.ParentStandardId.HasValue && !query.AllStandards)
                {
                    dbQuery.Sql.AppendFormat(" and ([{0}].[{1}] is null or [{0}].[{1}] not in (", "Standard",
                                             Standard.PARENT_STANDARD_REF_FIELD)
                                .Append(subQuery).Append("))");
                }
            }
            return ReadMany<Standard>(dbQuery);
        }
    }

    public class StandardSubjectDataAccess : DataAccessBase<StandardSubject, int>
    {
        public StandardSubjectDataAccess(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public void Delete(IList<int> ids)
        {
            SimpleDelete(ids.Select(x => new StandardSubject {Id = x}).ToList());
        }

        public IList<StandardSubject> GetStandardSubjectByClass(int classId)
        {
            var dbQuery = new DbQuery();
            var classT = typeof (Class);
            dbQuery.Sql.AppendFormat(@"select  distinct [{0}].* from [{0}]
                                       join [{1}] on [{1}].StandardSubjectRef = [{0}].Id
                                       join [{2}] on [{2}].StandardRef = Standard.Id
                                       join [{3}] on [{3}].Id = [{2}].ClassRef or [{3}].CourseRef = [{2}].ClassRef "
                                     , typeof (StandardSubject).Name, typeof (Standard).Name, typeof (ClassStandard).Name,
                                     classT.Name);
            var conds = new AndQueryCondition {{Class.ID_FIELD, classId}};
            conds.BuildSqlWhere(dbQuery, classT.Name);
            return ReadMany<StandardSubject>(dbQuery);
        } 
    }

    public class ClassStandardDataAccess: DataAccessBase<ClassStandard, int>
    {
        public ClassStandardDataAccess(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        public void Delete(IList<ClassStandard> classStandards)
        {
            SimpleDelete(classStandards);
        }
    }

    public class AnnouncementStandardDataAccess : DataAccessBase<AnnouncementStandard, int>
    {
        public AnnouncementStandardDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Delete(int announcementId, int standardId)
        {
            SimpleDelete(new AndQueryCondition
                {
                    {AnnouncementStandard.ANNOUNCEMENT_REF_FIELD, announcementId},
                    {AnnouncementStandard.STANDARD_REF_FIELD, standardId}
                });
        }

        public void DeleteAll(int standardId)
        {
            SimpleDelete(new AndQueryCondition
                {
                    {AnnouncementStandard.STANDARD_REF_FIELD, standardId}
                });
        }
        
        public void Delete(QueryCondition annCondition, QueryCondition classCondition, bool notInClassStandard)
        {
            var cStandardDbQuery = BuildClassStandardQuery(new List<string> {ClassStandard.STANDARD_REF_FIELD}, classCondition);
            var dbQuery = new DbQuery();
            dbQuery.Sql.AppendFormat(@"delete from AnnouncementStandard ");
            annCondition.BuildSqlWhere(dbQuery, "AnnouncementStandard");
            dbQuery.Sql.AppendFormat(" and AnnouncementStandard.[{0}] {2} in ({1})"
                , AnnouncementStandard.STANDARD_REF_FIELD, cStandardDbQuery.Sql, notInClassStandard ? "not" : "");
            foreach (var parameter in cStandardDbQuery.Parameters)
                dbQuery.Parameters.Add(parameter);
            ExecuteNonQueryParametrized(dbQuery.Sql.ToString(), dbQuery.Parameters);
        }

        private DbQuery BuildClassStandardQuery(IEnumerable<string> columnsResList, QueryCondition classCondition)
        {
            var res = new DbQuery();
            res.Sql.AppendFormat(@"select {0}
                                   from ClassStandard 
                                   join [Class] on [Class].[{1}] = ClassStandard.[{2}] or ClassStandard.[{2}] = Class.[{3}]"
                                , columnsResList.Select(x => "ClassStandard.[" + x + "]").JoinString(",")
                                , Class.ID_FIELD, ClassStandard.CLASS_REF_FIELD, Class.COURSE_REF_FIELD);
            classCondition.BuildSqlWhere(res, "Class");
            return res;
        }

        public IList<AnnouncementStandard> GetAnnouncementStandards(int classId)
        {
            var cond = new AndQueryCondition {{Class.ID_FIELD, classId}};
            var classStDbQuery = BuildClassStandardQuery(new List<string> {ClassStandard.STANDARD_REF_FIELD}, cond);
            var dbQuery = new DbQuery();
            dbQuery.Sql.AppendFormat(@"select * from AnnouncementStandard where AnnouncementStandard.[{0}] in ({1})",
                AnnouncementStandard.STANDARD_REF_FIELD, classStDbQuery.Sql);
            dbQuery.Parameters = classStDbQuery.Parameters;
            return ReadMany<AnnouncementStandard>(dbQuery);
        }
    }

    public class StandardQuery
    {
        public int? ClassId { get; set; }
        public int? GradeLavelId { get; set; }
        public int? StandardSubjectId { get; set; }
        public int? ParentStandardId { get; set; }
        public bool AllStandards { get; set; }
    }
}
