using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.School.Model
{
    public class PersonEthnicity
    {
        public int PersonRef { get; set; }
        public int EthnicityRef { get; set; }
        public byte Percentage { get; set; }
        public bool IsPrimary { get; set; }
    }
}
