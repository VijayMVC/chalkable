﻿using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Master
{
    public class DemoDeveloperService : DemoMasterServiceBase, IDeveloperService
    {
        public DemoDeveloperService(IServiceLocatorMaster serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
        }

        //TODO: needs test 
        public Developer GetDeveloperByDictrict(Guid districtId)
        {
            using (var uow = Read())
            {
                return new DeveloperDataAccess(uow).GetDeveloper(districtId);
            }
        }

        public Developer GetDeveloperById(Guid developerId)
        {
            throw new NotImplementedException();
        }

        public IList<Developer> GetDevelopers()
        {
            throw new NotImplementedException();
        }

        public Developer Add(string login, string password, string name, string webSite, Guid districtId)
        {
            throw new NotImplementedException();
        }

        public Developer Edit(Guid developerId, string name, string email, string webSite)
        {
            throw new NotImplementedException();
        }


        public Developer Add(string login, string password, string name, string webSite)
        {
            throw new NotImplementedException();
        }
    }
}
