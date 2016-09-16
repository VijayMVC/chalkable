using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface ILanguageService
    {
        void Edit(IList<Language> models);
        void Add(IList<Language> models);
        void Delete(IList<Language> models);

        void EditPersonLanguages(IList<PersonLanguage> models);
        void AddPersonLanguages(IList<PersonLanguage> models);
        void DeletePersonLanguages(IList<PersonLanguage> models);
    }

    public class LanguageService : SchoolServiceBase, ILanguageService
    {
        public LanguageService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void Add(IList<Language> models)
        {
            DoUpdate(u => new DataAccessBase<Language>(u).Insert(models));
        }

        public void Delete(IList<Language> models)
        {
            DoUpdate(u => new DataAccessBase<Language>(u).Delete(models));
        }

        public void EditPersonLanguages(IList<PersonLanguage> models)
        {
            DoUpdate(u => new DataAccessBase<PersonLanguage>(u).Update(models));
        }

        public void AddPersonLanguages(IList<PersonLanguage> models)
        {
            DoUpdate(u => new DataAccessBase<PersonLanguage>(u).Insert(models));
        }

        public void DeletePersonLanguages(IList<PersonLanguage> models)
        {
            DoUpdate(u => new DataAccessBase<PersonLanguage>(u).Delete(models));
        }

        public void Edit(IList<Language> models)
        {
            DoUpdate(u => new DataAccessBase<Language>(u).Update(models));
        }
    }
}
