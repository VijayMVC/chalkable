using System;
using System.Collections.Generic;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoAddressStorage: BaseDemoIntStorage<Address>
    {
        public DemoAddressStorage(DemoStorage storage) : base(storage, x => x.Id)
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

        public override void Setup()
        {
            throw new NotImplementedException();
        }
    }
}
