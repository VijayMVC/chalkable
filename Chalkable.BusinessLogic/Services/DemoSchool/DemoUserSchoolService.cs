using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoUserSchoolService : DemoSchoolServiceBase, IUserSchoolService
    {
        public DemoUserSchoolService(IServiceLocatorSchool serviceLocator, DemoStorage demoStorage) : base(serviceLocator, demoStorage)
        {
        }

        public void Add(IList<UserSchool> userSchools)
        {
            throw new NotImplementedException();
        }

        public void Delete(IList<UserSchool> userSchools)
        {
            throw new NotImplementedException();
        }
    }
}