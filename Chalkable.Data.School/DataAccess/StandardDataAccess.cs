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
        public StandardDataAccess(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }


        public Standard GetStandardByABId(Guid id)
        {
            return SelectOneOrNull<Standard>(new AndQueryCondition
            {
                {nameof(Standard.AcademicBenchmarkId), id}
            });
        }

        public IList<Standard> GetStandardsByABIds(IList<Guid> ids)
        {
            var dbQuery = Orm.SimpleSelect<Standard>(new AndQueryCondition());
            var idsStr = ids.Select(x => $"'{x}'").JoinString(",");
            dbQuery.Sql.AppendFormat(" and [{0}] in ({1})", nameof(Standard.AcademicBenchmarkId), idsStr);
            return ReadMany<Standard>(dbQuery);
        }

        public IList<Standard> GetStandardsByIds(IList<int> ids)
        {
            if(ids == null || ids.Count == 0)
                return new List<Standard>();
            var dbQuery = new DbQuery();
            dbQuery.Sql.AppendFormat("select * from [{0}] where [{0}].[{1}] in ({2})"
                , typeof (Standard).Name, nameof(Standard.Id), ids.JoinString(","));
            return ReadMany<Standard>(dbQuery);
        }
        //TODO: move this to stored procedure
        public IList<Standard> GetStandards(StandardQuery query)
        {
            var condition = new AndQueryCondition();
            if (query.StandardSubjectId.HasValue)
                condition.Add(nameof(Standard.StandardSubjectRef), query.StandardSubjectId);
            if (query.GradeLavelId.HasValue)
            {
                condition.Add(nameof(Standard.LowerGradeLevelRef), query.GradeLavelId, ConditionRelation.LessEqual);
                condition.Add(nameof(Standard.UpperGradeLevelRef), query.GradeLavelId, ConditionRelation.GreaterEqual);
            }
            if (query.ParentStandardId.HasValue)
                condition.Add(nameof(Standard.ParentStandardRef), query.ParentStandardId);
            if (query.ActiveOnly)
                condition.Add(nameof(Standard.IsActive), true);

            var dbQuery = BuildSelectQuery(condition);
            
            if (query.ClassId.HasValue)
            {
                dbQuery.Parameters.Add("classId", query.ClassId);

                dbQuery.Sql.AppendFormat("and [{0}].[{1}] in (", "Standard", nameof(Standard.Id));

                var subQuery = BuildClassStandardSubQuery("classId");
                dbQuery.Sql.Append(subQuery).Append(")");

                if (!query.ParentStandardId.HasValue && !query.AllStandards)
                {
                    dbQuery.Sql.AppendFormat(" and ([{0}].[{1}] is null or [{0}].[{1}] not in (", "Standard",
                        nameof(Standard.ParentStandardRef))
                        .Append(subQuery).Append("))");
                }
            }
            return ReadMany<Standard>(dbQuery);
        }

        private DbQuery BuildSelectQuery(QueryCondition condition)
        {
            var dbQuery = new DbQuery();
            condition = condition ?? new AndQueryCondition();
            //TODO make a view later
            dbQuery.Sql.Append($" Select [{nameof(Standard)}].*,  ")
                                // calculating is deepest(or leaf) standard
                                .Append($" cast((case when exists(Select * From  [{nameof(Standard)}]  innerSt ")
                                .Append($" Where innerSt.{nameof(Standard.ParentStandardRef)} = [{nameof(Standard)}].{nameof(Standard.Id)}) ")
                                .Append($" then 0 else 1 end) as bit) as [{nameof(Standard.IsDeepest)}] ")
                       .Append($" From [{nameof(Standard)}] ");
            condition.BuildSqlWhere(dbQuery, nameof(Standard));
            return dbQuery;
        }

        public IList<Standard> SearchStandards(string filter, bool activeOnly = false)
        {
            if (string.IsNullOrEmpty(filter)) return new List<Standard>();
            var words = filter.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length == 0)
                return new List<Standard>();

            var conds = new AndQueryCondition();
            if (activeOnly)
                conds.Add(nameof(Standard.IsActive), true);
            var dbQuery = BuildSelectQuery(conds);
            dbQuery.Sql.Append(" and (");
            for (int i = 0; i < words.Length; i++)
            {
                if (i > 0) dbQuery.Sql.Append(" or ");
                var param = $"@word{i + 1}";
                dbQuery.Sql.AppendFormat(" [{0}] like {2} or [{1}] like {2}", nameof(Standard.Name),
                    nameof(Standard.Description), param);
                dbQuery.Parameters.Add(param, "%" + words[i] + "%");
            }
            dbQuery.Sql.Append(")");
            return ReadMany<Standard>(dbQuery);
        }

        private string BuildClassStandardSubQuery(string classIdParamName)
        {
            return string.Format(@"select [{0}].[{1}] from [{0}] 
                                    join [{3}] on [{3}].[{4}] = [{0}].[{2}] or [{3}].[{5}] = [{0}].[{2}]
                                    where [{3}].[{4}] = @{6} and [{3}].[{5}] is not null",
                "ClassStandard", ClassStandard.STANDARD_REF_FIELD
                , ClassStandard.CLASS_REF_FIELD, "Class", Class.ID_FIELD
                , Class.COURSE_REF_FIELD, classIdParamName);
        }

        public IList<StandardTreeItem> GetStandardParentsSubTree(int standardId, int? classId)
        {
            var parameters = new Dictionary<string, object>
            {
                {"@CurrentClassId", classId},
                {"@StandardIdForSearch", standardId}
            };
            return ExecuteStoredProcedureList<StandardTreeItem>("spGetStandardParentsSubTree", parameters); 
        }

        public IList<Standard> GetGridStandardsByPacing(int? classId, int? gradeLevelId, int? subjectId, int? parentStandardId = null, bool allStandards = true, bool activeOnly = false)
        {
            var parameters = new Dictionary<string, object>
            {
                {"@ClassId", classId},
                {"@GradeLavelId", gradeLevelId},
                {"@StandardSubjectId", subjectId},
                {"@ParentStandardId", parentStandardId},
                {"@AllStandards", allStandards},
                {"@IsActive", activeOnly}
            };
            return ExecuteStoredProcedureList<Standard>("spGetGridStandardsByPacing", parameters);
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

    public class AnnouncementStandardDataAccess : DataAccessBase<AnnouncementStandard, int>
    {
        public AnnouncementStandardDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Delete(int? announcementId, int? standardId)
        {
            var conds = new AndQueryCondition();
            if (announcementId.HasValue)
                conds.Add(nameof(AnnouncementStandard.AnnouncementRef), announcementId.Value);
            if (standardId.HasValue)
                conds.Add(nameof(AnnouncementStandard.StandardRef), standardId.Value);

            SimpleDelete(conds);
        }
        
        public void DeleteNotAssignedToClass(int announcementId, int classId)
        {
            var annCond = new AndQueryCondition {{AnnouncementStandard.ANNOUNCEMENT_REF_FIELD, announcementId}};
            var classCond = new AndQueryCondition {{Class.ID_FIELD, classId}};
            Delete(annCond, classCond, true);
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

        public IList<AnnouncementStandard> GetAnnouncementStandardsByClassId(int classId)
        {
            var cond = new AndQueryCondition {{Class.ID_FIELD, classId}};
            var classStDbQuery = BuildClassStandardQuery(new List<string> {ClassStandard.STANDARD_REF_FIELD}, cond);
            var dbQuery = new DbQuery();
            dbQuery.Sql.AppendFormat(@"select * from AnnouncementStandard where AnnouncementStandard.[{0}] in ({1})",
                AnnouncementStandard.STANDARD_REF_FIELD, classStDbQuery.Sql);
            dbQuery.Parameters = classStDbQuery.Parameters;
            return ReadMany<AnnouncementStandard>(dbQuery);
        }

        public IList<AnnouncementStandardDetails> GetAnnouncementStandardsByAnnId(int announcementId)
        {
            var dbQuery = new DbQuery();
            var types = new List<Type> {typeof (AnnouncementStandard), typeof (Standard)};
            dbQuery.Sql
                   .AppendFormat(Orm.SELECT_FORMAT, Orm.ComplexResultSetQuery(types), types[0].Name).Append(" ")
                   .AppendFormat(Orm.SIMPLE_JOIN_FORMAT, types[1].Name, nameof(Standard.Id),
                                 types[0].Name, AnnouncementStandard.STANDARD_REF_FIELD)
                   .Append(" ");
            var conds = new AndQueryCondition { { AnnouncementStandard.ANNOUNCEMENT_REF_FIELD, announcementId } };
            conds.BuildSqlWhere(dbQuery, types[0].Name);
            return ReadMany<AnnouncementStandardDetails>(dbQuery, true);
        }
    }

    public class StandardQuery
    {
        public int? ClassId { get; set; }
        public int? GradeLavelId { get; set; }
        public int? StandardSubjectId { get; set; }
        public int? ParentStandardId { get; set; }
        public bool AllStandards { get; set; }
        public bool ActiveOnly { get; set; }

        public StandardQuery()
        {
            ActiveOnly = true;
        }
    }
}
