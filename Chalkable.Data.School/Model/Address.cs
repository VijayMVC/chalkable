using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class Address
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
        public int CountryID { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }

    public enum AddressType
    {
        Home,
        Work
    }
}
