using System;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models
{
    public class SisSyncViewData
    {
        public Guid  Id { get; set; }
        public int? AttendanceSyncFreq { get; set; }
        public int? DisciplineSyncFreq { get; set; }
        public int? PersonSyncFreq { get; set; }
        public int? ScheduleSyncFreq { get; set; }
        public string SisDatabaseUrl { get; set; }
        public string SisDatabaseName { get; set; }
        public string SisDatabaseUserName { get; set; }
        public string SisDatabasePassword { get; set; }

        public static SisSyncViewData Create(SisSync sisSync)
        {
            return new SisSyncViewData
            {
                Id = sisSync.Id,
                AttendanceSyncFreq = sisSync.AttendanceSyncFrequency,
                DisciplineSyncFreq =  sisSync.DisciplineSyncFrequency,
                PersonSyncFreq = sisSync.PersonSyncFrequency,
                ScheduleSyncFreq = sisSync.PersonSyncFrequency,
                SisDatabaseName = sisSync.SisDatabaseName,
                SisDatabasePassword = sisSync.SisDatabasePassword,
                SisDatabaseUserName = sisSync.SisDatabaseUserName,
                SisDatabaseUrl = sisSync.SisDatabaseUrl
            };
        }
    }
}