using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Chalkable.AcademicBenchmarkConnector.Models;
using Chalkable.Common;

namespace Chalkable.AcademicBenchmarkConnector.Connectors
{
    public interface ITopicsConnector
    {
        Task<PaginatedList<Topic>> GetTopics(Guid? subjectDocId, Guid? courseId, Guid? parentId, bool? deepest, string searchQuery, int start, int count);
        Task<Topic> GetTopic(Guid id);
        Task<IList<SubjectDocument>> GetSubjectDocuments();
        Task<IList<Course>> GetCourses(Guid? subjectDocumentId);
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
        
        public async Task<IList<SubjectDocument>> GetSubjectDocuments()
        {
            return (await GetPage<SubjectDocumentWrapper>()).Select(x => x.SubjectDocument).ToList();
        }

        public async Task<IList<Course>> GetCourses(Guid? subjectDocumentId)
        {
            return (await GetPage<CourseWrapper>(subjectDocumentId)).Select(x => x.Course).ToList();
        }

        public async Task<PaginatedList<Topic>> GetTopics(Guid? subjectDocId, Guid? courseId, Guid? parentId, bool? deepest, string searchQuery, int start, int count)
        {
            return await GetPage<Topic>(subjectDocId, courseId, parentId, deepest, searchQuery, start, count);
        }

        private static IDictionary<Type, string> _typesDic = new Dictionary<Type, string>
        {
            [typeof(Topic)] = null,
            [typeof(SubjectDocumentWrapper)] = "subject_doc",
            [typeof(CourseWrapper)] = "course"
        };
        protected async Task<PaginatedList<TModel>> GetPage<TModel>(Guid? subjectDocId = null, Guid? courseId = null,
            Guid? parentId = null, bool? deepest = null, string searchQuery = null, int offset = 0, int limit = int.MaxValue)
        {
            var nvc = new NameValueCollection
            {
                ["list"] = _typesDic[typeof(TModel)]
            };
            if (subjectDocId.HasValue)
                nvc.Add("subject_doc", subjectDocId.Value.ToString());
            if (courseId.HasValue)
                nvc.Add("course", courseId.Value.ToString());
            if (parentId.HasValue)
                nvc.Add("parent", parentId.Value.ToString());
            if (!string.IsNullOrWhiteSpace(searchQuery))
                nvc.Add("query", searchQuery);
            if (deepest.HasValue)
                nvc.Add("deepest", deepest.Value ? "Y" : "N");
            var res = await GetPage<BaseResource<TModel>>("topics", nvc, offset, limit);
            return res.Transform(x => x.Data);
        }
    }
}
