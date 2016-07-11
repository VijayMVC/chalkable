using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Common;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.DisciplinesViewData;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class StudentInfoViewData : PersonInfoViewData
    {
        public IdNameViewData<int> GradeLevel { get; set; }
        public IList<StudentHealthConditionViewData> HealthConditions { get; set; }
        public bool HasMedicalAlert { get; set; }
        public bool IsAllowedInetAccess { get; set; }
        public string SpecialInstructions { get; set; }
        public string SpEdStatus { get; set; }
        public string CurrentAttandanceLevel { get; set; }
        public bool IsIEPActive { get; set; }
        public bool IsTitle1Eligible { get; set; }
        public bool Section504 { get; set; }
        public bool IsHomeless { get; set; }
        public IdNameViewData<int> Language { get; set; }
        public bool IsImmigrant { get; set; }
        public IdNameViewData<int> Nationality { get; set; }
        public bool IsHispanic { get; set; }
        public bool Lep { get; set; }
        public bool IsForeignExchange { get; set; }
        public string StateIdNumber { get; set; }
        public string AlternateStudentNumber { get; set; }
        public string StudentNumber { get; set; }
        public DateTime? OriginalEnrollmentDate { get; set; }
        public bool IsRetained { get; set; }
        public ShortPersonViewData Counselor { get; set; }
        public HomeroomViewData Homeroom { get; set; }
        public IList<StudentCustomAlertDetailViewData> StudentCustomAlertDetails { get; set; }  
        public IList<StudentContactViewData> StudentContacts { get; set; }
        public StudentHoverBoxViewData<TotalAbsencesPerClassViewData> AttendanceBox { get; set; }
        public StudentHoverBoxViewData<DisciplineTypeSummaryViewData> DisciplineBox { get; set; }
        public StudentHoverBoxViewData<StudentSummaryGradeViewData> GradesBox { get; set; }

        private const string NO_CLASS_SCHEDULED = "No Class Scheduled";

        public string CurrentClassName { get; set; }
        public int? RoomId { get; set; }
        public string RoomName { get; set; }
        public string RoomNumber { get; set; }

        protected StudentInfoViewData(PersonDetails student):base(student)
        {
            var gradeLevels = student.StudentSchoolYears
                .OrderBy(x => x.SchoolYearRef)
                .Select( x => IdNameViewData<int>.Create(x.GradeLevelRef, x.GradeLevel.Name))
                .ToList();
            GradeLevel = gradeLevels.LastOrDefault();
        }

        public static new StudentInfoViewData Create(PersonDetails student)
        {
            return new StudentInfoViewData(student);
        }

        public static StudentInfoViewData Create(PersonDetails student, StudentDetailsInfo studentDetails, StudentSummaryInfo studentSummary, 
            IList<ClassDetails> studentClasses, ClassDetails currentClass, Room currentRoom, int currentSchoolYearId, DateTime today)
        {
            var res = Create(student);

            res.DisplayName = studentDetails.DisplayName(includeMiddleName: true);

            var gradeLevels = student.StudentSchoolYears
                .OrderBy(x => x.SchoolYearRef)
                .Select( x => IdNameViewData<int>.Create(x.GradeLevelRef, x.GradeLevel.Name))
                .ToList();
            var currentStudentSchoolYear = student.StudentSchoolYears.FirstOrDefault(x => x.SchoolYearRef == currentSchoolYearId);

            if (currentStudentSchoolYear != null)
                res.GradeLevel = gradeLevels.First(x => x.Id == currentStudentSchoolYear.GradeLevelRef);

            res.IsHispanic = studentDetails.IsHispanic;
            res.HasMedicalAlert = studentDetails.HasMedicalAlert;
            res.IsAllowedInetAccess = studentDetails.IsAllowedInetAccess;
            res.SpecialInstructions = studentDetails.SpecialInstructions;
            res.SpEdStatus = studentDetails.SpEdStatus;
            res.IsIEPActive = studentDetails.IsIEPActive(today);
            res.IsTitle1Eligible = studentDetails.StudentSchool.IsTitle1Eligible;
            res.Section504 = !string.IsNullOrWhiteSpace(studentDetails.Section504Qualification) && studentDetails.Section504Qualification != "NA";
            res.IsHomeless = studentDetails.IsHomeless;
            res.IsImmigrant = studentDetails.IsImmigrant;
            res.Language = studentDetails.Language != null
                ? IdNameViewData<int>.Create(studentDetails.Language.Id, studentDetails.Language.Name)
                : null;            
            res.Nationality = studentDetails.Country != null
                ? IdNameViewData<int>.Create(studentDetails.Country.Id, studentDetails.Country.Name)
                : null;
            res.Ethnicity = studentDetails.Ethnicity != null
                ? EthnicityViewData.Create(studentDetails.Ethnicity)
                : null; 
            res.Lep = studentDetails.LimitedEnglishRef.HasValue;
            res.IsForeignExchange = studentDetails.IsForeignExchange;
            res.StateIdNumber = studentDetails.StateIdNumber;
            res.AlternateStudentNumber = studentDetails.AltStudentNumber;
            res.StudentNumber = studentDetails.StudentNumber;
            res.OriginalEnrollmentDate = studentDetails.OriginalEnrollmentDate;
            res.IsRetained = student.StudentSchoolYears.First(x => x.SchoolYearRef == currentSchoolYearId).IsRetained;
            res.Counselor = studentDetails.Counselor != null 
                ? ShortPersonViewData.Create(studentDetails.Counselor)
                : null;

            res.AttendanceBox = StudentHoverBoxViewData<TotalAbsencesPerClassViewData>.Create(studentSummary.DailyAttendance,
                    studentSummary.Attendances, studentClasses);
            res.DisciplineBox = StudentHoverBoxViewData<DisciplineTypeSummaryViewData>.Create(studentSummary.InfractionSummaries,
                    studentSummary.TotalDisciplineOccurrences);
            res.GradesBox = StudentHoverBoxViewData<StudentSummaryGradeViewData>.Create(studentSummary.StudentAnnouncements);

            res.CurrentAttandanceLevel = studentSummary.CurrentAttendanceLevel;

            res.CurrentClassName = NO_CLASS_SCHEDULED;

            if (currentClass != null)
                res.CurrentClassName = currentClass.Name;



            return res;
        }
        
    }


   
}