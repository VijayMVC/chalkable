using System;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class Student
    {
        public const string ID_FIELD = "Id";
        public const string USER_ID_FIELD = "UserId";

        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; }
        public bool HasMedicalAlert { get; set; }
        public bool IsAllowedInetAccess { get; set; }
        public string SpecialInstructions { get; set; }
        public string SpEdStatus { get; set; }
        public DateTime? PhotoModifiedDate { get; set; }
        public int UserId { get; set; }
        public bool IsHispanic { get; set; }
        public DateTime? IEPBeginDate { get; set; }
        public DateTime? IEPEndDate { get; set; }

        public bool IsIEPActive(DateTime now)
        {
            if (IEPBeginDate.HasValue)
            {
                if (now >= IEPBeginDate.Value && !IEPEndDate.HasValue)
                    return true;

                if (IEPEndDate.HasValue && now >= IEPBeginDate.Value && now <= IEPEndDate.Value)
                    return true;
            }

            return false;
        }
    }

    public class StudentDetails : Student
    {
        public bool? IsWithdrawn { get; set; }
    }
}
