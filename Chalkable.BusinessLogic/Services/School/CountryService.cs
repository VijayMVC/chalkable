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
    public interface ICountryService
    {
        void Add(IList<Country> models);
        void Edit(IList<Country> models);
        void Delete(IList<Country> models);
    }

    public class CountryService : SchoolServiceBase, ICountryService
    {
        public CountryService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void Edit(IList<Country> models)
        {
            DoUpdate(u => new DataAccessBase<Country>(u).Update(models));
        }

        public void Delete(IList<Country> models)
        {
            DoUpdate(u => new DataAccessBase<Country>(u).Delete(models));
        }

        public void Add(IList<Country> models)
        {
            DoUpdate(u => new DataAccessBase<Country>(u).Insert(models));
        }

        
    }
}
