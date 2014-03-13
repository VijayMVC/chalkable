using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            if(!query.AllStandards || query.ParentStandardId.HasValue)
                condition.Add(Standard.PARENT_STANDARD_REF_FIELD, query.ParentStandardId);

            var dbQuery = new DbQuery();
            dbQuery.Sql.Append("select [Standard].* from [Standard]");
            condition.BuildSqlWhere(dbQuery, "Standard");

            if (query.ClassId.HasValue || query.CourseId.HasValue)
            {
                dbQuery.Sql.AppendFormat("and [{0}].[{1}] in (", "Standard", Standard.ID_FIELD);
                dbQuery.Sql.AppendFormat(@"select [{0}].[{1}] from [{0}] 
                                           join [{3}] on [{3}].[{4}] = [{0}].[{2}] 
                                           where 1=1 ", "ClassStandard", ClassStandard.STANDARD_REF_FIELD
                                                      , ClassStandard.CLASS_REF_FIELD, "Class", Class.ID_FIELD);
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
        } 
    }

    public class StandardSubjectDataAccess : DataAccessBase<StandardSubject, int>
    {
        public StandardSubjectDataAccess(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }

    public class ClassStandardDataAccess: DataAccessBase<ClassStandard, int>
    {
        public ClassStandardDataAccess(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }

    public class AnnouncementStandardDataAccess : DataAccessBase<AnnouncementStandard, int>
    {
        public AnnouncementStandardDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Delete(int announcementId, int standardId)
        {
            SimpleDelete<AnnouncementStandard>(new AndQueryCondition
                {
                    {AnnouncementStandard.ANNOUNCEMENT_REF_FIELD, announcementId},
                    {AnnouncementStandard.STANDARD_REF_FIELD, standardId}
                });
        }
    }

    public class StandardQuery
    {
        public int? CourseId { get; set; }
        public int? ClassId { get; set; }
        public int? GradeLavelId { get; set; }
        public int? StandardSubjectId { get; set; }
        public int? ParentStandardId { get; set; }
        public bool AllStandards { get; set; }
    }
}
