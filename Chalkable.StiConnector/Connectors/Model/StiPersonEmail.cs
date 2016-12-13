using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class StiPersonEmail
    {
        public string Description { get; set; }
        public string EmailAddress { get; set; }
        public bool IsListed { get; set; }
        public bool IsPrimary { get; set; }
        public int PersonId { get; set; }
    }
}
