using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface ICountryService
    {
        void Add(IList<Country> models);
        void Edit(IList<Country> models);
        void Delete(IList<Country> models);

        void AddPersonNationality(IList<PersonNationality> models);
        void EditPersonNationality(IList<PersonNationality> models);
        void DeletePersonNationality(IList<PersonNationality> models);
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

        public void AddPersonNationality(IList<PersonNationality> models)
        {
            DoUpdate(u => new DataAccessBase<PersonNationality>(u).Insert(models));
        }

        public void EditPersonNationality(IList<PersonNationality> models)
        {
            DoUpdate(u => new DataAccessBase<PersonNationality>(u).Update(models));
        }

        public void DeletePersonNationality(IList<PersonNationality> models)
        {
            DoUpdate(u => new DataAccessBase<PersonNationality>(u).Delete(models));
        }
    }
}
