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
        

        public static SisSyncViewData Create(SisSync sisSync)
        {
            return new SisSyncViewData
            {
                Id = sisSync.Id,
                AttendanceSyncFreq = sisSync.AttendanceSyncFrequency,
                DisciplineSyncFreq =  sisSync.DisciplineSyncFrequency,
                PersonSyncFreq = sisSync.PersonSyncFrequency,
                ScheduleSyncFreq = sisSync.PersonSyncFrequency
            };
        }
    }
}