using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class LanguageAdapter : SyncModelAdapter<Language>
    {
        public LanguageAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private Data.School.Model.Language Selector(Language x)
        {
            return new Data.School.Model.Language
            {
                Id = x.LanguageID,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                StateCode = x.StateCode,
                NCESCode = x.NCESCode,
                SIFCode = x.SIFCode,
                IsActive = x.IsActive,
                IsSystem = x.IsSystem
            };
        }

        protected override void InsertInternal(IList<Language> entities)
        {
            ServiceLocatorSchool.LanguageService.Add(entities.Select(Selector).ToList());
        }

        protected override void UpdateInternal(IList<Language> entities)
        {
            ServiceLocatorSchool.LanguageService.Edit(entities.Select(Selector).ToList());
        }

        protected override void DeleteInternal(IList<Language> entities)
        {
            ServiceLocatorSchool.LanguageService.Delete(entities.Select(Selector).ToList());
        }

        protected override void PrepareToDeleteInternal(IList<Language> entities)
        {
            throw new System.NotImplementedException();
        }
    }
}
