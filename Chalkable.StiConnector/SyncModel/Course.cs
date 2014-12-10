
namespace Chalkable.StiConnector.SyncModel
{
    using System;
    
    public class Course
    {
        public int CourseID { get; set; }
        public string CourseNumber { get; set; }
        public int? SectionOfCourseID { get; set; }
        public string SectionNumber { get; set; }
        public string FullSectionNumber { get; set; }
        public int? AcadSessionID { get; set; }
        public int? GradingScaleID { get; set; }
        public string AltCourseNumber { get; set; }
        public string StateCourseNumber { get; set; }
        public short? ClassInstProgramID { get; set; }
        public string ShortName { get; set; }
        public string FullName { get; set; }
        public short? MinGradeLevelID { get; set; }
        public short? MaxGradeLevelID { get; set; }
        public short CourseTypeID { get; set; }
        public int? PrimaryTeacherID { get; set; }
        public int? RoomID { get; set; }
        public string AllowedGenders { get; set; }
        public short? DefaultStudentCapacity { get; set; }
        public short? DifficultyLevelID { get; set; }
        public short? TeachingMethodID { get; set; }
        public short? FundingMethodID { get; set; }
        public short? InstructionalSettingID { get; set; }
        public decimal? GPACredit { get; set; }
        public decimal? GradCredit { get; set; }
        public bool IncludeInHonorRoll { get; set; }
        public bool IsInstructionalTime { get; set; }
        public bool IsStateReported { get; set; }
        public bool DisplayOnReportCard { get; set; }
        public short? ClassTypeId { get; set; }
        public Guid RowVersion { get; set; }
        public bool IsRequired { get; set; }
        public Guid DistrictGuid { get; set; }
        public bool Active { get; set; }
        public int? ReportingSchoolID { get; set; }
        public bool AreFeesPosted { get; set; }
        public bool MergeRostersForAttendance { get; set; }
    }
}
