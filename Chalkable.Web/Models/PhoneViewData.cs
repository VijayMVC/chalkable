using System.Collections.Generic;
using System.Linq;
using Chalkable.Common.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class PhoneViewData
    {
        public int PersonId { get; set; }
        [SensitiveData]
        public string Value { get; set; }
        public bool IsPrimary { get; set; }
        public int Type { get; set; }

        public static PhoneViewData Create(Phone phone)
        {
            return new PhoneViewData
                       {
                           PersonId = phone.PersonRef,
                           Value = phone.Value,
                           IsPrimary = phone.IsPrimary,
                           Type = (int)phone.Type
                       };
        }

        public static IList<PhoneViewData> Create(IEnumerable<Phone> phones)
        {
            return phones.Select(Create).ToList();
        }
    }
}