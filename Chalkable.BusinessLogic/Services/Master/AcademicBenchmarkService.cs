using System;
using System.Collections.Generic;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IAcademicBenchmarkService
    {
        IList<AcademicBenchmarkStandard> StandatdsIds(IList<Guid> standardsIds);

        IList<AcademicBenchmarkRelatedStandard> RelatedStandardsByIds(IList<Guid> standardsIds);
    }

    public class AcademicBenchmarkService : MasterServiceBase, IAcademicBenchmarkService
    {
        public AcademicBenchmarkService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        public IList<AcademicBenchmarkRelatedStandard> RelatedStandardsByIds(IList<Guid> standardsIds)
        {
            throw new NotImplementedException();
        }

        public IList<AcademicBenchmarkStandard> StandatdsIds(IList<Guid> standardsIds)
        {
            throw new NotImplementedException();
        }
    }
}
