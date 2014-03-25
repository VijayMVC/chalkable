﻿using System.Collections.Generic;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoStandardStorage:BaseDemoStorage<int ,Standard>
    {
        public DemoStandardStorage(DemoStorage storage) : base(storage)
        {
        }

        public void AddStandards(IList<Standard> standards)
        {
            foreach (var standard in standards)
            {
                if (!data.ContainsKey(standard.Id))
                    data[standard.Id] = standard;
            }
        }

        public IList<Standard> GetStandarts(StandardQuery standardQuery)
        {

            throw new System.NotImplementedException();
            /*
            var condition = new AndQueryCondition();
            if (query.StandardSubjectId.HasValue)
                condition.Add(Standard.STANDARD_SUBJECT_ID_FIELD, query.StandardSubjectId);
            if (query.GradeLavelId.HasValue)
            {
                condition.Add(Standard.LOWER_GRADE_LEVEL_REF_FIELD, query.GradeLavelId, ConditionRelation.LessEqual);
                condition.Add(Standard.UPPER_GRADE_LEVEL_REF_FIELD, query.GradeLavelId, ConditionRelation.GreaterEqual);
            }
            if (!query.AllStandards || query.ParentStandardId.HasValue)
                condition.Add(Standard.PARENT_STANDARD_REF_FIELD, query.ParentStandardId);

            var dbQuery = new DbQuery();
            dbQuery.Sql.Append("select [Standard].* from [Standard]");
            condition.BuildSqlWhere(dbQuery, "Standard");

            if (query.ClassId.HasValue || query.CourseId.HasValue)
            {
                dbQuery.Sql.AppendFormat("and [{0}].[{1}] in (", "Standard", Standard.ID_FIELD);
                dbQuery.Sql.AppendFormat(@"select [{0}].[{1}] from [{0}] 
                                           join [{3}] on [{3}].[{4}] = [{0}].[{2}] or [{3}].[{5}] = [{0}].[{2}]
                                           where 1=1 ", "ClassStandard", ClassStandard.STANDARD_REF_FIELD
                                                      , ClassStandard.CLASS_REF_FIELD, "Class", Class.ID_FIELD
                                                      , Class.COURSE_REF_FIELD);
                if (query.CourseId.HasValue)
                {
                    dbQuery.Parameters.Add("courseId", query.CourseId);
                    dbQuery.Sql.AppendFormat(" and ([{0}].[{1}] = @courseId or ([{0}].[{2}] =@courseId and [{0}].[{1}] is null))"
                        , "Class", Class.COURSE_REF_FIELD, Class.ID_FIELD);
                }
                if (query.ClassId.HasValue)
                {
                    dbQuery.Parameters.Add("classId", query.ClassId);
                    dbQuery.Sql.AppendFormat(" and ([{0}].[{1}] = @classId and [{0}].[{2}] is not null) ", "Class",
                                             Class.ID_FIELD, Class.COURSE_REF_FIELD);
                }
                dbQuery.Sql.AppendFormat(")");
            }
            return ReadMany<Standard>(dbQuery);
             */
        }

        public void Update(IList<Standard> standards)
        {
            foreach (var standard in standards)
            {
                if (data.ContainsKey(standard.Id))
                    data[standard.Id] = standard;
            }
        }
    }
}
