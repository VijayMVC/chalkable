using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.SyncModel
{
    public class PersonLanguage : SyncModel
    {
        public int PersonID { get; set; }
        public int LanguageID { get; set; }
        public bool IsPrimary { get; set; }
        public Guid DistrictGuid { get; set; }
        public override int DefaultOrder => 6;
    }
}
