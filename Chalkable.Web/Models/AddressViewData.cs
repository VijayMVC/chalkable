using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class AddressViewData
    {
        public Guid Id { get; set; }
        [SensitiveData]
        public string Value { get; set; }
        public int Type { get; set; }

        public static AddressViewData Create(Address address)
        {
            return new AddressViewData
                       {
                           Id = address.Id,
                           Value = address.Value,
                           Type = (int)address.Type
                       };
        }

        public static IList<AddressViewData> Create(IEnumerable<Address> addresses)
        {
            return addresses.Select(Create).ToList();
        }
    }
}