using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoSisUserService : DemoSchoolServiceBase, ISisUserService
    {
        public DemoSisUserService(IServiceLocatorSchool serviceLocator, DemoStorage demoStorage) : base(serviceLocator, demoStorage)
        {
        }

        public void Add(IList<SisUser> users)
        {
            throw new NotImplementedException();
        }

        public void Edit(IList<SisUser> users)
        {
            throw new NotImplementedException();
        }

        public void Delete(IList<int> ids)
        {
            throw new NotImplementedException();
        }

        public IList<SisUser> GetAll()
        {
            throw new NotImplementedException();
        }

        public SisUser GetById(int id)
        {
            throw new NotImplementedException();
        }
    }
}