using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chalkable.AcademicBenchmarkConnector.Connectors;
using Chalkable.AcademicBenchmarkConnector.Models;
using Chalkable.BusinessLogic.Model.AcademicBenchmarkStandard;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IAcademicBenchmarkService
    {
        Task<IList<AcademicBenchmarkStandard>> GetStandardsByIds(IList<Guid> standardsIds);
        Task<IList<AcademicBenchmarkStandardRelations>> GetListOfStandardRelations(IList<Guid> standardsIds);
    }

    public class AcademicBenchmarkService : MasterServiceBase, IAcademicBenchmarkService
    {
        private ConnectorLocator abConnectorLocator;
        public AcademicBenchmarkService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
            abConnectorLocator = new ConnectorLocator();
        }

        public async Task<IList<AcademicBenchmarkStandard>> GetStandardsByIds(IList<Guid> standardsIds)
        {
            IList<Task<Standard>> tasks = standardsIds.Select(stId => abConnectorLocator.StandardsConnector.GetStandardById(stId)).ToList();
            var standards = await Task.WhenAll(tasks);
            standards = standards.Where(x => x != null).ToArray();
            return standards.Select(AcademicBenchmarkStandard.Create).ToList();
        }

        public async Task<IList<AcademicBenchmarkStandardRelations>> GetListOfStandardRelations(IList<Guid> standardsIds)
        {
            IList<Task<RelatedStandard>> tasks = standardsIds.Select(stId => abConnectorLocator.StandardsConnector.GetRelatedStandardById(stId)).ToList();
            var standards = await Task.WhenAll(tasks);
            standards = standards.Where(x => x != null).ToArray();
            return standards.Select(AcademicBenchmarkStandardRelations.Create).ToList();
        }
        
    }
}
