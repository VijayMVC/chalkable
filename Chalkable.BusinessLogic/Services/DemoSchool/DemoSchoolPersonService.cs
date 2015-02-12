using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoSchoolPersonService : DemoSchoolServiceBase, ISchoolPersonService
    {
        public DemoSchoolPersonService(IServiceLocatorSchool serviceLocator, DemoStorage storage)
            : base(serviceLocator, storage)
        {
        }

        public IList<SchoolPerson> GetAll()
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            return Storage.SchoolPersonStorage.GetAll();
        }
    }
}