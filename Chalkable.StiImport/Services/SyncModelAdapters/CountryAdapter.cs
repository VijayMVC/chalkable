using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class CountryAdapter : SyncModelAdapter<Country>
    {
        public CountryAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private Data.School.Model.Country Selector(Country x)
        {
            return new Data.School.Model.Country
            {
                Id = x.CountryID,
                Code = x.Code,
                Description = x.Description,
                Name = x.Name,
                StateCode = x.StateCode,
                SIFCode = x.SIFCode,
                NCESCode = x.NCESCode,
                IsActive = x.IsActive,
                IsSystem = x.IsSystem
            };
        }

        protected override void InsertInternal(IList<Country> entities)
        {
            ServiceLocatorSchool.CountryService.Add(entities.Select(Selector).ToList());
        }

        protected override void UpdateInternal(IList<Country> entities)
        {
            ServiceLocatorSchool.CountryService.Edit(entities.Select(Selector).ToList());
        }

        protected override void DeleteInternal(IList<Country> entities)
        {
            ServiceLocatorSchool.CountryService.Delete(entities.Select(Selector).ToList());
        }
    }
}
