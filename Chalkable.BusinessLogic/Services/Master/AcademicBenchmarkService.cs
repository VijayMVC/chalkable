using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chalkable.AcademicBenchmarkConnector.Connectors;
using Chalkable.Common;

using Topic = Chalkable.BusinessLogic.Model.AcademicBenchmark.Topic;
using Subject = Chalkable.BusinessLogic.Model.AcademicBenchmark.Subject;
using Authority = Chalkable.BusinessLogic.Model.AcademicBenchmark.Authority;
using Document = Chalkable.BusinessLogic.Model.AcademicBenchmark.Document;
using GradeLevel = Chalkable.BusinessLogic.Model.AcademicBenchmark.GradeLevel;
using Standard = Chalkable.BusinessLogic.Model.AcademicBenchmark.Standard;
using StandardRelations = Chalkable.BusinessLogic.Model.AcademicBenchmark.StandardRelations;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IAcademicBenchmarkService
    {
        Task<IList<Standard>> GetStandardsByIds(IList<Guid> standardsIds);
        Task<IList<StandardRelations>> GetListOfStandardRelations(IList<Guid> standardsIds);
        Task<IList<Authority>> GetAuthorities();
        Task<IList<Document>> GetDocuments(Guid? authorityId);
        Task<IList<Subject>> GetSubjects(Guid? authorityId, Guid? documentId);
        Task<IList<GradeLevel>> GetGradeLevels(Guid? authorityId, Guid? documentId, string subjectCode);
        Task<PaginatedList<Standard>> SearchStandards(string searchQuery, int start = 0, int count = int.MaxValue);
        Task<IList<Standard>> GetStandards(Guid? authorityId, Guid? documentId, string subjectCode, string gradeLevelCode, Guid? parentId);
        Task<PaginatedList<Topic>> GetTopics(string subjectCode, string gradeLevel, Guid? parentId, string searchQuery, int start = 0, int count = int.MaxValue);
        Task<IList<Topic>> GetTopicsByIds(IList<Guid> topicsIds);
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
            return authorities.Select(Authority.Create).ToList();
        }

        public async Task<IList<Document>> GetDocuments(Guid? authorityId)
        {
            var docs = await _abConnectorLocator.StandardsConnector.GetDocuments(authorityId);
            return docs.Select(Document.Create).ToList();
        }

        public async Task<IList<Subject>> GetSubjects(Guid? authorityId, Guid? documentId)
        {
            var subs = await _abConnectorLocator.StandardsConnector.GetSubjects(authorityId, documentId);
            return subs.Select(Subject.Create).ToList();
        }

        public async Task<IList<GradeLevel>> GetGradeLevels(Guid? authorityId, Guid? documentId, string subjectCode)
        {
            var grLevels = await _abConnectorLocator.StandardsConnector.GetGradeLevels(authorityId, documentId, subjectCode);
            return grLevels.Select(GradeLevel.Create).ToList();
        }

        public async Task<PaginatedList<Standard>> SearchStandards(string searchQuery, int start = 0, int count = int.MaxValue)
        {
            var standards = await _abConnectorLocator.StandardsConnector.SearchStandards(searchQuery, start, count);
            return standards.Transform(Standard.Create);
        }

        public async Task<IList<Standard>> GetStandards(Guid? authorityId, Guid? documentId, string subjectCode, string gradeLevelCode, Guid? parentId)
        {
            var standards = await _abConnectorLocator.StandardsConnector.GetStandards(authorityId, documentId, subjectCode,
                        gradeLevelCode, parentId);
            return standards.Select(Standard.Create).ToList();
        }

        public async Task<PaginatedList<Topic>> GetTopics(string subjectCode, string gradeLevel, Guid? parentId, string searchQuery, 
            int start = 0, int count = int.MaxValue)
        {
            var topics = await _abConnectorLocator.TopicsConnector.GetTopics(subjectCode, gradeLevel, parentId, searchQuery, start, count);
            return topics.Transform(Topic.Create);
        }

        public async Task<IList<Topic>> GetTopicsByIds(IList<Guid> topicsIds)
        {
            var tasks = topicsIds.Select(x => _abConnectorLocator.TopicsConnector.GetTopic(x));
            var topics = await Task.WhenAll(tasks);
            return topics.Select(Topic.Create).Where(x => x != null).ToList();
        }
    }
}
