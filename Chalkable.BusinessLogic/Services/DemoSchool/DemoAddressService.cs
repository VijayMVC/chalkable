using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoAddressService : DemoSchoolServiceBase, IAddressService
    {
        public DemoAddressService(IServiceLocatorSchool serviceLocator, DemoStorage storage)
            : base(serviceLocator, storage)
        {
        }

        public void Add(IList<AddressInfo> addressInfos)
        {
            if (!BaseSecurity.IsDistrict(Context))//TODO:can teacher do this?
                throw new ChalkableSecurityException();
            var addresses = addressInfos.Select(EditAddress).ToList();
            Storage.AddressStorage.Add(addresses);
        }

        public Address Add(AddressInfo addressInfo)
        {
            var a = EditAddress(addressInfo);
            if (!BaseSecurity.IsDistrict(Context))//TODO:can teacher do this?
                throw new ChalkableSecurityException();
            var address = EditAddress(addressInfo);
            Storage.AddressStorage.Add(address);
            return address;
        }

        public Address Edit(AddressInfo addressInfo)
        {
            if (!BaseSecurity.IsDistrict(Context))//TODO:can teacher do this?
                throw new ChalkableSecurityException();
            var address = Storage.AddressStorage.GetById(addressInfo.Id);
            if (!AddressSecurity.CanModify(address, Context))
                throw new ChalkableSecurityException();
            address = EditAddress(addressInfo);
            return address;
        }

        public IList<Address> Edit(IList<AddressInfo> addresses)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            var res = addresses.Select(EditAddress).ToList();
            return Storage.AddressStorage.Update(res);
        }

        private Address EditAddress(AddressInfo addressInfo)
        {
            var address = new Address();
            address.Id = addressInfo.Id;
            address.AddressLine1 = addressInfo.AddressLine1;
            address.AddressLine2 = addressInfo.AddressLine2;
            address.AddressNumber = addressInfo.AddressNumber;
            address.StreetNumber = addressInfo.StreetNumber;
            address.City = addressInfo.City;
            address.State = addressInfo.State;
            address.Country = addressInfo.Country;
            address.CountyId = addressInfo.CountyId;
            address.Latitude = addressInfo.Latitude;
            address.Longitude = addressInfo.Longitude;
            address.PostalCode = addressInfo.PostalCode;
            return address;
        }

        public void Delete(int id)
        {
            var address = Storage.AddressStorage.GetById(id);
            if (!AddressSecurity.CanModify(address, Context))
                throw new ChalkableSecurityException();
            Storage.AddressStorage.Delete(id);
        }

        public void Delete(IList<int> ids)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            Storage.AddressStorage.Delete(ids);
            
        }

        public IList<Address> GetAddress()
        {
            if (!BaseSecurity.IsAdminOrTeacher(Context))
                throw new ChalkableSecurityException();
            return Storage.AddressStorage.GetAll();
        }

        public IList<Address> GetAddress(int personId)
        {
            if (!BaseSecurity.IsAdminOrTeacher(Context))
                throw new ChalkableSecurityException();
            return Storage.AddressStorage.GetAddress(personId);
            
        }
    }
}
