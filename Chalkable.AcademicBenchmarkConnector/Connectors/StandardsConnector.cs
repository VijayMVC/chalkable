using System;
using System.Threading.Tasks;
using Chalkable.AcademicBenchmarkConnector.Models;

namespace Chalkable.AcademicBenchmarkConnector.Connectors
{
    public interface IStandardsConnector
    {
        Task<Standard> GetStandardById(Guid standardId);
        Task<RelatedStandard> GetRelatedStandardById(Guid standardId);
    }

    public class StandardsConnector : ConnectorBase, IStandardsConnector
    {
        public StandardsConnector(IConnectorLocator connectorLocator) : base(connectorLocator)
        {
        }

        public async Task<Standard> GetStandardById(Guid standardId)
        {
            var url = $"standanrds/{standardId}";
            var res = await GetOne<BaseResource<Standard>>(url, null);
            return res?.Data;
        }

        public async Task<RelatedStandard> GetRelatedStandardById(Guid standardId)
        {
            var url = $"standards/{standardId}/_relate";
            return await GetOne<RelatedStandard>(url, null);
        }
    }
}
