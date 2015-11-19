using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.SyncModel
{
    public class SystemSetting:SyncModel
    {

        public string Category { get; set; }
        public string Setting { get; set; }
        public string Value { get; set; }
        public override int DefaultOrder => 49;
    }
}
