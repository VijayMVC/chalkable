using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Chalkable.AcademicBenchmarkConnector.Models;
using Chalkable.Common;

namespace Chalkable.AcademicBenchmarkConnector.Connectors
{
    public interface ITopicsConnector
    {
        Task<PaginatedList<Topic>> GetTopics(Guid? subject, string gradeLevel, Guid? parentId, string searchQuery, int start, int count);
        Task<TopicStandards> GetTopicStandards(Guid topicId);
    }

    public class TopicsConnector : ConnectorBase, ITopicsConnector
    {
        public TopicsConnector(IConnectorLocator connectorLocator) : base(connectorLocator)
        {
        }


        public async Task<PaginatedList<Topic>> GetTopics(Guid? subject, string gradeLevel, Guid? parentId, string searchQuery, int start, int count)
        {
            var nvc = new NameValueCollection();
            if(subject.HasValue)
                nvc.Add("subject", subject.Value.ToString());
            if(!string.IsNullOrWhiteSpace(gradeLevel))
                nvc.Add("grade", gradeLevel);
            if(parentId.HasValue)
                nvc.Add("parent", parentId.Value.ToString());
            if(!string.IsNullOrWhiteSpace(searchQuery))
                nvc.Add("query", searchQuery);

            var res = await GetPage<BaseResource<Topic>>("topics", nvc, start, count);
            return res.Transform(x => x.Data);
        }

        public async Task<TopicStandards> GetTopicStandards(Guid topicId)
        {
            var nvc = new NameValueCollection
            {
                ["group"] = "topics" 
            };
            var res = await GetOne<BaseResource<TopicStandards>>($"topic/{topicId}/standards", nvc);
            return res?.Data;
        }
    }
}
