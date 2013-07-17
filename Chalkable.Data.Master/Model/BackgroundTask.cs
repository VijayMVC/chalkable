using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;

namespace Chalkable.Data.Master.Model
{
    public class BackgroundTask
    {
        public const string SCHOOL_REF_FIELD_NAME = "SchoolRef";
        public Guid? SchoolRef { get; set; }
        public const string ID_FIELD_NAME = "Id";
        public Guid Id { get; set; }
        public const string TYPE_FIELD_NAME = "Type";
        public BackgroundTaskTypeEnum Type { get; set; }
        public const string STATE_FIELD_NAME = "State";
        public BackgroundTaskStateEnum State { get; set; }
        public const string SCHEDULED_FIELD_NAME = "Scheduled";
        public DateTime Scheduled { get; set; }
        public DateTime Created { get; set; }
        public string Data { get; set; }
        public DateTime? Started { get; set; }

        public void SetData(object data)
        {
            Data = data.ToString();
        }

        public T GetData<T>()
        {
            var res = (T)Activator.CreateInstance(typeof(T), new object[] { Data });
            return res;
        }
    }

    public class SisImportTaskData
    {
        private const string FORMAT = "{0},{1},{2}";
        public Guid SchoolId { get; private set; }
        public int SisSchoolId { get; private set; }
        public IList<int> SchoolYearIds { get; private set; }

        public override string ToString()
        {
            return string.Format(FORMAT, SchoolId, SisSchoolId, SchoolYearIds.JoinString(","));
        }

        public SisImportTaskData(Guid schoolId, int sisSchoolId, IList<int> schoolYearIds)
        {
            SchoolId = schoolId;
            SisSchoolId = sisSchoolId;
            SchoolYearIds = schoolYearIds;
        }

        public SisImportTaskData(string str)
        {
            var intList = str.Split(',').ToList();
            SchoolId = Guid.Parse(intList[0]);
            SisSchoolId = int.Parse(intList[1]);
            SchoolYearIds = intList.Skip(2).Select(int.Parse).ToList();
        }

    }

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

    public enum BackgroundTaskTypeEnum
    {
        CreateEmptySchool = 0,
        SisDataImport = 1,
        BackupDatabases = 2,
        RestoreDatabases = 3,
        DatabaseUpdate = 4

    }

    public enum BackgroundTaskStateEnum
    {
        Created = 0,
        Processing = 1,
        Processed = 2,
        Canceled = 3,
        Failed = 4
    }
}
