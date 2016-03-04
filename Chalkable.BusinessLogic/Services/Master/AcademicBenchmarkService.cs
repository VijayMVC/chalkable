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
        Task<IList<AcademicBenchmarkStandard>> GetStandatdsByIds(IList<Guid> standardsIds);
        Task<IList<AcademicBenchmarkRelatedStandard>> GetRelatedStandardsByIds(IList<Guid> standardsIds);
    }

    public class AcademicBenchmarkService : MasterServiceBase, IAcademicBenchmarkService
    {
        private ConnectorLocator abConnectorLocator;
        public AcademicBenchmarkService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
            abConnectorLocator = new ConnectorLocator();
        }

        public async Task<IList<AcademicBenchmarkStandard>> GetStandatdsByIds(IList<Guid> standardsIds)
        {
            IList<Task<Standard>> tasks = standardsIds.Select(stId => abConnectorLocator.StandardsConnector.GetStandardById(stId)).ToList();
            var standards = await Task.WhenAll(tasks);
            return standards.Select(AcademicBenchmarkStandard.Create).ToList();
        }

        public async Task<IList<AcademicBenchmarkRelatedStandard>> GetRelatedStandardsByIds(IList<Guid> standardsIds)
        {
            throw new NotImplementedException();
        }
        
    }
}
