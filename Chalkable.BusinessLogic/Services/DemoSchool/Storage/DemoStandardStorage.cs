using System.Collections.Generic;
using System.Linq;
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

        public IList<Standard> GetStandarts(StandardQuery query)
        {
            var standards = data.Select(x => x.Value);
            if (query.StandardSubjectId.HasValue)
                standards = standards.Where(x => x.StandardSubjectRef == query.StandardSubjectId);
            if (query.GradeLavelId.HasValue)
                standards =
                    standards.Where(
                        x => query.GradeLavelId <= x.LowerGradeLevelRef && query.GradeLavelId >= x.UpperGradeLevelRef);
            if (!query.AllStandards || query.ParentStandardId.HasValue)
                standards = standards.Where(x => x.ParentStandardRef == query.ParentStandardId);
             

            /*
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
            return standards.ToList();
        }

        public void Update(IList<Standard> standards)
        {
            foreach (var standard in standards)
            {
                if (data.ContainsKey(standard.Id))
                    data[standard.Id] = standard;
            }
        }

        public override void Setup()
        {
            throw new System.NotImplementedException();
        }
    }
}
