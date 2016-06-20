using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class PersonLanguage
    {
        [PrimaryKeyFieldAttr]
        public int PersonRef { get; set; }
        [PrimaryKeyFieldAttr]
        public int LanguageRef { get; set; }
        public bool IsPrimary { get; set; }
    }
}
