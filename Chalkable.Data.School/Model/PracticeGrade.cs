using System;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class PracticeGrade
    {
        public const string ID_FIELD = "Id";
        public const string STUDENT_ID_FIELD = "StudentId";
        public const string STANDARD_ID_FIELD = "StandardId";
        public const string APPLICATION_REF_FIELD = "ApplicationRef";
        public const string DATE_FIELD = "Date";

        [PrimaryKeyFieldAttr]
        [IdentityFieldAttr]
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int StandardId { get; set; }
        public Guid ApplicationRef { get; set; }
        public string Score { get; set; }
        public DateTime Date { get; set; }
    }
}
