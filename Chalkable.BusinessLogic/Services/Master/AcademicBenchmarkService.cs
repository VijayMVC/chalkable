using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chalkable.AcademicBenchmarkConnector.Connectors;
using Chalkable.AcademicBenchmarkConnector.Models;
using Chalkable.BusinessLogic.Model.AcademicBenchmark;
using Standard = Chalkable.BusinessLogic.Model.AcademicBenchmark.Standard;
using StandardRelations = Chalkable.BusinessLogic.Model.AcademicBenchmark.StandardRelations;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IAcademicBenchmarkService
    {
        Task<IList<Standard>> GetStandardsByIds(IList<Guid> standardsIds);
        Task<IList<StandardRelations>> GetListOfStandardRelations(IList<Guid> standardsIds);
    }

    public class AcademicBenchmarkService : MasterServiceBase, IAcademicBenchmarkService
    {
        private ConnectorLocator abConnectorLocator;
        public AcademicBenchmarkService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
            abConnectorLocator = new ConnectorLocator();
        }

        public async Task<IList<Standard>> GetStandardsByIds(IList<Guid> standardsIds)
        {
            IList<Task<AcademicBenchmarkConnector.Models.Standard>> tasks = standardsIds.Select(stId => abConnectorLocator.StandardsConnector.GetStandardById(stId)).ToList();
            var standards = await Task.WhenAll(tasks);
            standards = standards.Where(x => x != null).ToArray();
            return standards.Select(Standard.Create).ToList();
        }

        public async Task<IList<StandardRelations>> GetListOfStandardRelations(IList<Guid> standardsIds)
        {
            IList<Task<AcademicBenchmarkConnector.Models.StandardRelations>> tasks = standardsIds.Select(stId => abConnectorLocator.StandardsConnector.GetStandardRelationsById(stId)).ToList();
            var standards = await Task.WhenAll(tasks);
            standards = standards.Where(x => x != null).ToArray();
            return standards.Select(StandardRelations.Create).ToList();
        }
        
    }
}
