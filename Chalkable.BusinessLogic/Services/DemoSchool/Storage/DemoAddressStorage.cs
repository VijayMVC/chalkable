using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoAddressStorage
    {
        private readonly Dictionary<int, Address> addressData = new Dictionary<int, Address>(); 
 
        public Address GetById(int id)
        {
            if (addressData.ContainsKey(id))
                return addressData[id];
            return null;
        }

        public void Delete(int id)
        {
            addressData.Remove(id);
        }

        public IList<Address> GetAll()
        {
            return addressData.Select(x => x.Value).ToList();
        }

        public IList<Address> GetAddress(int personId)
        {
            //return addressData.Where(x => x.Value.)
            throw new NotImplementedException();
        }

        public void Delete(IList<int> ids)
        {
            foreach (var id in ids)
            {
                Delete(id);
            }
        }

        public IList<Address> Update(List<Address> res)
        {

            foreach (var address in res)
            {
                if (addressData.ContainsKey(address.Id))
                    addressData[address.Id] = address;
            }
            return res;
        }

        public void Add(Address address)
        {
            if (!addressData.ContainsKey(address.Id))
            {
                addressData.Add(address.Id, address);
            }
        }

        public void Add(List<Address> addresses)
        {
            foreach (var address in addresses)
            {
                Add(address);
            }
        }
    }
}
