﻿using System.Collections.Generic;

namespace Chalkable.BusinessLogic.Services.School.Notifications
{
    public static class NotificationTemplateProvider
    {
        private static Dictionary<string, string> templates = new Dictionary<string, string>();

        public const string ANNOUNCEMENT_NEW_ATTACHMENT_NOTIFICATION = "ANNOUNCEMENT_NEW_ATTACHMENT_NOTIFICATION";
        private const string ANNOUNCEMENT_ATTACHMENT_NOTIFICATION_TEMPLATE = "New attachment is added to announcement <a href='@0#announcement/view/^.Notification.AnnouncementRef'>^.Other.AnnouncementTitle</a>";

        public const string ANNOUNCEMENT_REMINDER_NOTIFICATION = "ANNOUNCEMENT_REMINDER_NOTIFICATION";
        private const string ANNOUNCEMENT_REMINDER_NOTIFICATION_TEMPLATE = "Reminder about announcement: announcement <a href='@0#announcement/view/^.Notification.AnnouncementRef'>^.Other.AnnouncementTitle</a>";

        public const string ANNOUNCEMENT_QUESTION_NOTIFICATION_TOAUTHOR = "ANNOUNCEMENT_QUESTION_NOTIFICATION_TOAUTHOR";
        private const string ANNOUNCEMENT_QUESTION_NOTIFICATION_TOAUTHOR_TEMPLATE = "Student <a href='@0#students/details/^.Notification.PersonRef'>^.Notification.QuestionPerson.ShortSalutationName</a> " +
                                                                                    "asked question about announcement <a href='@0#announcement/view/^.Notification.AnnouncementRef'>" +
                                                                                   "^.Other.AnnouncementTitle ^.Other.AnnouncementTypeName</a> ^.Other.PersonQuestion";

        public const string ANNOUNCEMENT_NOTIFICATION_ANSWER_TO_PERSON = "ANNOUNCEMENT_NOTIFICATION_ANSWER_TO_PERSON";
        private const string ANNOUNCEMENT_NOTIFICATION_ASWER_TO_PERSON_TEMPLATE = "<a href='@0#teachers/details/^.Notification.PersonRef'>^.Notification.Person.SalutationName" +
                                                                            " ^.Notification.Person.LastName</a> answered a question about <a href='@0#announcement/view/^.Notification.AnnouncementRef'>" +
                                                                            "^.Other.AnnouncementTitle ^.Other.AnnouncementTypeName</a> ^.Other.PersonQuestion"; 

        public const string ANNOUNCEMENT_NEW_ATTACHMENT_NOTIFICATION_TO_PERSON = "ANNOUNCEMENT_NEW_ATTACHMENT_NOTIFICATION_TO_PERSON";
        private const string ANNOUNCEMENT_NEW_ATTACHMENT_NOTIFICATION_TO_PERSON_TEMPLATE = "Student <a href='@0#students/details/^.Notification.PersonRef'>^.Notification.Person.ShortSalutationName</a> added  new attachment";

        public const string ANNOUNCEMENT_SET_GRADE_NOTIFICATION_TO_PERSON = "ANNOUNCEMENT_SET_GRADE_NOTIFICATION_TO_PERSON";
        private const string ANNOUNCEMENT_SET_GRADE_NOTIFICATION_TO_PERSON_TEMPLATE = "<a> ^.Other.AnnouncementOwner.ShortSalutationName </a> graded or commented your <a href='@0#announcement/view/^.Notification.AnnouncementRef'> ^.Other.AnnouncementTitle</a> ^.Other.AnnouncementTypeName";

        public const string APPLICATION_NOTIFICATION = "APPLICATION_NOTIFICATION";
        private const string APPLICATION_NOTIFICATION_TEMPLATE = "Application <a href='@0#applications/install/^.Notification.ApplicationRef'>^.Other.ApplicationName</a> was added to My Apps for you by " +
                                                                 "<a ^[^.Other.NeedsUrlLink=true]{href='@0#@1/details/^.Notification.QuestionSchoolPersonRef'}>^.Other.FromPersonName</a>";

        public const string PRIVATE_MESSAGE_NOTIFICATION = "PRIVATE_MESSAGE_NOTIFICATION";
        private const string PRIVATE_MESSAGE_NOTIFICATION_TEMPLATE = " <a ^[^.Other.NeedsUrlLink=true]{href='@0#@1/details/^.Notification.PersonRef'}>^.Other.MessageSender.ShortSalutationName</a>" +
                                                                     " wrote : ^.Other.MessageSubject - ^.Other.ShortedMessage";

        public const string END_MARKINGPERIOD_NOTIFICATION_TO_ADMIN = "END_MARKINGPERIOD_NOTIFICATION_TO_ADMIN";
        private const string END_MARKINGPERIOD_NOTIFICATION_TO_ADMIN_TEMPALTE = "The end of marking period will come in ^.Other.EndDays . ^.Other[^.NextMarkingPeriodNotExist=true]{ Next Marking period does not exist.}" +
                                                                                "^.Other[^.NotAssignedToClass=true]{ Next Marking not assigned to any class.}";

        public const string ATTENDANCE_NOTIFICATION_TO_ADMIN = "ATTENDANCE_NOTIFICATION_TO_ADMIN";
        private const string ATTENDANCE_NOTIFICATION_TO_ADMIN_TEMPLATE = "^.Other.PersonsCount students have been  absent more than 2 days:\n " +
                                                                         "^.Other.Persons[]{<a href='@0#students/details/^.Id'>^.FullName</a> \n}";

        public const string ATTENDANCE_NOTIFICATION_TO_STUDENT = "ATTENDANCE_NOTIFICATION_TO_STUDENT";
        private const string ATTENDANCE_NOTIFICATION_TO_STUDENT_TEMPLATE = "You were marked ^.Other.AttendanceType for ^.Other.ClassName on ^.Other.Date period ^.Other.Period ";

        public const string ATTENDANCE_NOTIFICATION_TO_TEACHER = "ATTENDANCE_NOTIFICATION_TO_TEACHER";
        private const string ATTENDANCE_NOTIFICATION_TO_TEACHER_TEMPLATE = "There was no attendance taken for ^.Other.ClassName ^.Other.Date";

        public const string APP_BUDGET_BALANCE_NOTIFICATION = "APP_BUDGET_BALANCE_NOTIFICATION";
        private const string APP_BUDGET_BALANCE_NOTIFICATION_TEMPLATE = "Your App budget is now $^.Other.BudgetBalance";

        static NotificationTemplateProvider()
        {
            templates.Add(ANNOUNCEMENT_NEW_ATTACHMENT_NOTIFICATION, ANNOUNCEMENT_ATTACHMENT_NOTIFICATION_TEMPLATE);
            templates.Add(ANNOUNCEMENT_REMINDER_NOTIFICATION, ANNOUNCEMENT_REMINDER_NOTIFICATION_TEMPLATE);
            templates.Add(ANNOUNCEMENT_QUESTION_NOTIFICATION_TOAUTHOR, ANNOUNCEMENT_QUESTION_NOTIFICATION_TOAUTHOR_TEMPLATE);
            templates.Add(ANNOUNCEMENT_NOTIFICATION_ANSWER_TO_PERSON, ANNOUNCEMENT_NOTIFICATION_ASWER_TO_PERSON_TEMPLATE);
            templates.Add(ANNOUNCEMENT_NEW_ATTACHMENT_NOTIFICATION_TO_PERSON, ANNOUNCEMENT_NEW_ATTACHMENT_NOTIFICATION_TO_PERSON_TEMPLATE);
            templates.Add(ANNOUNCEMENT_SET_GRADE_NOTIFICATION_TO_PERSON, ANNOUNCEMENT_SET_GRADE_NOTIFICATION_TO_PERSON_TEMPLATE);
            templates.Add(APPLICATION_NOTIFICATION, APPLICATION_NOTIFICATION_TEMPLATE);
            templates.Add(PRIVATE_MESSAGE_NOTIFICATION, PRIVATE_MESSAGE_NOTIFICATION_TEMPLATE);
            templates.Add(ATTENDANCE_NOTIFICATION_TO_ADMIN, ATTENDANCE_NOTIFICATION_TO_ADMIN_TEMPLATE);
            templates.Add(END_MARKINGPERIOD_NOTIFICATION_TO_ADMIN, END_MARKINGPERIOD_NOTIFICATION_TO_ADMIN_TEMPALTE);
            templates.Add(APP_BUDGET_BALANCE_NOTIFICATION, APP_BUDGET_BALANCE_NOTIFICATION_TEMPLATE);
            templates.Add(ATTENDANCE_NOTIFICATION_TO_STUDENT, ATTENDANCE_NOTIFICATION_TO_STUDENT_TEMPLATE);
            templates.Add(ATTENDANCE_NOTIFICATION_TO_TEACHER, ATTENDANCE_NOTIFICATION_TO_TEACHER_TEMPLATE);
        }

        public static string GetTemplate(string templateName)
        {
            return templates[templateName];
        }

        public static void SetTemplate(string templateName, string template)
        {
            if (templates.ContainsKey(templateName))
                templates[templateName] = template;
            else
                templates.Add(templateName, template);
        }
    }
}
