using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class PersonEmailAdapter : SyncModelAdapter<PersonEmail>
    {
        public PersonEmailAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private Data.School.Model.PersonEmail Selector(PersonEmail x)
        {
            return new Data.School.Model.PersonEmail
            {
                PersonRef = x.PersonID,
                Description = x.Description,
                EmailAddress = x.EmailAddress,
                IsListed = x.IsListed,
                IsPrimary = x.IsPrimary
            };
        }

        protected override void InsertInternal(IList<PersonEmail> entities)
        {
            var chlkPersonsEmails = entities.Select(Selector).ToList();
            ServiceLocatorSchool.PersonEmailService.AddPersonsEmails(chlkPersonsEmails);
        }

        protected override void UpdateInternal(IList<PersonEmail> entities)
        {
            var chlkPersonsEmails = entities.Select(Selector).ToList();
            ServiceLocatorSchool.PersonEmailService.UpdatePersonsEmails(chlkPersonsEmails);
        }

        protected override void DeleteInternal(IList<PersonEmail> entities)
        {
            var personEmails = entities.Select(x => new Data.School.Model.PersonEmail
            {
                PersonRef = x.PersonID,
                Description = x.Description,
                EmailAddress = x.EmailAddress,
                IsListed = x.IsListed,
                IsPrimary = x.IsPrimary
            }).ToList();
            ServiceLocatorSchool.PersonEmailService.DeletePersonsEmails(personEmails);
        }
    }
}