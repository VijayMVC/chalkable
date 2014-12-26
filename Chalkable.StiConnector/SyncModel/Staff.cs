using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class Staff
    {
        public int StaffID { get; set; }
        public Guid StaffGUID { get; set; }
        public string ConversionId { get; set; }
        public string StaffNumber { get; set; }
        public string AltStaffNumber { get; set; }
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
        public short? HighestDegreeEarnedID { get; set; }
        public short? MaritalStatusID { get; set; }
        public int? PrimarySchoolID { get; set; }
        public short? PrimaryClassificationID { get; set; }
        public short? ReligionID { get; set; }
        public short? ResidencyStatusID { get; set; }
        public int? ResidentSchoolZoneID { get; set; }
        public int? PhotographID { get; set; }
        public int? MailingAddressID { get; set; }
        public int? PhysicalAddressID { get; set; }
        public DateTime? DateHired { get; set; }
        public short? YearsExperience { get; set; }
        public string StateIDNumber { get; set; }
        public string CountryOfResidence { get; set; }
        public bool HasTenure { get; set; }
        public bool ProvidesHomeSchooling { get; set; }
        public bool IsItinerantWorker { get; set; }
        public string TelephonyPIN { get; set; }
        public string InternetPassword { get; set; }
        public bool IntegratedWithSETS { get; set; }
        public int? UserID { get; set; }
        public Guid RowVersion { get; set; }
        public Guid DistrictGuid { get; set; }
        public bool IsHispanic { get; set; }
        public short? CountryOfResidenceID { get; set; }
    }
}
