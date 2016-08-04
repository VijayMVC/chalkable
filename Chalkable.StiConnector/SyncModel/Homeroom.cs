using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.SyncModel
{
    public class Homeroom : SyncModel
    {
        public int HomeroomID { get; set; }
        public int AcadSessionID { get; set; }
        public string Name { get; set; }
        public int? TeacherID { get; set; }
        [NullableForeignKey]
        public int? RoomID { get; set; }
        public Guid RowVersion { get; set; }
        public Guid DistrictGuid { get; set; }
        public override int DefaultOrder => 21;
    }
}
