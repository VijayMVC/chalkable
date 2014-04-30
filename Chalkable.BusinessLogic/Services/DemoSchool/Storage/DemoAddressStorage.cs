using System;
using System.Collections.Generic;
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
            var person = Storage.PersonStorage.GetById(personId);
            var res = new List<Address>();

            if (person.AddressRef.HasValue && data.ContainsKey(person.AddressRef.Value))
                res.Add(data[person.AddressRef.Value]);
            return res;
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

        public override void Setup()
        {
            throw new NotImplementedException();
        }
    }
}
