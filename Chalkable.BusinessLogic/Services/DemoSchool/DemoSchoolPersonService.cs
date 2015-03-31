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
        private DemoSchoolPersonStorage SchoolPersonStorage { get; set; }
        public DemoSchoolPersonService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
            SchoolPersonStorage = new DemoSchoolPersonStorage();
        }

        public IList<SchoolPerson> GetAll()
        {
            return SchoolPersonStorage.GetAll();
        }
    }
}