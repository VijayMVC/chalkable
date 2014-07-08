using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class PersonEmail
    {
        public const string ID_FIELD = "Id";
        public const string EMAIL_ADDRESS_FIELD = "EmailAddress";

        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        [PrimaryKeyFieldAttr]
        public string EmailAddress { get; set; }
        public string Description { get; set; }
        public bool IsListed { get; set; }
        public bool IsPrimary { get; set; }
    }
}
