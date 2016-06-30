using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Model
{
    public class StudentDetailsInfo : Student
    {
        public Ethnicity Ethnicity { get; set; }
        public Language Language { get; set; }
        public Country Country { get; set; }
        public Person Counselor { get; set; }
        public StudentSchool StudentSchool { get; set; }

        public static StudentDetailsInfo Create(StudentDetails student, Ethnicity ethnicity, Language language, Country country, 
            Person counselor, StudentSchool studentSchool)
        {
            return new StudentDetailsInfo
            {
                Id = student.Id,
                AltStudentNumber = student.AltStudentNumber,
                BirthDate = student.BirthDate,
                FirstName = student.FirstName,
                Gender = student.Gender,
                GenderDescriptor = student.GenderDescriptor,
                HasMedicalAlert = student.HasMedicalAlert,
                IEPBeginDate = student.IEPBeginDate,
                IEPEndDate = student.IEPEndDate,
                IsAllowedInetAccess = student.IsAllowedInetAccess,
                IsForeignExchange = student.IsForeignExchange,
                IsHispanic = student.IsHispanic,
                IsHomeless = student.IsHomeless,
                IsImmigrant = student.IsImmigrant,
                IsWithdrawn = student.IsWithdrawn,
                LastName = student.LastName,
                LimitedEnglishRef = student.LimitedEnglishRef,
                OriginalEnrollmentDate = student.OriginalEnrollmentDate,
                PhotoModifiedDate = student.PhotoModifiedDate,
                UserId = student.UserId,
                StudentNumber = student.StudentNumber,
                Section504Qualification = student.Section504Qualification,
                StateIdNumber = student.StateIdNumber,
                SpecialInstructions = student.SpecialInstructions,
                SpEdStatus = student.SpEdStatus,
                Ethnicity = ethnicity,
                Language = language,
                Country = country,
                Counselor = counselor,
                StudentSchool = studentSchool
            };
        }

        public static IList<StudentDetailsInfo> Create(IList<StudentDetails> students, IList<Ethnicity> ethnicities,
            IList<Language> languages, IList<Country> countries, IList<Person> counselors, int currentSchoolId)
        {
            var res = new List<StudentDetailsInfo>();
            foreach (var student in students)
            {
                Ethnicity ethnicity = null;
                Language language = null;
                Country country = null;
                Person person = null;

                if (student.PrimaryPersonEthnicity != null)
                    ethnicity = ethnicities.FirstOrDefault(x => x.Id == student.PrimaryPersonEthnicity.EthnicityRef);

                if (student.PrimaryPersonLanguage != null)
                    language = languages.FirstOrDefault(x => x.Id == student.PrimaryPersonLanguage.LanguageRef);

                if (student.PrimaryPersonNationality != null)
                    country = countries.FirstOrDefault(x => x.Id == student.PrimaryPersonNationality.CountryRef);

                var studentSchool = student.StudentSchools.First(x => x.SchoolRef == currentSchoolId);
                if (studentSchool.CounselorRef.HasValue)
                    person = counselors.FirstOrDefault(x => x.Id == studentSchool.CounselorRef.Value);

                res.Add(Create(student, ethnicity, language, country, person, studentSchool));
            }

            return res;
        }
    }
}
