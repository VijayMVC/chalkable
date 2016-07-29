using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class PersonAdapter : SyncModelAdapter<Person>
    {
        public PersonAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private Data.School.Model.Person Selector(Person x)
        {
            return new Data.School.Model.Person
            {
                Active = false,
                AddressRef = x.PhysicalAddressID,
                BirthDate = x.DateOfBirth,
                FirstName = x.FirstName,
                Gender = x.GenderID.HasValue ? Locator.GenderMapping[x.GenderID.Value] : "U",
                Id = x.PersonID,
                LastName = x.LastName,
                PhotoModifiedDate = x.PhotoModifiedDate
            };
        }

        protected override void InsertInternal(IList<Person> entities)
        {
            var persons = entities.Select(Selector).ToList();
            ServiceLocatorSchool.PersonService.Add(persons);
        }

        protected override void UpdateInternal(IList<Person> entities)
        {
            var persons = entities.Select(Selector).ToList();
            ServiceLocatorSchool.PersonService.UpdateForImport(persons);
        }

        protected override void DeleteInternal(IList<Person> entities)
        {
            var persons = entities.Select(x => new Data.School.Model.Person { Id = x.PersonID }).ToList();
            ServiceLocatorSchool.PersonService.Delete(persons);
        }
    }
}