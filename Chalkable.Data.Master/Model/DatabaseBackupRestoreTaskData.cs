namespace Chalkable.Data.Master.Model
{
    public class DatabaseBackupRestoreTaskData
    {
        public long Time { get; set; }
        public bool BackupMaster { get; set; }
        private const string FORMAT = "{0},{1}";
        public override string ToString()
        {
            return string.Format(FORMAT, Time, BackupMaster);
        }
        public DatabaseBackupRestoreTaskData(long time, bool backupMaster)
        {
            Time = time;
            BackupMaster = backupMaster;
        }
        public DatabaseBackupRestoreTaskData(string str)
        {
            var sl = str.Split(',');
            Time = long.Parse(sl[0]);
            BackupMaster = bool.Parse(sl[1]);
        }
    }
}