using System;
using Chalkable.Data.Common;

namespace Chalkable.Data.Master.Model
{
    public class ChalkableDepartment
    {
        [PrimaryKeyFieldAttr]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Keywords { get; set; }
    }
}