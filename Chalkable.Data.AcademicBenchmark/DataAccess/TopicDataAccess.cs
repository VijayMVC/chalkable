using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Common;
using Chalkable.Data.AcademicBenchmark.Model;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;

namespace Chalkable.Data.AcademicBenchmark.DataAccess
{
    public class TopicDataAccess : DataAccessBase<Topic, Guid>
    {
        public TopicDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<Topic> Get(Guid? subjectDocId, Guid? courseId, Guid? parentId, bool firstLevelOnly = false)
        {
            AndQueryCondition conditions = new AndQueryCondition();
            if (subjectDocId.HasValue)
                conditions.Add(nameof(Topic.SubjectDocRef), subjectDocId, ConditionRelation.Equal);

            if (parentId.HasValue)
                conditions.Add(nameof(Topic.ParentRef), parentId, ConditionRelation.Equal);

            if (courseId.HasValue)
                conditions.Add(nameof(Topic.CourseRef), courseId, ConditionRelation.Equal);

            if (firstLevelOnly)
                conditions.Add(nameof(Topic.Level), 1, ConditionRelation.Equal);
            
            var query = Orm.SimpleSelect(nameof(Topic), conditions);

            return ReadMany<Topic>(query);
        }

        public PaginatedList<Topic> Search(string searchQuery, bool? deepest, int start, int count)
        {
            var @params = new Dictionary<string, object>
            {
                ["searchQuery"] = searchQuery,
                ["deepest"] = deepest,
                ["start"] = start,
                ["count"] = count
            };

            return ExecuteStoredProcedurePaginated<Topic>("spSearchTopics", @params, start, count);
        }
    }
}
