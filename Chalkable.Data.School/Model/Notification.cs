using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class Notification
    {
        public Guid Id { get; set; }
        public NotificationType Type { get; set; }
        public string Message { get; set; }
        public bool Shown { get; set; }
        public DateTime Created { get; set; }
        public bool WasSend { get; set; }
        public Guid PersonRef { get; set; }
        public Guid? AnnouncementRef { get; set; }
        public Guid? PrivateMessageRef { get; set; }
        public Guid? ApplicationRef { get; set; }
        public Guid? QuestionPersonRef { get; set; }
        public Guid? MarkingPeriodRef { get; set; }
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
