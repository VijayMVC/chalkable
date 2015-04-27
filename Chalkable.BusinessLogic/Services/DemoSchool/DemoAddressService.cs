using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoAddressStorage : BaseDemoIntStorage<Address>
    {
        public DemoAddressStorage()
            : base(x => x.Id)
        {
        }
    }

    public class DemoAddressService : DemoSchoolServiceBase, IAddressService
    {
        private DemoAddressStorage AddressStorage { get; set; }
        public DemoAddressService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
            AddressStorage = new DemoAddressStorage();
        }
      
        public void Add(IList<Address> addresses)
        {
            AddressStorage.Add(addresses);
        }

        public void Edit(IList<Address> addressInfos)
        {
            AddressStorage.Update(addressInfos);
        }

        public void Delete(IList<Address> addresses)
        {
            AddressStorage.Delete(addresses);
            
        }

        public IList<Address> GetAddress()
        {
            return AddressStorage.GetAll();
        }

        public Address GetAddress(int addressId)
        {
            return AddressStorage.GetAll().FirstOrDefault(x => x.Id == addressId);
        }
    }
}
