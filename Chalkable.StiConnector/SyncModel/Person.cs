using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class Person
    {
        public int PersonID { get; set; }
        public Guid PersonGUID { get; set; }
        public string ConversionId { get; set; }
        public string PersonNumber { get; set; }
        public string StudentNumber { get; set; }
        public string StaffNumber { get; set; }
        public string AltPersonNumber { get; set; }
        public string DisplayName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PreferredName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string SocialSecurityNumber { get; set; }
        public short? ProperTitleID { get; set; }
        public short? GenerationID { get; set; }
        public short? GenderID { get; set; }
        public string GenderDescriptor { get; set; }
        public short? EducationLevelID { get; set; }
        public short? MaritalStatusID { get; set; }
        public short? ReligionID { get; set; }
        public short? ResidencyStatusID { get; set; }
        public int? PhotographID { get; set; }
        public int? MailingAddressID { get; set; }
        public int? PhysicalAddressID { get; set; }
        public int? UserID { get; set; }
        public string CountryOfResidence { get; set; }
        public string EmployerName { get; set; }
        public bool IsHomeless { get; set; }
        public Guid RowVersion { get; set; }
        public int? SpouseId { get; set; }
        public Guid DistrictGuid { get; set; }
        public bool IsGoGreen { get; set; }
        public bool IsHispanic { get; set; }
        public short? CountryOfResidenceID { get; set; }
        public bool RecievesAttendanceAlerts { get; set; }
        public bool RecievesDisciplineAlerts { get; set; }
        public bool RecievesGradeAlerts { get; set; }
        public DateTime? PhotoModifiedDate { get; set; }
    }
}
