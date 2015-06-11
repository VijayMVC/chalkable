using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{

        public class DistrictSettings
        {
            public const string CATEGORY_FIELD = "Category";
            public const string SETTING_FIELD = "Setting";
            
            [PrimaryKeyFieldAttr]
            public string Category { get; set; }
            [PrimaryKeyFieldAttr]
            public string Setting { get; set; }
            public string Value { get; set; }
        }

}
