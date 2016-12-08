using System;
using System.Collections.Generic;
using System.Linq;
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
        public string MiddleName { get; set; }
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
        public string Section504Qualification { get; set; }
        public string GenderDescriptor { get; set; }
        public bool IsHomeless { get; set; }
        public bool IsImmigrant { get; set; }
        public int? LimitedEnglishRef { get; set; }
        public bool IsForeignExchange { get; set; }
        public string StateIdNumber { get; set; }
        public string AltStudentNumber { get; set; }
        public string StudentNumber { get; set; }
        public DateTime? OriginalEnrollmentDate { get; set; }

        public bool IsIEPActive
        {
            get
            {
                var now = DateTime.UtcNow;

                if (!IEPBeginDate.HasValue)
                    return false;

                if (now >= IEPBeginDate.Value && !IEPEndDate.HasValue)
                    return true;

                if (IEPEndDate.HasValue && now >= IEPBeginDate.Value && now <= IEPEndDate.Value)
                    return true;

                return false;
            }
        }

        [DataEntityAttr]
        public bool? IsWithdrawn { get; set; }
    }

    public class StudentDetails : Student
    {
        public IList<StudentSchool> StudentSchools { get; set; }
        public IList<PersonEthnicity> PersonEthnicities { get; set; }
        public PersonEthnicity PrimaryPersonEthnicity => PersonEthnicities?.FirstOrDefault(x => x.IsPrimary);
        
        public IList<PersonNationality> PersonNationalities { get; set; }
        public PersonNationality PrimaryPersonNationality => PersonNationalities?.FirstOrDefault(x => x.IsPrimary);
        
        public IList<PersonLanguage> PersonLanguages { get; set; }
        public PersonLanguage PrimaryPersonLanguage => PersonLanguages?.FirstOrDefault(x => x.IsPrimary);
    }

    public class StudentSchoolsInfo : Student
    {
        public IList<School> StudentSchools { get; set; }
        public GradeLevel GradeLevel { get; set; }

        public bool IsClassmate { get; set; }
        public bool IsMyStudent { get; set; }
    }
}
