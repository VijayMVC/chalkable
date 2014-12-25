using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.Common;

namespace Chalkable.Data.Master.Model
{
    public class BackgroundTask
    {
        public const string DISTRICT_REF_FIELD_NAME = "DistrictRef";
        public Guid? DistrictRef { get; set; }
        public const string ID_FIELD_NAME = "Id";
        [PrimaryKeyFieldAttr]
        public Guid Id { get; set; }
        public const string TYPE_FIELD_NAME = "Type";
        public BackgroundTaskTypeEnum Type { get; set; }
        public const string STATE_FIELD_NAME = "State";
        public BackgroundTaskStateEnum State { get; set; }
        public const string SCHEDULED_FIELD_NAME = "Scheduled";
        public DateTime Scheduled { get; set; }
        public const string COMPLETED_FIELD_NAME = "Completed";
        public DateTime? Completed { get; set; }
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

    public enum BackgroundTaskTypeEnum
    {
        SisDataImport = 1,
        BackupDatabases = 2,
        RestoreDatabases = 3,
        DatabaseUpdate = 4,
        DeleteDistrict = 6,
        ProcessReminder = 7,
        AttendanceNotification = 8,
        TeacherAttendanceNotification = 9,
        PictureImport = 10
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
