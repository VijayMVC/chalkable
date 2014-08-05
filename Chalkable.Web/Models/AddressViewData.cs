using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chalkable.Common;
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
        public decimal? Latitude { get; set; }
        [SensitiveData]
        public decimal? Longitude { get; set; }

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
                           PostalCode = address.PostalCode,
                           Value = BuildFullAddress(address)
                       };
        }

        private static string BuildFullAddress(Address address)
        {
            var streetName = string.IsNullOrEmpty(address.AddressLine1) ? address.AddressLine2 : address.AddressLine1;
            var res = new List<string>();
            if(!string.IsNullOrEmpty(streetName) && !string.IsNullOrEmpty(address.StreetNumber))
                res.Add(string.Format("{0} {1}", address.StreetNumber, streetName));
            if(!string.IsNullOrEmpty(address.AddressNumber))
                res.Add(string.Format("#{0}", address.AddressNumber));
            if(!string.IsNullOrEmpty(address.City))
                res.Add(address.City);
            if(!string.IsNullOrEmpty(address.State))
                res.Add(address.State);
            if(!string.IsNullOrEmpty(address.PostalCode))
                res.Add(address.PostalCode);
            return res.JoinString(", ");
        }

        public static IList<AddressViewData> Create(IEnumerable<Address> addresses)
        {
            return addresses.Select(Create).ToList();
        }
    }
}