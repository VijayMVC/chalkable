using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class AutoGrade
    {
        public const string ANNOUNCEMENT_APPLICATION_REF_FIELD = "AnnouncementApplicationRef";
        public const string STUDENT_REF_FIELD = "StudentRef";

        [PrimaryKeyFieldAttr]
        public int AnnouncementApplicationRef { get; set; }
        [PrimaryKeyFieldAttr]
        public int StudentRef { get; set; }
        public DateTime Date { get; set; }
        public bool Posted { get; set; }
        public string Grade { get; set; }

        [DataEntityAttr]
        public AnnouncementApplication AnnouncementApplication { get; set; }
    }
}
