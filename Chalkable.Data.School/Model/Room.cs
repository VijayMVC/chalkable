using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class Room
    {
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public string Description { get; set; }
        public string RoomNumber { get; set; }
        public string Size { get; set; }
        public int? Capacity { get; set; }
        public string PhoneNumber { get; set; }
        public int SchoolRef { get; set; }
    }
}
