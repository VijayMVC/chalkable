﻿using System;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.Data.School.Model
{
    public class Notification
    {
        public const string ID_FIELD = nameof(Id);
        public const string TYPE_FIELD = nameof(Type);
        public const string SHOWN_FIELD = nameof(Shown);
        public const string CREATED_FIELD = nameof(Created);
        public const string PERSON_REF_FIELD = nameof(PersonRef);
        public const string ROLE_REF_FIELD = nameof(RoleRef);

        [IdentityFieldAttr]
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public NotificationType Type { get; set; }
        public string Message { get; set; }
        public bool Shown { get; set; }
        public int PersonRef { get; set; }
        public int? AnnouncementRef { get; set; }
        public int? PrivateMessageRef { get; set; }
        public Guid? ApplicationRef { get; set; }
        public int? QuestionPersonRef { get; set; }
        public DateTime Created { get; set; }
        public int? MarkingPeriodRef { get; set; }
        public bool WasSend { get; set; }
        public int RoleRef { get; set; }
    }


    public class NotificationDetails : Notification
    {
        public Person Person { get; set; }
        public Announcement Announcement { get; set; }
        public ClassAnnouncementType ClassAnnouncementType { get; set; }
        public PrivateMessage PrivateMessage { get; set; }
        public Person QuestionPerson { get; set; }
        public MarkingPeriod MarkingPeriod { get; set; }
    }

    
    public enum NotificationType
    {
        Simple = 0,
        Announcement = 1,
        Message = 2,
        Question = 3,
        ItemToGrade = 4,
        AppBudgetBallance = 5,
        Application = 6,
        MarkingPeriodEnding = 7,
        Attendance = 8,
        NoTakeAttendance = 9,
        ReportProcessing = 10
    }
}
