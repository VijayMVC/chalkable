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
      

        public Address Add(Address addressInfo)
        {
            if (!BaseSecurity.IsDistrict(Context))//TODO:can teacher do this?
                throw new ChalkableSecurityException();
            Storage.AddressStorage.Add(addressInfo);
            return addressInfo;
        }

        public void Add(IList<Address> addressInfos)
        {
            if (!BaseSecurity.IsDistrict(Context))//TODO:can teacher do this?
                throw new ChalkableSecurityException();
            Storage.AddressStorage.Add(addressInfos);
        }

        public Address Edit(Address addressInfo)
        {
            return Edit(new List<Address> { addressInfo }).First();
        }

        public IList<Address> Edit(IList<Address> addressInfos)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            return Storage.AddressStorage.Update(addressInfos);
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
