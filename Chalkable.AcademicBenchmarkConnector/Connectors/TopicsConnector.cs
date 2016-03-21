using System;
using System.Collections.Specialized;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Chalkable.AcademicBenchmarkConnector.Models;
using Chalkable.Common;

namespace Chalkable.AcademicBenchmarkConnector.Connectors
{
    public interface ITopicsConnector
    {
        Task<PaginatedList<Topic>> GetTopics(Guid? subjectDocId, Guid? courseId, Guid? parentId, bool? deepest, string searchQuery, int start, int count);
        Task<TopicStandards> GetTopicStandards(Guid topicId);
        Task<Topic> GetTopic(Guid id);
    }

    public class TopicsConnector : ConnectorBase, ITopicsConnector
    {
        public TopicsConnector(IConnectorLocator connectorLocator) : base(connectorLocator)
        {
        }

        public async Task<Topic> GetTopic(Guid id)
        {
            var responce = await GetOne<BaseResource<Topic>>($"topics/{id}", null);
            return responce?.Data;
        }

        public async Task<PaginatedList<Topic>> GetTopics(Guid? subjectDocId, Guid? courseId, Guid? parentId, bool? deepest, string searchQuery, int start, int count)
        {
            var nvc = new NameValueCollection();
            if(subjectDocId.HasValue)
                nvc.Add("subject_doc", subjectDocId.Value.ToString());
            if(courseId.HasValue)
                nvc.Add("course", courseId.Value.ToString());
            if(parentId.HasValue)
                nvc.Add("parent", parentId.Value.ToString());
            if(!string.IsNullOrWhiteSpace(searchQuery))
                nvc.Add("query", searchQuery);
            if(deepest.HasValue)
                nvc.Add("deepest", deepest.Value ? "Y" : "N");
            
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
