using System;

namespace Chalkable.Data.School.Model
{
    public class ClassPerson
    {
        public const string ID_FIELD = "Id";
        public Guid Id { get; set; }
        public const string PERSON_REF_FIELD = "PersonRef";
        public Guid PersonRef { get; set; }
        public const string CLASS_REF_FIELD = "ClassRef";
        public Guid ClassRef { get; set; }
   }
}
