using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class ContactRelationshipAdapter : SyncModelAdapter<ContactRelationship>
    {
        public ContactRelationshipAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private Data.School.Model.ContactRelationship Selector(ContactRelationship x)
        {
            return new Data.School.Model.ContactRelationship
            {
                Id = x.ContactRelationshipID,
                Name = x.Name,
                Code = x.Code,
                Description = x.Description,
                ReceivesMailings = x.ReceivesMailings,
                IsFamilyMember = x.IsFamilyMember,
                IsCustodian = x.IsCustodian,
                IsEmergencyContact = x.IsEmergencyContact,
                StateCode = x.StateCode,
                CanPickUp = x.CanPickUp,
                NCESCode = x.NCESCode,
                SIFCode = x.SIFCode,
                IsActive = x.IsActive,
                IsSystem = x.IsSystem
            };
        }

        protected override void InsertInternal(IList<ContactRelationship> entities)
        {
            var chlkContactRelationships = entities.Select(Selector).ToList();
            ServiceLocatorSchool.ContactService.AddContactRelationship(chlkContactRelationships);
        }

        protected override void UpdateInternal(IList<ContactRelationship> entities)
        {
            var contactRelationships = entities.Select(Selector).ToList();
            ServiceLocatorSchool.ContactService.EditContactRelationship(contactRelationships);
        }

        protected override void DeleteInternal(IList<ContactRelationship> entities)
        {
            var contactRelationships = entities.Select(x => new Data.School.Model.ContactRelationship { Id = x.ContactRelationshipID }).ToList();
            ServiceLocatorSchool.ContactService.DeleteContactRelationship(contactRelationships);
        }

        protected override void PrepareToDeleteInternal(IList<ContactRelationship> entities)
        {
            throw new System.NotImplementedException();
        }
    }
}