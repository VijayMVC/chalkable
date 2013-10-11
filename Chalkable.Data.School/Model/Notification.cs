using System;

namespace Chalkable.Data.School.Model
{
    public class Notification
    {
        public const string ID_FIELD = "Id";
        public Guid Id { get; set; }
        public const string TYPE_FIELD = "Type";
        public NotificationType Type { get; set; }
        public string Message { get; set; }
        public const string SHOWN_FIELD = "Shown";
        public bool Shown { get; set; }
        public const string CREATED_FIELD = "Created";
        public DateTime Created { get; set; }
        public bool WasSend { get; set; }
        public const string PERSON_REF_FIELD = "PersonRef";
        public Guid PersonRef { get; set; }
        public Guid? AnnouncementRef { get; set; }
        public Guid? PrivateMessageRef { get; set; }
        public Guid? ApplicationRef { get; set; }
        public Guid? QuestionPersonRef { get; set; }
        public Guid? MarkingPeriodRef { get; set; }
        public const string CLASS_PERIOD_REF_FIELD = "ClassPeriodRef";
        public Guid? ClassPeriodRef { get; set; }
    }


    public class NotificationDetails : Notification
    {
        public Person Person { get; set; }
        public Announcement Announcement { get; set; }
        public AnnouncementType AnnouncementType { get; set; }
        public PrivateMessageDetails PrivateMessage { get; set; }
        public Person QuestionPerson { get; set; }
        public MarkingPeriod MarkingPeriod { get; set; }
        public ClassPeriod ClassPeriod { get; set; }
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
