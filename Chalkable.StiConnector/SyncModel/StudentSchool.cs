using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class StudentSchool
    {
        public int StudentID { get; set; }
        public int SchoolID { get; set; }
        public int? GradingScaleID { get; set; }
        public int? CounselorID { get; set; }
        public short? TransportMethodID { get; set; }
        public string ParkingNumber { get; set; }
        public string LockerNumber { get; set; }
        public string LockerCombination { get; set; }
        public bool IsResponsibleForLock { get; set; }
        public decimal? DistanceFromSchool { get; set; }
        public bool ResidesOutOfSchoolZone { get; set; }
        public bool IsTitle1Eligible { get; set; }
        public bool IncludeInClassRank { get; set; }
        public int? AMBusID { get; set; }
        public short? AMBusStopNumber { get; set; }
        public short? AMBusStopTime { get; set; }
        public int? PMBusID { get; set; }
        public short? PMBusStopNumber { get; set; }
        public short? PMBusStopTime { get; set; }
        public Guid RowVersion { get; set; }
        public Guid DistrictGuid { get; set; }
    }
}
