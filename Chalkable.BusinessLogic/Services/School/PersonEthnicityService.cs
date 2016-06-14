using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IPersonEthnicityService
    {
        void Add(IList<PersonEthnicity> models);
        void Edit(IList<PersonEthnicity> models);
        void Delete(IList<PersonEthnicity> models);
    }

    public class PersonEthnicityService : SchoolServiceBase, IPersonEthnicityService
    {
        public PersonEthnicityService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void Add(IList<PersonEthnicity> models)
        {
            DoUpdate(u => new DataAccessBase<PersonEthnicity>(u).Insert(models));
        }

        public void Edit(IList<PersonEthnicity> models)
        {
            DoUpdate(u => new DataAccessBase<PersonEthnicity>(u).Update(models));
        }

        public void Delete(IList<PersonEthnicity> models)
        {
            DoUpdate(u => new DataAccessBase<PersonEthnicity>(u).Delete(models));
        }

    }
}
