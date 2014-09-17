namespace Chalkable.StiConnector.SyncModel
{
    public class AttendanceSetting
    {
        public int AttendanceSettingId { get; set; }
        public int? SchoolId { get; set; }
        public short MissingReasonId { get; set; }
        public byte Unit { get; set; }
        public decimal AllDay { get; set; }
        public decimal HalfDay { get; set; }
        public decimal TardyDay { get; set; }
        public decimal AllPeriod { get; set; }
        public decimal HalfPeriod { get; set; }
        public decimal TardyPeriod { get; set; }
        public byte AllowTardy { get; set; }
        public decimal? TruantAbsences { get; set; }
        public decimal? TruantUnexcusedAbsences { get; set; }
        public short? TardyPeriodAbsenceReasonId { get; set; }
    
        public virtual AbsenceReason AbsenceReason { get; set; }
        public virtual AbsenceReason AbsenceReason1 { get; set; }
        public virtual School School { get; set; }
    }
}
