using System;

namespace Chalkable.Data.Master.Model
{
    public class SisSync
    {
        public Guid Id { get; set; }
        public DateTime LastAttendanceSync { get; set; }
        public int? AttendanceSyncFrequency { get; set; }
        public DateTime LastDisciplineSync { get; set; }
        public int? DisciplineSyncFrequency { get; set; }
        public DateTime LastPersonSync { get; set; }
        public int? PersonSyncFrequency { get; set; }
        public DateTime LastScheduleSync { get; set; }
        public int? ScheduleSyncFrequency { get; set; }
        public int? SisSchoolId { get; set; }
        public string SisDatabaseUrl { get; set; }
        public string SisDatabaseName { get; set; }
        public string SisDatabaseUserName { get; set; }
        public string SisDatabasePassword { get; set; }
    }
}