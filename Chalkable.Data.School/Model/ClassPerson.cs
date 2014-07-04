using System;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class ClassPerson
    {
        public const string PERSON_REF_FIELD = "PersonRef";
        public const string CLASS_REF_FIELD = "ClassRef";
        public const string MARKING_PERIOD_REF = "MarkingPeriodRef";
        public const string IS_ENROLLED_FIELD = "IsEnrolled";

        [PrimaryKeyFieldAttr]
        public int ClassRef { get; set; }
        [PrimaryKeyFieldAttr]
        public int PersonRef { get; set; }
        [PrimaryKeyFieldAttr]
        public int MarkingPeriodRef { get; set; }
        public int SchoolRef { get; set; }
        public bool IsEnrolled { get; set; }
   }
}
