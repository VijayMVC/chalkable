using System;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class Notification
    {
        public const string ID_FIELD = "Id";
        public const string TYPE_FIELD = "Type";
        public const string SHOWN_FIELD = "Shown";
        public const string CREATED_FIELD = "Created";
        public const string PERSON_REF_FIELD = "PersonRef";
        public const string CLASS_PERIOD_REF_FIELD = "ClassPeriodRef";


        [IdentityFieldAttr]
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public NotificationType Type { get; set; }
        public string Message { get; set; }
        public bool Shown { get; set; }
        public DateTime Created { get; set; }
        public bool WasSend { get; set; }
        public int PersonRef { get; set; }
        public int? AnnouncementRef { get; set; }
        public int? PrivateMessageRef { get; set; }
        public Guid? ApplicationRef { get; set; }
        public int? QuestionPersonRef { get; set; }
        public int? MarkingPeriodRef { get; set; }
    }


    public class NotificationDetails : Notification
    {
        public Person Person { get; set; }
        public Announcement Announcement { get; set; }
        public ClassAnnouncementType AnnouncementType { get; set; }
        public PrivateMessageDetails PrivateMessage { get; set; }
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
        NoTakeAttendance = 9
    }
}
