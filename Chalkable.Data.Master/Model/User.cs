using System;
using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Enums;

namespace Chalkable.Data.Master.Model
{
    public class User
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        [NotDbFieldAttr]
        public IList<SchoolUser> SchoolUsers { get; set; }
        public bool IsSysAdmin { get; set;}
        public bool IsDeveloper { get; set; }
        public const string CONFIRMATION_KEY_FIELD = "ConfirmationKey";
        public string ConfirmationKey { get; set; }
    }

    public class School
    {
        public const string ID_FIELD = "Id";
        public Guid Id { get; set; }
        public string Name { get; set; }
        public const string SERVER_URL_FIELD = "ServerUrl";
        public string ServerUrl { get; set; }
        public const string DISTRICT_REF_FIELD = "DistrictRef";
        public Guid? DistrictRef { get; set; }
        public const string IS_EMPTY_FIELD = "IsEmpty";
        public bool IsEmpty { get; set; }
        public SchoolStatus Status { get; set; }
        public ImportSystemTypeEnum ImportSystemType { get; set; }
        public string TimeZone { get; set; }
        public const string DEMO_PREFIX_FIELD = "DemoPrefix";
        public string DemoPrefix { get; set; }
        public const string LAST_USED_DEMO_FIELD = "LastUseDemo";
        public DateTime? LastUseDemo { get; set; }
    }

    public class SchoolUser
    {
        public Guid Id { get; set; }
        public Guid UserRef { get; set; }
        public Guid SchoolRef { get; set; }
        public const string ROLE_FIELD = "Role";
        public int Role { get; set; }
        [DataEntityAttr]
        public User User { get; set; }
        [DataEntityAttr]
        public School School { get; set; }
    }

    public enum SchoolStatus
    {
        Created = 0,
        DataImported = 1,
        PersonalInfoImported = 2,
        GradeLevels = 3,
        MarkingPeriods = 4,
        BlockScheduling = 5,
        DailyPeriods = 6,
        ScheduleInfoImported = 7,
        InvitedUser = 8 ,
        TeacherLogged = 9,
        InvitedStudent = 10,
        StudentLogged = 11 ,
        PayingCustomer = 12
    }
}
