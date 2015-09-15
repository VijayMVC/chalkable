using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class PersonSetting
    {
        [PrimaryKeyFieldAttr]
        public int PersonRef { get; set; }
        [PrimaryKeyFieldAttr]
        public string Key { get; set; }
        public string Value { get; set; }

        //Settings
        public const string FEED_SORTING = "feedsort";
        public const string FEED_LESSON_PLANS_ONLY = "feedlessonplansonly";
        public const string FEED_START_DATE = "feedstartdate";
        public const string FEED_END_DATE = "feedenddate";
    }
}
