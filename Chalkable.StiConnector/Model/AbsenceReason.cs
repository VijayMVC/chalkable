//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Chalkable.StiConnector.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class AbsenceReason
    {
        public AbsenceReason()
        {
            this.AbsenceLevelReasons = new HashSet<AbsenceLevelReason>();
            this.AttendanceSettings = new HashSet<AttendanceSetting>();
            this.AttendanceSettings1 = new HashSet<AttendanceSetting>();
            this.StudentCheckInOuts = new HashSet<StudentCheckInOut>();
            this.StudentDailyAbsences = new HashSet<StudentDailyAbsence>();
            this.StudentPeriodAbsences = new HashSet<StudentPeriodAbsence>();
        }
    
        public short AbsenceReasonID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string AbsenceCategory { get; set; }
        public string StateCode { get; set; }
        public string SIFCode { get; set; }
        public string NCESCode { get; set; }
        public bool IsActive { get; set; }
        public bool IsSystem { get; set; }
        public System.Guid RowVersion { get; set; }
        public System.Guid DistrictGuid { get; set; }
    
        public virtual ICollection<AbsenceLevelReason> AbsenceLevelReasons { get; set; }
        public virtual ICollection<AttendanceSetting> AttendanceSettings { get; set; }
        public virtual ICollection<AttendanceSetting> AttendanceSettings1 { get; set; }
        public virtual ICollection<StudentCheckInOut> StudentCheckInOuts { get; set; }
        public virtual ICollection<StudentDailyAbsence> StudentDailyAbsences { get; set; }
        public virtual ICollection<StudentPeriodAbsence> StudentPeriodAbsences { get; set; }
    }
}
