using System;
using System.Collections.Generic;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface ISchoolService
    {
        Data.Master.Model.School Create(Guid districtId, string name, IList<User> principals);
    }

    public class SchoolService : MasterServiceBase, ISchoolService
    {
        public SchoolService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        public Data.Master.Model.School Create(Guid districtId, string name, IList<User> principals)
        {
            string serverUrl = null;
            using (var uow = Update())
            {
                
                var school = new Data.Master.Model.School
                    {
                        DistrictRef = districtId,
                        Id = Guid.NewGuid(),
                        Name = name,
                        ServerUrl = serverUrl
                    };
            }
            throw new System.NotImplementedException();
        }
    }
}