using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class School
    {
        public int SchoolID { get; set; }
        public Guid SchoolGUID { get; set; }
        public string SchoolNumber { get; set; }
        public string Name { get; set; }
        public short? SchoolCategoryID { get; set; }
        public string TelephoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string EmailAddress { get; set; }
        public int? MailingAddressID { get; set; }
        public int? PhysicalAddressID { get; set; }
        public string Title1Status { get; set; }
        public string ReportedSchoolNumber { get; set; }
        public string NCESNumber { get; set; }
        public string ACTLocationCode { get; set; }
        public string SATLocationCode { get; set; }
        public int? SchoolLogoID { get; set; }
        public string SchoolColor { get; set; }
        public bool ReportToState { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsActive { get; set; }
        public Guid RowVersion { get; set; }
        public Guid DistrictGuid { get; set; }
        public int? CourseRequestSessionID { get; set; }
        public bool IsLEEnabled { get; set; }
        public bool IsLESyncComplete { get; set; }
        public bool IsChalkableEnabled { get; set; }
    }
}
