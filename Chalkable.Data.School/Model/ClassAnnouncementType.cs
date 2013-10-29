using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.School.Model
{
    public class ClassAnnouncementType
    {
        public const string CLASS_REF_FIELD = "ClassRef";
        public const string PERCENTAGE_FIELD = "Percentage";

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Gradable { get; set; }
        public int Percentage { get; set; }
        public int AnnouncementTypeRef { get; set; }
        public int ClassRef { get; set; }
    }
}
