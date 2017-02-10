using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class PersonEthnicity
    {
        [PrimaryKeyFieldAttr]
        public int PersonRef { get; set; }
        [PrimaryKeyFieldAttr]
        public int EthnicityRef { get; set; }
        public int Percentage { get; set; }
        public bool IsPrimary { get; set; }
    }
}
