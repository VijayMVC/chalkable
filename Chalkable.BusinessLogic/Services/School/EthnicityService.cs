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
        IList<Ethnicity> GetAll(); 

        void AddPersonEthnicities(IList<PersonEthnicity> models);
        void EditPersonEthnicities(IList<PersonEthnicity> models);
        void DeletePersonEthnicities(IList<PersonEthnicity> models);
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

        public IList<Ethnicity> GetAll()
        {
            return DoRead(u => new DataAccessBase<Ethnicity>(u).GetAll());
        }

        public void AddPersonEthnicities(IList<PersonEthnicity> models)
        {
            DoUpdate(u => new DataAccessBase<PersonEthnicity>(u).Insert(models));
        }

        public void EditPersonEthnicities(IList<PersonEthnicity> models)
        {
            DoUpdate(u => new DataAccessBase<PersonEthnicity>(u).Update(models));
        }

        public void DeletePersonEthnicities(IList<PersonEthnicity> models)
        {
            DoUpdate(u => new DataAccessBase<PersonEthnicity>(u).Delete(models));
        }
    }
}
