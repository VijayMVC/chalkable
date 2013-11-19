using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Model
{
    public class AddressInfo
    {
        public int Id { get; set; }
        public string AddressNumber { get; set; }
        public string StreetNumber { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public int? CountyId { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public static AddressInfo Create(Address address)
        {
            return new AddressInfo
                {
                    Id = address.Id,
                    AddressLine2 = address.AddressLine2,
                    AddressLine1 = address.AddressLine1,
                    AddressNumber = address.AddressNumber,
                    City = address.City,
                    Country = address.Country,
                    CountyId = address.CountyId,
                    Latitude = address.Latitude,
                    Longitude = address.Longitude,
                    PostalCode = address.PostalCode,
                    StreetNumber = address.StreetNumber,
                    State = address.State
                };
        }
    }
}
