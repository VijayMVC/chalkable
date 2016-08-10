using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class PersonLanguageAdapter : SyncModelAdapter<PersonLanguage>
    {
        public PersonLanguageAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private Data.School.Model.PersonLanguage Selector(PersonLanguage model)
        {
            return new Data.School.Model.PersonLanguage
            {
                PersonRef = model.PersonID,
                LanguageRef = model.LanguageID,
                IsPrimary = model.IsPrimary
            };
        }

        protected override void InsertInternal(IList<PersonLanguage> entities)
        {
            ServiceLocatorSchool.LanguageService.AddPersonLanguages(entities.Select(Selector).ToList());
        }

        protected override void UpdateInternal(IList<PersonLanguage> entities)
        {
            ServiceLocatorSchool.LanguageService.EditPersonLanguages(entities.Select(Selector).ToList());
        }

        protected override void DeleteInternal(IList<PersonLanguage> entities)
        {
            ServiceLocatorSchool.LanguageService.DeletePersonLanguages(entities.Select(Selector).ToList());
        }
    }
}
