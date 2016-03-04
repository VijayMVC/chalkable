using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Model.AcademicBenchmarkStandard;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IAcademicBenchmarkService
    {
        IList<AcademicBenchmarkStandard> GetStandatdsByIds(IList<Guid> standardsIds);

        IList<AcademicBenchmarkRelatedStandard> GetRelatedStandardsByIds(IList<Guid> standardsIds);
    }

    public class AcademicBenchmarkService : MasterServiceBase, IAcademicBenchmarkService
    {
        public AcademicBenchmarkService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        public IList<AcademicBenchmarkStandard> GetStandatdsByIds(IList<Guid> standardsIds)
        {
            throw new NotImplementedException();
        }

        public IList<AcademicBenchmarkRelatedStandard> GetRelatedStandardsByIds(IList<Guid> standardsIds)
        {
            throw new NotImplementedException();
        }
        
    }
}
