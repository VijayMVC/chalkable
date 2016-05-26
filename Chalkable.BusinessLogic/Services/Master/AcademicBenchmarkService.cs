using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chalkable.AcademicBenchmarkConnector.Connectors;
using Chalkable.BusinessLogic.Model.AcademicBenchmark;
using Chalkable.Common;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IAcademicBenchmarkService
    {
        Task<IList<Standard>> GetStandardsByIds(IList<Guid> standardsIds);
        Task<IList<StandardRelations>> GetListOfStandardRelations(IList<Guid> standardsIds);
        Task<IList<Authority>> GetAuthorities();
        Task<IList<Document>> GetDocuments(Guid? authorityId);
        Task<IList<SubjectDocument>> GetSubjectDocuments(Guid? authorityId, Guid? documentId);
        Task<IList<GradeLevel>> GetGradeLevels(Guid? authorityId, Guid? documentId, Guid? subjectDocId);
        Task<IList<Course>> GetCourses(Guid? authorityId, Guid? documentId, Guid? subjectDocId, string gradeLevelCode);
        Task<PaginatedList<Standard>> SearchStandards(string searchQuery, bool? deepest, int start = 0, int count = int.MaxValue);
        Task<IList<Standard>> GetStandards(Guid? authorityId, Guid? documentId, Guid? subjectDocId, string gradeLevelCode, Guid? parentId, Guid? courseId, bool firstLevelOnly = false);
        Task<IList<Topic>> GetTopics(Guid? subjectDocId, Guid? courseId, Guid? parentId, bool firstLevelOnly = false);
        Task<PaginatedList<Topic>> SearchTopics(string searchQuery, bool? deepest = null, int start = 0, int count = int.MaxValue);
        Task<IList<Topic>> GetTopicsByIds(IList<Guid> topicsIds);

        Task<IList<SubjectDocument>> GetTopicSubjectDocuments();
        Task<IList<Course>> GetTopicCourses(Guid? subjectDoucmentId);
    }

    public class AcademicBenchmarkService : MasterServiceBase, IAcademicBenchmarkService
    {
        private readonly ConnectorLocator _abConnectorLocator;
        public AcademicBenchmarkService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
            _abConnectorLocator = new ConnectorLocator();
        }

        public async Task<IList<Standard>> GetStandardsByIds(IList<Guid> standardsIds)
        {
            IList<Task<AcademicBenchmarkConnector.Models.Standard>> tasks = standardsIds.Select(stId => _abConnectorLocator.StandardsConnector.GetStandardById(stId)).ToList();
            var standards = await Task.WhenAll(tasks);
            standards = standards.Where(x => x != null).ToArray();
            return standards.Select(Standard.Create).ToList();
        }

        public async Task<IList<StandardRelations>> GetListOfStandardRelations(IList<Guid> standardsIds)
        {
            IList<Task<AcademicBenchmarkConnector.Models.StandardRelations>> tasks = standardsIds.Select(stId => _abConnectorLocator.StandardsConnector.GetStandardRelationsById(stId)).ToList();
            var standards = await Task.WhenAll(tasks);
            standards = standards.Where(x => x != null).ToArray();
            return standards.Select(StandardRelations.Create).ToList();
        }

        public async Task<IList<Authority>> GetAuthorities()
        {
            var authorities = await _abConnectorLocator.StandardsConnector.GetAuthorities();
            return authorities.Select(Authority.Create).OrderBy(x => x.Code).ThenBy(x=>x.Description).ToList();
        }

        public async Task<IList<Document>> GetDocuments(Guid? authorityId)
        {
            var docs = await _abConnectorLocator.StandardsConnector.GetDocuments(authorityId);
            return docs.Select(Document.Create).OrderBy(x => x.Description).ToList();
        }
        
        public async Task<IList<GradeLevel>> GetGradeLevels(Guid? authorityId, Guid? documentId, Guid? subjectDocId)
        {
            var grLevels = await _abConnectorLocator.StandardsConnector.GetGradeLevels(authorityId, documentId, subjectDocId);
            return grLevels.Select(GradeLevel.Create).ToList();
        }
        public async Task<IList<SubjectDocument>> GetSubjectDocuments(Guid? authorityId, Guid? documentId)
        {
            var subDocs = await _abConnectorLocator.StandardsConnector.GetSubjectDocuments(authorityId, documentId);
            return subDocs.Select(SubjectDocument.Create).OrderBy(x=>x.Description).ToList();
        }
        public async Task<IList<Course>> GetCourses(Guid? authorityId, Guid? documentId, Guid? subjectDocId, string gradeLevelCode)
        {
            var res = await _abConnectorLocator.StandardsConnector.GetCourses(authorityId, documentId, subjectDocId, gradeLevelCode);
            return res.Select(Course.Create).OrderBy(x=>x.Description).ToList();
        }

        public async Task<PaginatedList<Standard>> SearchStandards(string searchQuery, bool? deepest, int start = 0, int count = int.MaxValue)
        {
            var standards = await _abConnectorLocator.StandardsConnector.SearchStandards(searchQuery, deepest, start, count);
            return standards.Transform(Standard.Create);
        }
        

        public async Task<IList<Standard>> GetStandards(Guid? authorityId, Guid? documentId, Guid? subjectDocId, string gradeLevelCode, Guid? parentId, Guid? courseId, bool firstLevelOnly = false)
        {
            var deepest = firstLevelOnly ? false : (bool?)null;
            var standards = await _abConnectorLocator.StandardsConnector.GetStandards(authorityId, documentId, subjectDocId, gradeLevelCode, parentId, courseId, deepest);
            if (firstLevelOnly)
                standards = standards.Where(x => x.Level == 1).ToList();
            return standards.Select(Standard.Create).OrderBy(x=>x.Code).ThenBy(x=>x.Description).ToList();
        }

        public async Task<IList<Topic>> GetTopics(Guid? subjectDocId, Guid? courseId, Guid? parentId, bool firstLevelOnly = false)
        {
            var deepest = firstLevelOnly ? false : (bool?)null;
            var topics = await _abConnectorLocator.TopicsConnector.GetTopics(subjectDocId, courseId, parentId, deepest, null, 0, int.MaxValue);
            var res = topics.Select(Topic.Create).OrderBy(x => x.Description).ToList();
            return firstLevelOnly ? res.Where(x => x.Level == 1).ToList() : res;
        }

        public async Task<PaginatedList<Topic>> SearchTopics(string searchQuery, bool? deepest = null, int start = 0, int count = int.MaxValue)
        {
            var topics = await _abConnectorLocator.TopicsConnector.GetTopics(null, null, null, deepest, searchQuery, start, count);
            return topics.Transform(Topic.Create);
        }

        public async Task<IList<Topic>> GetTopicsByIds(IList<Guid> topicsIds)
        {
            var tasks = topicsIds.Select(x => _abConnectorLocator.TopicsConnector.GetTopic(x));
            var topics = await Task.WhenAll(tasks);
            return topics.Select(Topic.Create).Where(x => x != null).ToList();
        }

        public async Task<IList<SubjectDocument>> GetTopicSubjectDocuments()
        {
            var subDocs = await _abConnectorLocator.TopicsConnector.GetSubjectDocuments();
            return subDocs.Select(SubjectDocument.Create).OrderBy(x=>x.Description).ToList();
        }

        public async Task<IList<Course>> GetTopicCourses(Guid? subjectDoucmentId)
        {
            var res = await _abConnectorLocator.TopicsConnector.GetCourses(subjectDoucmentId);
            return res.Select(Course.Create).OrderBy(x=>x.Description).ToList();
        }
        
    }
}
