using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IEthnicityService
    {
        void Add(IList<Ethnicity> models);
        void Edit(IList<Ethnicity> models);
        void Delete(IList<Ethnicity> models);
    }

    public class EthnicityService : SchoolServiceBase, IEthnicityService
    {
        public EthnicityService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void Add(IList<Ethnicity> models)
        {
            DoUpdate(u => new DataAccessBase<Ethnicity>(u).Insert(models));
        }

        public void Edit(IList<Ethnicity> models)
        {
            DoUpdate(u => new DataAccessBase<Ethnicity>(u).Update(models));
        }

        public void Delete(IList<Ethnicity> models)
        {
            DoUpdate(u => new DataAccessBase<Ethnicity>(u).Delete(models));
        }
    }
}
