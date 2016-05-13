using System;
using System.Collections.Generic;
using Chalkable.Data.Master.DataAccess;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IApplicationSchoolOptionService
    {
        void SubmitApplicationBan(Guid applicationId, IList<Guid> schoolIds);
    }

    public class ApplicationSchoolOptionService : MasterServiceBase, IApplicationSchoolOptionService
    {
        public ApplicationSchoolOptionService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        public void SubmitApplicationBan(Guid applicationId, IList<Guid> schoolIds)
        {
            DoUpdate(u => new ApplicationSchoolOptionDataAccess(u).BanSchoolsByIds(applicationId, schoolIds));
        }
    }
}
