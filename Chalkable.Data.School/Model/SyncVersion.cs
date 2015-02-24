using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class SyncVersion
    {
        [IdentityFieldAttr]
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public const string TABLE_NAME_FIELD = "TableName";
        public string TableName { get; set; }
        public const string VERSION_FIELD = "Version";
        public int Version { get; set; }
    }
}
