using System;

namespace Chalkable.Data.School.Model
{
    public class ClassPerson
    {
        public const string PERSON_REF_FIELD = "PersonRef";
        public const string CLASS_REF_FIELD = "ClassRef";
       
        public int PersonRef { get; set; }
        public int ClassRef { get; set; }
        public int SchoolRef { get; set; }
   }
}
