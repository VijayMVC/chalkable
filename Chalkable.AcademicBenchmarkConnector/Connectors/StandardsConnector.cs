using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Web.Management;
using Chalkable.AcademicBenchmarkConnector.Models;

namespace Chalkable.AcademicBenchmarkConnector.Connectors
{
    public interface IStandardsConnector
    {
        Task<Standard> GetStandardById(Guid standardId);
        Task<StandardRelations> GetStandardRelationsById(Guid standardId);
        Task<PaginatedResponse<BaseResource<Standard>>> SearchStandard(string searchQuery, int start, int count);
        Task<IList<Authority>> GetAuthorities();
        Task<IList<Document>> GetDocuments(Guid? authorityId);
        Task<IList<Subject>> GetSubjects(Guid? authorityId, Guid? documentId);
        Task<IList<GradeLevel>> GetGradeLevels(Guid? authorityId, Guid? documentId, string subjectCode);

    }

    public class StandardsConnector : ConnectorBase, IStandardsConnector
    {
        public StandardsConnector(IConnectorLocator connectorLocator) : base(connectorLocator)
        {
        }

        public async Task<Standard> GetStandardById(Guid standardId)
        {
            var url = $"standards/{standardId}";
            var res = await GetOne<BaseResource<Standard>>(url, null);
            return res?.Data;
        }

        public async Task<StandardRelations> GetStandardRelationsById(Guid standardId)
        {
            var url = $"standards/{standardId}/_relate";
            return await GetOne<StandardRelations>(url, null);
        }

        public async Task<PaginatedResponse<BaseResource<Standard>>> SearchStandard(string searchQuery, int start, int count)
        {
            var url = $"standards";
            var nvc = new NameValueCollection
            {
                ["query"] = searchQuery,
                ["offest"] = start.ToString(),
                ["limit"] = count.ToString()
            };
            return await CallAsync<PaginatedResponse<BaseResource<Standard>>>(url, nvc);
        }

        public async Task<IList<Authority>> GetAuthorities()
        {
            return await GetList<Authority>(null, null, null);
        }
        public async Task<IList<Document>> GetDocuments(Guid? authorityId)
        {
            return await GetList<Document>(authorityId, null, null);
        }
        public async Task<IList<Subject>> GetSubjects(Guid? authorityId, Guid? documentId)
        {
            return await GetList<Subject>(authorityId, documentId, null);
        }

        public async Task<IList<GradeLevel>> GetGradeLevels(Guid? authorityId, Guid? documentId, string subjectCode)
        {
            return await GetList<GradeLevel>(authorityId, documentId, subjectCode);
        }
        
        private static IDictionary<Type, string> _typesDic = new Dictionary<Type, string>
        {
            [typeof(Standard)] = null,
            [typeof (Authority)] = "authority",
            [typeof (Document)] = "document",
            [typeof (Subject)] = "subject",
            [typeof (GradeLevel)] = "grade",
            [typeof(SubjectDocument)] = "subject_doc"
        };
        protected async Task<IList<TModel>> GetList<TModel>(Guid? authorityId, Guid? documentId, string subjectCode, int start = 0, int limit = int.MaxValue)
        {
            var nvc = new NameValueCollection
            {
                ["offset"] = start.ToString(),
                ["limit"] = limit.ToString(),
                ["list"] = _typesDic[typeof(TModel)]
            };
            if (authorityId.HasValue)
                nvc.Add("authority", authorityId.ToString());
            if (documentId.HasValue)
                nvc.Add("document", documentId.ToString());
            if (!string.IsNullOrWhiteSpace(subjectCode))
                nvc.Add("subject", subjectCode);

            return await GetList<TModel>("standards", nvc);
        }
    }
}
