using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class AddressAdapter : SyncModelAdapter<Address>
    {
        public AddressAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private Data.School.Model.Address Selector(Address x)
        {
            return new Data.School.Model.Address
            {
                AddressLine1 = x.AddressLine1,
                AddressLine2 = x.AddressLine2,
                AddressNumber = x.AddressNumber,
                StreetNumber = x.StreetNumber,
                City = x.City,
                PostalCode = x.PostalCode,
                State = x.State,
                Country = x.Country,
                CountyId = x.CountryID,
                Id = x.AddressID
            };
        }

        protected override void InsertInternal(IList<Address> entities)
        {
            var addressInfos = entities.Select(Selector).ToList();
            ServiceLocatorSchool.AddressService.Add(addressInfos);
        }

        protected override void UpdateInternal(IList<Address> entities)
        {
            var addresses = entities.Select(Selector).ToList();
            ServiceLocatorSchool.AddressService.Edit(addresses);
        }

        protected override void DeleteInternal(IList<Address> entities)
        {
            var addresses = entities.Select(x => new Data.School.Model.Address { Id = x.AddressID }).ToList();
            ServiceLocatorSchool.AddressService.Delete(addresses);
        }

        protected override void PrepareToDeleteInternal(IList<Address> entities)
        {
            throw new System.NotImplementedException();
        }
    }
}