﻿using System;
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
        public string ConfirmationKey { get; set; }
    }

    public class School
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public const string SERVER_URL_FIELD = "ServerUrl";
        public string ServerUrl { get; set; }
        public Guid? DistrictRef { get; set; }
        public const string IS_EMPTY_FIELD = "IsEmpty";
        public bool IsEmpty { get; set; }
        public SchoolStatus Status { get; set; }
        public ImportSystemTypeEnum ImportSystemType { get; set; }
        public string TimeZone { get; set; }
    }

    public class SchoolUser
    {
        public Guid Id { get; set; }
        public Guid UserRef { get; set; }
        public Guid SchoolRef { get; set; }
        public int Role { get; set; }
        [DataEntityAttr]
        public User User { get; set; }
        [DataEntityAttr]
        public School School { get; set; }
    }

    public enum SchoolStatus
    {
        Created = 1,
        DataImported = 2,
        PersonalInfoImported = 3,
        GradeLevels = 4,
        MarkingPeriods = 5,
        BlockScheduling = 6,
        DailyPeriods = 7,
        ScheduleInfoImported = 8,
        InvitedUser = 9 ,
        TeacherLogged = 10,
        InvitedStudent = 11,
        StudentLogged = 12 ,
        PayingCustomer = 13
    }
}
