using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        public AcademicBenchmarkService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        public async Task<IList<AcademicBenchmarkStandard>> GetStandatdsByIds(IList<Guid> standardsIds)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<AcademicBenchmarkRelatedStandard>> GetRelatedStandardsByIds(IList<Guid> standardsIds)
        {
            throw new NotImplementedException();
        }
        
    }
}
