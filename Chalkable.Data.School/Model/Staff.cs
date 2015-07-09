using System;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class Staff
    {
        public const string USER_ID_FIELD = "UserId";

        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; }
        public int? UserId { get; set; }
    }
}
