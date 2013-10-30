using System.Collections.Generic;
using System.Linq;
using Chalkable.Common.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class AddressViewData
    {
        public int Id { get; set; }
        [SensitiveData]
        public string AddressNumber { get; set; }
        [SensitiveData]
        public string StreetNumber { get; set; }
        [SensitiveData]
        public string AddressLine1 { get; set; }
        [SensitiveData]
        public string AddressLine2 { get; set; }
        [SensitiveData]
        public string City { get; set; }
        [SensitiveData]
        public string State { get; set; }
        [SensitiveData]
        public string PostalCode { get; set; }
        [SensitiveData]
        public string Country { get; set; }
        [SensitiveData]
        public int? CountyId { get; set; }
        [SensitiveData]
        public decimal Latitude { get; set; }
        [SensitiveData]
        public decimal Longitude { get; set; }

        [SensitiveData]
        public string Value { get; set; } 

        public static AddressViewData Create(Address address)
        {
            return new AddressViewData
                       {
                           Id = address.Id,
                           AddressLine1 = address.AddressLine1,
                           AddressLine2 = address.AddressLine2,
                           AddressNumber = address.AddressNumber,
                           City = address.City,
                           Country = address.Country,
                           CountyId = address.CountyId,
                           Latitude = address.Latitude,
                           Longitude = address.Longitude,
                           PostalCode = address.PostalCode
                       };
        }

        public static IList<AddressViewData> Create(IEnumerable<Address> addresses)
        {
            return addresses.Select(Create).ToList();
        }
    }
}