using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class Staff
    {
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Salutation { get; set; }
        public int? AddressRef { get; set; }
        public bool Active { get; set; }
        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public int? UresId { get; set; }
    }
}
