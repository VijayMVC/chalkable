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
                    {Standard.ACADEMIC_BENCHMARK_ID_FIELD, id}
                });
        }
        public IList<Standard> GetStandardsByABIds(IList<Guid> ids)
        {
            var dbQuery = Orm.SimpleSelect<Standard>(new AndQueryCondition ());
            var idsStr = ids.Select(x => string.Format("'{0}'", x)).JoinString(",");
            dbQuery.Sql.AppendFormat(" and [{0}] in ({1})", Standard.ACADEMIC_BENCHMARK_ID_FIELD, idsStr);
            return ReadMany<Standard>(dbQuery);
        }

        public IList<Standard> GetStandardsByIds(IList<int> ids)
        {
            var dbQuery = new DbQuery();
            dbQuery.Sql.AppendFormat("select * from [{0}] where [{0}].[{1}] in ({2})"
                , typeof(Standard).Name, Standard.ID_FIELD, ids.JoinString(","));
            return ReadMany<Standard>(dbQuery);
        }

        public IList<Standard> GetStandards(StandardQuery query)
        {
            var condition = new AndQueryCondition();
            if (query.StandardSubjectId.HasValue)
                condition.Add(Standard.STANDARD_SUBJECT_ID_FIELD, query.StandardSubjectId);
            if (query.GradeLavelId.HasValue)
            {
                condition.Add(Standard.LOWER_GRADE_LEVEL_REF_FIELD, query.GradeLavelId, ConditionRelation.LessEqual);
                condition.Add(Standard.UPPER_GRADE_LEVEL_REF_FIELD, query.GradeLavelId, ConditionRelation.GreaterEqual);
            }
            if (query.ParentStandardId.HasValue)
                condition.Add(Standard.PARENT_STANDARD_REF_FIELD, query.ParentStandardId);
            if(query.ActiveOnly)
                condition.Add(Standard.IS_ACTIVE_FIELD, true);

            var dbQuery = new DbQuery();
            dbQuery.Sql.Append(@"select [Standard].* from [Standard]");
            condition.BuildSqlWhere(dbQuery, "Standard");

            if (query.ClassId.HasValue)
            {
                dbQuery.Parameters.Add("classId", query.ClassId);

                dbQuery.Sql.AppendFormat("and [{0}].[{1}] in (", "Standard", Standard.ID_FIELD);

                var subQuery = BuildClassStandardSubQuery("classId");
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

        public IList<Standard> SearchStandards(string filter, bool activeOnly = false)
        {
            if (string.IsNullOrEmpty(filter)) return new List<Standard>();
            var words = filter.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length == 0)
                return new List<Standard>();

            var conds = new AndQueryCondition();
            if(activeOnly)
                conds.Add(Standard.IS_ACTIVE_FIELD, true);
            var dbQuery = Orm.SimpleSelect<Standard>(conds);
            dbQuery.Sql.Append(" and (");
            for (int i = 0; i < words.Length; i++)
            {
                if (i > 0) dbQuery.Sql.Append(" or ");
                var param = string.Format("@word{0}", i + 1);
                dbQuery.Sql.AppendFormat(" [{0}] like {2} or [{1}] like {2}", Standard.NAME_FIELD,
                                         Standard.DESCRIPTION_FIELD, param);
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

        public StandardTreePath GetStandardParentsSubTree(int standardId, int? classId)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@standardId", standardId },
                { "@classId", classId}
            };
            using (var reader = ExecuteStoredProcedureReader("spGetStandardParentsWithChilds", parameters))
            {
                IList<Standard> standards = new List<Standard>();
                IList<Standard> path = new List<Standard>();
                IList<int> parentIds = new List<int>();
                while (reader.Read())
                {
                    parentIds.Add(SqlTools.ReadInt32(reader, "id"));
                }
                parentIds = parentIds.Reverse().ToList();
                if (parentIds.Count > 0)
                {
                    reader.NextResult();
                    standards = reader.ReadList<Standard>();
                    
                    foreach (var parentId in parentIds)
                    {
                        path.Add(standards.First(s => s.Id == parentId));
                    }
               }
               return new StandardTreePath { AllStandards = standards, Path = path.ToList() };
            }
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
                   .AppendFormat(Orm.SIMPLE_JOIN_FORMAT, types[1].Name, Standard.ID_FIELD,
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
