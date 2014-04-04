using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoAddressStorage: BaseDemoStorage<int ,Address>
    {
        public DemoAddressStorage(DemoStorage storage) : base(storage)
        {
        }

        public IList<Address> GetAddress(int personId)
        {
            //add data to person storage
            //var ids = Storage.PersonStorage.
            //return addressData.Where(x => x.Value.)
            throw new NotImplementedException();
        }

        public IList<Address> Update(IList<Address> res)
        {

            foreach (var address in res)
            {
                if (data.ContainsKey(address.Id))
                    data[address.Id] = address;
            }
            return res;
        }

        public void Add(Address address)
        {
            if (!data.ContainsKey(address.Id))
            {
                data.Add(address.Id, address);
            }
        }

        public void Add(IList<Address> addresses)
        {
            foreach (var address in addresses)
            {
                Add(address);
            }
        }
    }
}
