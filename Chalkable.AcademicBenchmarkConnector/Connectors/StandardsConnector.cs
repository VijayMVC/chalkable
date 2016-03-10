using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Chalkable.AcademicBenchmarkConnector.Models;
using Chalkable.Common;

namespace Chalkable.AcademicBenchmarkConnector.Connectors
{
    public interface IStandardsConnector
    {
        Task<Standard> GetStandardById(Guid standardId);
        Task<StandardRelations> GetStandardRelationsById(Guid standardId);
        Task<IList<Authority>> GetAuthorities();
        Task<IList<Document>> GetDocuments(Guid? authorityId);
        Task<IList<Subject>> GetSubjects(Guid? authorityId, Guid? documentId);
        Task<IList<GradeLevel>> GetGradeLevels(Guid? authorityId, Guid? documentId, string subjectCode);
        Task<PaginatedList<Standard>> SearchStandards(string searchQuery, int start, int count);
        Task<IList<Standard>> GetStandards(Guid? authorityId, Guid? documentId, string subjectCode, string gradeLevelCode, Guid? parentId);

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

        public async Task<PaginatedList<Standard>> SearchStandards(string searchQuery, int start, int count)
        {
            return await GetPage<Standard>(null, null, null, searchQuery: searchQuery, start: start, limit: count);
        }

        public async Task<IList<Standard>> GetStandards(Guid? authorityId, Guid? documentId, string subjectCode, string gradeLevelCode, Guid? parentId)
        {
            return await GetPage<Standard>(authorityId, documentId, subjectCode, parentId: parentId, gradeLevelCode: gradeLevelCode);
        }

        public async Task<IList<Authority>> GetAuthorities()
        {
            return (await GetPage<AuthorityWrapper>(null, null, null)).Select(x => x.Authority).ToList();
        }
        public async Task<IList<Document>> GetDocuments(Guid? authorityId)
        {
            return (await GetPage<DocumentWrapper>(authorityId, null, null)).Select(x => x.Document).ToList();
        }
        public async Task<IList<Subject>> GetSubjects(Guid? authorityId, Guid? documentId)
        {
            return (await GetPage<SubjectWrapper>(authorityId, documentId, null)).Select(x => x.Subject).ToList();
        }

        public async Task<IList<GradeLevel>> GetGradeLevels(Guid? authorityId, Guid? documentId, string subjectCode)
        {
            return (await GetPage<GradeLevelWrapper>(authorityId, documentId, subjectCode)).Select(x=>x.GradeLevel).ToList();
        }
        
        private static IDictionary<Type, string> _typesDic = new Dictionary<Type, string>
        {
            [typeof (Standard)] = null,
            [typeof (AuthorityWrapper)] = "authority",
            [typeof (DocumentWrapper)] = "document",
            [typeof (SubjectWrapper)] = "subject",
            [typeof (GradeLevelWrapper)] = "grade",
            [typeof (SubjectDocumentWrapper)] = "subject_doc"
        };
        protected async Task<PaginatedList<TModel>> GetPage<TModel>(Guid? authorityId, Guid? documentId, string subjectCode, 
            Guid? parentId = null, string gradeLevelCode = null, string searchQuery = null, int start = 0, int limit = int.MaxValue)
        {
            var nvc = new NameValueCollection
            {
                ["list"] = _typesDic[typeof(TModel)]
            };
            if (authorityId.HasValue)
                nvc.Add("authority", authorityId.Value.ToString());
            if (documentId.HasValue)
                nvc.Add("document", documentId.Value.ToString());
            if (!string.IsNullOrWhiteSpace(subjectCode))
                nvc.Add("subject", subjectCode);
            if(parentId.HasValue)
                nvc.Add("parent", parentId.Value.ToString());
            if (!string.IsNullOrWhiteSpace(searchQuery))
                nvc.Add("query", searchQuery);
            if(!string.IsNullOrWhiteSpace(gradeLevelCode))
                nvc.Add("grade", gradeLevelCode);

            var res = await GetPage<BaseResource<TModel>>("standards", nvc);
            return res.Transform(x => x.Data);
        }
    }
}
