using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class PersonEthnicityAdapter : SyncModelAdapter<PersonEthnicity>
    {
        public PersonEthnicityAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private Data.School.Model.PersonEthnicity Selector(PersonEthnicity model)
        {
            return new Data.School.Model.PersonEthnicity
            {
                PersonRef = model.PersonID,
                EthnicityRef = model.EthnicityID,
                Percentage = model.Percentage,
                IsPrimary = model.IsPrimary
            };
        }

        protected override void InsertInternal(IList<PersonEthnicity> entities)
        {
            ServiceLocatorSchool.PersonEthnicityService.Add(entities.Select(Selector).ToList());
        }

        protected override void UpdateInternal(IList<PersonEthnicity> entities)
        {
            ServiceLocatorSchool.PersonEthnicityService.Edit(entities.Select(Selector).ToList());
        }

        protected override void DeleteInternal(IList<PersonEthnicity> entities)
        {
            ServiceLocatorSchool.PersonEthnicityService.Delete(entities.Select(Selector).ToList());
        }
    }
}
