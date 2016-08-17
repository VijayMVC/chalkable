using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class PersonNationalityAdapter : SyncModelAdapter<PersonNationality>
    {
        public PersonNationalityAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private Data.School.Model.PersonNationality Selector(PersonNationality x)
        {
            return new Data.School.Model.PersonNationality
            {
                Id = x.PersonNationalityID,
                PersonRef = x.PersonID,
                CountryRef = x.NationalityID,
                Nationality = x.Nationality,
                IsPrimary = x.IsPrimary
            };
        }
        protected override void InsertInternal(IList<PersonNationality> entities)
        {
            ServiceLocatorSchool.CountryService.AddPersonNationality(entities.Select(Selector).ToList());
        }

        protected override void UpdateInternal(IList<PersonNationality> entities)
        {
            ServiceLocatorSchool.CountryService.EditPersonNationality(entities.Select(Selector).ToList());
        }

        protected override void DeleteInternal(IList<PersonNationality> entities)
        {
            ServiceLocatorSchool.CountryService.DeletePersonNationality(entities.Select(Selector).ToList());
        }

        protected override void PrepareToDeleteInternal(IList<PersonNationality> entities)
        {
            throw new System.NotImplementedException();
        }
    }
}
