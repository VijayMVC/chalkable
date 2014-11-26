using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoAddressService : DemoSchoolServiceBase, IAddressService
    {
        public DemoAddressService(IServiceLocatorSchool serviceLocator, DemoStorage storage)
            : base(serviceLocator, storage)
        {
        }
      
        public void Add(IList<Address> addresses)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            Storage.AddressStorage.Add(addresses);
        }

        public void Edit(IList<Address> addressInfos)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            Storage.AddressStorage.Update(addressInfos);
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
