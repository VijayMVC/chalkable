using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class Student : SyncModel
    {
        public int StudentID { get; set; }
        public Guid StudentGUID { get; set; }
        public string ConversionId { get; set; }
        public string StudentNumber { get; set; }
        public string DisplayName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PreferredName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string SocialSecurityNumber { get; set; }
        public int? NextYearSchoolID { get; set; }
        public short? ProperTitleID { get; set; }
        public short? GenerationID { get; set; }
        public short? GenderID { get; set; }
        public string GenderDescriptor { get; set; }
        public short? MaritalStatusID { get; set; }
        public short? ReligionID { get; set; }
        public short? ResidencyStatusID { get; set; }
        public int? PhotographID { get; set; }
        public int? MailingAddressID { get; set; }
        public string MailingAddressMultiLine { get; set; }
        public string MailingAddressSingleLine { get; set; }
        public bool? MailingAddressIsListed { get; set; }
        public int? PhysicalAddressID { get; set; }
        public bool ResidesOutOfDistrict { get; set; }
        [NullableForeignKey]
        public short? LimitedEnglishID { get; set; }
        public short? MigrantFamilyID { get; set; }
        public string BirthCertNumber { get; set; }
        public string BirthCertVerifyNumber { get; set; }
        public string Section504Qualification { get; set; }
        public string AltStudentNumber { get; set; }
        public string StateIDNumber { get; set; }
        public string CountryOfResidence { get; set; }
        public string EmployerName { get; set; }
        public bool IsHomeless { get; set; }
        public bool IsImmigrant { get; set; }
        public bool IsForeignExchange { get; set; }
        public bool IsAllowedInetAccess { get; set; }
        public bool HasMedicalAlert { get; set; }
        public bool IsGraduate { get; set; }
        public bool IsHomeSchooled { get; set; }
        public bool IsTuition { get; set; }
        public string TelephonyPIN { get; set; }
        public string InternetPassword { get; set; }
        public string TelephoneNumber { get; set; }
        public bool? IsListed { get; set; }
        public string EmailAddress { get; set; }
        public DateTime? OriginalEnrollmentDate { get; set; }
        public DateTime? ExpectedGraduationDate { get; set; }
        public string CachedSchoolList { get; set; }
        public string SpecialInstructions { get; set; }
        public short? SpEdStatusID { get; set; }
        public short? PrimaryExcepID { get; set; }
        public short? SecondaryExcepID { get; set; }
        public DateTime? SpecialEducationExitDate { get; set; }
        public short? SpecialEducationExitReasonID { get; set; }
        public short? LeastRestEnvID { get; set; }
        public int? CaseManagerID { get; set; }
        public DateTime? EligibilityDate { get; set; }
        public DateTime? ReevaluationDate { get; set; }
        public DateTime? IEPBeginDate { get; set; }
        public DateTime? IEPEndDate { get; set; }
        public DateTime? DateEnrolledInLEA { get; set; }
        public int? ReportingSchoolID { get; set; }
        public DateTime? USLEPEntryDate { get; set; }
        public DateTime? LEPExitDate { get; set; }
        public bool IntegratedWithSETS { get; set; }
        public int UserID { get; set; }
        public Guid RowVersion { get; set; }
        public Guid DistrictGuid { get; set; }
        public bool IsGoGreen { get; set; }
        public bool IsHispanic { get; set; }
        public short? CountryOfResidenceID { get; set; }
        public int? LocatorStudentId { get; set; }
        public short? HomelessStatusID { get; set; }
        public short? CohortYear { get; set; }
        public DateTime? NinthGradeEntry { get; set; }
        public bool RecievesAttendanceAlerts { get; set; }
        public bool RecievesDisciplineAlerts { get; set; }
        public bool RecievesGradeAlerts { get; set; }

        public override int DefaultOrder => 7;
    }
}
