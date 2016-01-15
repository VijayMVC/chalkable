using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models
{
    public class MessagingSettingsViewData
    {
        public bool StudentMessagingEnabled { get; set; }
        public bool StudentToClassMessagingOnly { get; set; }
        public bool TeacherToStudentMessaginEnabled { get; set; }
        public bool TeacherToClassMessagingOnly { get; set; }

        public static MessagingSettingsViewData Create(bool studentMessaging, bool studentToClassOnly, bool teacherToStudentMessaging, bool teacherToClassOnly)
        {
            return new MessagingSettingsViewData
            {
                StudentMessagingEnabled = studentMessaging,
                StudentToClassMessagingOnly = studentToClassOnly,
                TeacherToStudentMessaginEnabled = teacherToStudentMessaging,
                TeacherToClassMessagingOnly = teacherToClassOnly
            };
        }

        public static MessagingSettingsViewData Create(MessagingSettings messagingSettings)
        {
            return Create(
                messagingSettings.StudentMessagingEnabled,
                messagingSettings.StudentToClassMessagingOnly,
                messagingSettings.TeacherToStudentMessaginEnabled,
                messagingSettings.TeacherToClassMessagingOnly
            );
        }
    }
}