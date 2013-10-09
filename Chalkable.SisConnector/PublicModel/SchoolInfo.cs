using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.SisConnector.PublicModel
{
    public class SchoolInfo
    {
        public int SchoolId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }

        public string PrincipalName { get; set; }
        public string PrincipalTitle { get; set; }
        public string PrincipalEmail { get; set; }

        public IList<SchoolYearInfo> SchoolYears { get; set; }
    }
}
