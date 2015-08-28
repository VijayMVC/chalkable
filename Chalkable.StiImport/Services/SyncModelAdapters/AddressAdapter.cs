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

        protected override void InsertInternal(IList<Address> entities)
        {
            var addressInfos = entities.Select(x => new Data.School.Model.Address
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
            }).ToList();
            ServiceLocatorSchool.AddressService.Add(addressInfos);
        }

        protected override void UpdateInternal(IList<Address> entities)
        {
            var addresses = entities.Select(x => new Data.School.Model.Address
            {
                AddressLine1 = x.AddressLine1,
                AddressLine2 = x.AddressLine2,
                AddressNumber = x.AddressNumber,
                City = x.City,
                Country = x.Country,
                CountyId = x.CountryID,
                Id = x.AddressID,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                PostalCode = x.PostalCode,
                StreetNumber = x.StreetNumber,
                State = x.State
            }).ToList();
            ServiceLocatorSchool.AddressService.Edit(addresses);
        }

        protected override void DeleteInternal(IList<Address> entities)
        {
            var addresses = entities.Select(x => new Data.School.Model.Address { Id = x.AddressID }).ToList();
            ServiceLocatorSchool.AddressService.Delete(addresses);
        }
    }
}