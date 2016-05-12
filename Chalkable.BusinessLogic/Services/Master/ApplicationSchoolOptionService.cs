using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Master.DataAccess;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IApplicationSchoolOptionService
    {
        void SubmitApplicationsBan(Guid applicationId, IList<Guid> schoolIds);
    }

    public class ApplicationSchoolOptionService : MasterServiceBase, IApplicationSchoolOptionService
    {
        public ApplicationSchoolOptionService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        public void SubmitApplicationsBan(Guid applicationId, IList<Guid> schoolIds)
        {
            DoUpdate(u => new ApplicationSchoolOptionDataAccess(u).BanSchoolsByIds(applicationId, schoolIds));
        }
    }
}
