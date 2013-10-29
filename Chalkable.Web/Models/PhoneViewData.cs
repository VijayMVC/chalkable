using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Common.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class PhoneViewData
    {
        public int Id { get; set; }
        [SensitiveData]
        public string Value { get; set; }
        public bool IsPrimary { get; set; }
        public int Type { get; set; }

        public static PhoneViewData Create(Phone phone)
        {
            return new PhoneViewData
                       {
                           Id =  phone.Id,
                           Value = phone.Value,
                           IsPrimary = phone.IsPRIMARY,
                           Type = (int)phone.Type
                       };
        }

        public static IList<PhoneViewData> Create(IEnumerable<Phone> phones)
        {
            return phones.Select(Create).ToList();
        }
    }
}