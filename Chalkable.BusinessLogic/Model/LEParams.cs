using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.BusinessLogic.Model
{
    public class LEParams
    {
        public bool LELinkStatus { get; set; }
        public string LEBaseUrl { get; set; }
        public bool LEEnabled { get; set; }
        public bool LESyncComplete { get; set; }
        public bool EnabledForUser { get; set; }
    }
}
