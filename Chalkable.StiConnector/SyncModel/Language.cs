using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.SyncModel
{
    public class Language : SyncModel
    {
        public int LanguageID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string StateCode { get; set; }
        public string SIFCode { get; set; }
        public string NCESCode { get; set; }
        public bool IsActive { get; set; }
        public bool IsSystem { get; set; }
        public Guid RowVersion { get; set; }
        public Guid DistrictGuid { get; set; }

        public override int DefaultOrder => 2;
    }
}
