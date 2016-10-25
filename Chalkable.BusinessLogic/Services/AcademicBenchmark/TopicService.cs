using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Model.AcademicBenchmark;
using Chalkable.Common;
using Chalkable.Data.AcademicBenchmark.DataAccess;
using Topic = Chalkable.Data.AcademicBenchmark.Model.Topic;

namespace Chalkable.BusinessLogic.Services.AcademicBenchmark
{
    public interface ITopicService : IAcademicBenchmarkServiceBase<Topic, Guid>
    {
        IList<Topic> Get(Guid? subjectDocId, Guid? courseId, Guid? parentId, bool firstLevelOnly = false);
        PaginatedList<Topic> SearchTopics(string searchQuery, bool? deepest, int start, int count);
    }

    public class TopicService : AcademicBenchmarkServiceBase<Topic, Guid>, ITopicService
    {
        public TopicService(IAcademicBenchmarkServiceLocator locator) : base(locator)
        {
        }

        public IList<Topic> Get(Guid? subjectDocId, Guid? courseId, Guid? parentId, bool firstLevelOnly = false)
        {
            return DoRead(u => new TopicDataAccess(u).Get(subjectDocId, courseId, parentId, firstLevelOnly));
        }

        public PaginatedList<Topic> SearchTopics(string searchQuery, bool? deepest, int start, int count)
        {
            return DoRead(u => new TopicDataAccess(u).Search(searchQuery, deepest, start, count));
        }
    }
}
