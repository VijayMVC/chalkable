using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using GCObjectRenderer;

namespace Chalkable.BusinessLogic.Services.School.Notifications
{
    public class NotificationBuilder
    {
        static NotificationBuilder() 
        {
            var announcementAttachmentTemplate = NotificationTemplateProvider.GetTemplate(NotificationTemplateProvider.ANNOUNCEMENT_NEW_ATTACHMENT_NOTIFICATION);
            RenderService.RegisterMainRenderer(NotificationTemplateProvider.ANNOUNCEMENT_NEW_ATTACHMENT_NOTIFICATION, new TemplateRenderer(announcementAttachmentTemplate), false);

            var announcementReminderTemplate = NotificationTemplateProvider.GetTemplate(NotificationTemplateProvider.ANNOUNCEMENT_REMINDER_NOTIFICATION);
            RenderService.RegisterMainRenderer(NotificationTemplateProvider.ANNOUNCEMENT_REMINDER_NOTIFICATION, new TemplateRenderer(announcementReminderTemplate), false);

            var announcementQnToAouthorTemplate = NotificationTemplateProvider.GetTemplate(NotificationTemplateProvider.ANNOUNCEMENT_QUESTION_NOTIFICATION_TOAUTHOR);
            RenderService.RegisterMainRenderer(NotificationTemplateProvider.ANNOUNCEMENT_QUESTION_NOTIFICATION_TOAUTHOR, new TemplateRenderer(announcementQnToAouthorTemplate), false);

            var announcementQnToPersonTemplate = NotificationTemplateProvider.GetTemplate(NotificationTemplateProvider.ANNOUNCEMENT_NOTIFICATION_ANSWER_TO_PERSON);
            RenderService.RegisterMainRenderer(NotificationTemplateProvider.ANNOUNCEMENT_NOTIFICATION_ANSWER_TO_PERSON, new TemplateRenderer(announcementQnToPersonTemplate), false);

            var announcementAttachmentToPersonTemplate = NotificationTemplateProvider.GetTemplate(NotificationTemplateProvider.ANNOUNCEMENT_NEW_ATTACHMENT_NOTIFICATION_TO_PERSON);
            RenderService.RegisterMainRenderer(NotificationTemplateProvider.ANNOUNCEMENT_NEW_ATTACHMENT_NOTIFICATION_TO_PERSON, new TemplateRenderer(announcementAttachmentToPersonTemplate), false);

            var announcementGradeToStudentTemplate = NotificationTemplateProvider.GetTemplate(NotificationTemplateProvider.ANNOUNCEMENT_SET_GRADE_NOTIFICATION_TO_PERSON);
            RenderService.RegisterMainRenderer(NotificationTemplateProvider.ANNOUNCEMENT_SET_GRADE_NOTIFICATION_TO_PERSON, new TemplateRenderer(announcementGradeToStudentTemplate), false);

            var applicationTemplate = NotificationTemplateProvider.GetTemplate(NotificationTemplateProvider.APPLICATION_NOTIFICATION);
            RenderService.RegisterMainRenderer(NotificationTemplateProvider.APPLICATION_NOTIFICATION, new TemplateRenderer(applicationTemplate), false);

            var privateMassageTemplate = NotificationTemplateProvider.GetTemplate(NotificationTemplateProvider.PRIVATE_MESSAGE_NOTIFICATION);
            RenderService.RegisterMainRenderer(NotificationTemplateProvider.PRIVATE_MESSAGE_NOTIFICATION, new TemplateRenderer(privateMassageTemplate), false);

            var endMarkingPeriodTemplate = NotificationTemplateProvider.GetTemplate(NotificationTemplateProvider.END_MARKINGPERIOD_NOTIFICATION_TO_ADMIN);
            RenderService.RegisterMainRenderer(NotificationTemplateProvider.END_MARKINGPERIOD_NOTIFICATION_TO_ADMIN, new TemplateRenderer(endMarkingPeriodTemplate), false);

            var attendanceToAdminTemplate = NotificationTemplateProvider.GetTemplate(NotificationTemplateProvider.ATTENDANCE_NOTIFICATION_TO_ADMIN);
            RenderService.RegisterMainRenderer(NotificationTemplateProvider.ATTENDANCE_NOTIFICATION_TO_ADMIN, new TemplateRenderer(attendanceToAdminTemplate), false);

            var attendanceToStudentTemplate = NotificationTemplateProvider.GetTemplate(NotificationTemplateProvider.ATTENDANCE_NOTIFICATION_TO_STUDENT);
            RenderService.RegisterMainRenderer(NotificationTemplateProvider.ATTENDANCE_NOTIFICATION_TO_STUDENT, new TemplateRenderer(attendanceToStudentTemplate), false);

            var attendanceToTeacherTemplate = NotificationTemplateProvider.GetTemplate(NotificationTemplateProvider.ATTENDANCE_NOTIFICATION_TO_TEACHER);
            RenderService.RegisterMainRenderer(NotificationTemplateProvider.ATTENDANCE_NOTIFICATION_TO_TEACHER, new TemplateRenderer(attendanceToTeacherTemplate), false);

            var appBudgetBalance = NotificationTemplateProvider.GetTemplate(NotificationTemplateProvider.APP_BUDGET_BALANCE_NOTIFICATION);
            RenderService.RegisterMainRenderer(NotificationTemplateProvider.APP_BUDGET_BALANCE_NOTIFICATION, new TemplateRenderer(appBudgetBalance), false);
        }

        private IServiceLocatorSchool serviceLocator;

        private const string DATA_FORMAT = "MMM dd";
        private const string TEACHERS = "teachers";
        private const string STUDENTS = "students";


        private const string ROLE_SYSADMIN = "SysAdmin";
        private const string ROLE_TEACHER = "Teacher";
        private const string ROLE_STUDENT = "Student";
        private const string ROLE_PARENT = "Parent";
        private const string ROLE_CHECKIN = "Checkin";
        private const string ROLE_ADMIN = "Admin";

        public NotificationBuilder(IServiceLocatorSchool serviceLocator)
        {
            this.serviceLocator = serviceLocator;
        }

        private Notification BuildNotificationFromTemplate(string templateName, NotificationType type, Person recipient, Announcement announcement = null
            , Guid? applicationId = null, MarkingPeriod markingPeriod = null, PrivateMessageDetails privateMessage = null, Person asker = null, object other = null, string baseUrl = null)
        {
            var parameters = new List<string> {GetBaseUrlByRole(recipient, baseUrl)};
            var notification = new NotificationDetails
            {
                PersonRef = recipient.Id,
                Person = recipient,
                Shown = false,
                Type = type,
                Created = serviceLocator.Context.NowSchoolTime
            };
            if (announcement != null)
            {
                notification.AnnouncementRef = announcement.Id;
                notification.Announcement = announcement;
            }
            if (applicationId.HasValue)
            {
                //notification.Application = application;
                notification.ApplicationRef = applicationId;
            }
            if (markingPeriod != null)
            {
                notification.MarkingPeriod = markingPeriod;
                notification.MarkingPeriodRef = markingPeriod.Id;
            }
            if (privateMessage != null)
            {
                notification.PrivateMessage = privateMessage;
                notification.PrivateMessageRef = privateMessage.Id;
            }
            if (asker != null)
            {
                notification.QuestionPerson = asker;
                notification.QuestionPersonRef = asker.Id;
                parameters.Add(GetRelativeUrlByRole(asker));
            }
            
            var model = new {Notification = notification, Other = other};
            string message = RenderService.Render(templateName,  model, parameters);
            notification.Message = message;
            return notification;
        }

        private string GetBaseUrlByRole(Person person, string baseUrl)
        {
            if (string.IsNullOrEmpty(baseUrl))
                baseUrl = PreferenceService.Get(Preference.APPLICATION_URL).Value;
            var url = UrlTools.UrlCombine(baseUrl, "/Home/") + "{0}";
            if (person.RoleRef == CoreRoles.SUPER_ADMIN_ROLE.Id)
                return string.Format(url, ROLE_SYSADMIN);
            if (person.RoleRef == CoreRoles.ADMIN_EDIT_ROLE.Id || person.RoleRef == CoreRoles.ADMIN_VIEW_ROLE.Id
               || person.RoleRef == CoreRoles.ADMIN_GRADE_ROLE.Id)
            {
                return string.Format(url, ROLE_ADMIN);
            }
            if (person.RoleRef == CoreRoles.TEACHER_ROLE.Id)
                return string.Format(url, ROLE_TEACHER);
            if (person.RoleRef == CoreRoles.STUDENT_ROLE.Id)
                return string.Format(url, ROLE_STUDENT);
            if (person.RoleRef == CoreRoles.PARENT_ROLE.Id)
                return string.Format(url, ROLE_PARENT);
            if (person.RoleRef == CoreRoles.CHECKIN_ROLE.Id)
                return string.Format(url, ROLE_CHECKIN);

            throw new UnknownRoleException();
        }

        private string GetRelativeUrlByRole(Person person)
        {
            if (person.RoleRef == CoreRoles.TEACHER_ROLE.Id)
                return TEACHERS;
            if (person.RoleRef == CoreRoles.STUDENT_ROLE.Id)
                return STUDENTS;
            return null;
        }

        //TODO: implement builders 

        public Notification BuildAnnouncementNewAttachmentNotification(AnnouncementComplex announcement, Person recipient)
        {
            return BuildNotificationFromTemplate(NotificationTemplateProvider.ANNOUNCEMENT_NEW_ATTACHMENT_NOTIFICATION,
                                                 NotificationType.Announcement, recipient, announcement, null, null,
                                                 null, null, new { AnnouncementTitle = announcement.Title});
        }
        public Notification BuildAnnouncementNewAttachmentNotificationToPerson(AnnouncementDetails announcement, Person fromschoolPerson)
        {
            return BuildNotificationFromTemplate(NotificationTemplateProvider.ANNOUNCEMENT_NEW_ATTACHMENT_NOTIFICATION_TO_PERSON,
                                                 NotificationType.Announcement, announcement.Owner, announcement, null, null,  
                                                 null, fromschoolPerson, new { AnnouncementTitle = announcement.Title});
        }

        public Notification BuildAnnouncementReminderNotification(AnnouncementComplex announcement, Person recipient)
        {
            return BuildNotificationFromTemplate(NotificationTemplateProvider.ANNOUNCEMENT_REMINDER_NOTIFICATION,
                                                 NotificationType.Announcement, recipient, announcement, null, null, 
                                                 null, null, new { AnnouncementTitle = announcement.Title });
        }

        public Notification BuildAnnouncementQnToAuthorNotifiaction(AnnouncementQnAComplex announcementQnA, AnnouncementComplex announcement)
        {
            return BuildNotificationFromTemplate(NotificationTemplateProvider.ANNOUNCEMENT_QUESTION_NOTIFICATION_TOAUTHOR,
                                                 NotificationType.Question, announcementQnA.Answerer, announcement, null, null, null, 
                                                 announcementQnA.Asker, new
                                                     {
                                                         AnnouncementTypeName = announcement.ClassAnnouncementTypeName,
                                                         PersonQuestion = StringTools.BuildShortText(announcementQnA.Question, 35),
                                                         AnnouncementTitle = announcement.Title
                                                     });
        }

        public Notification BuildAnnouncementAnswerToPersonNotifiaction(AnnouncementQnAComplex announcementQnA, AnnouncementComplex announcement)
        {

            return BuildNotificationFromTemplate(NotificationTemplateProvider.ANNOUNCEMENT_NOTIFICATION_ANSWER_TO_PERSON,
                                                 NotificationType.Announcement, announcementQnA.Asker, announcement, null, null, 
                                                 null, announcementQnA.Answerer, new
                                                     {
                                                         AnnouncementTypeName = announcement.ClassAnnouncementTypeName,
                                                         AnnouncementTitle = announcement.Title,
                                                         PersonQuestion = StringTools.BuildShortText(announcementQnA.Question, 40)
                                                     });
        }

        public Notification BuildAnnouncementSetGradeToPersonNotifiaction(AnnouncementDetails announcement, Person recipient)
        {
            return BuildNotificationFromTemplate(NotificationTemplateProvider.ANNOUNCEMENT_SET_GRADE_NOTIFICATION_TO_PERSON,
                                                 NotificationType.Announcement, recipient, announcement, null, null, null, null, 
                                                 new {
                                                         AnnouncementOwner = announcement.Owner,
                                                         AnnouncementTypeName = announcement.ClassAnnouncementTypeName,
                                                         AnnouncementTitle = announcement.Title
                                                     });
        }

        //public Notification BuildApplicationNotification(Application application, SchoolPerson recipient, SchoolPerson fromSchoolPerson, string fromPersonName, string baseUrl = null)
        //{
        //    var otherModel = new {
        //                             NeedsUrlLink = fromSchoolPerson.HasRole(CoreRoles.TEACHER_ROLE.Name)
        //                                            || fromSchoolPerson.HasRole(CoreRoles.STUDENT_ROLE.Name),
        //                             FromPersonName = fromPersonName,
        //                             ApplicationName = application.Name
        //                         };
        //    return BuildNotificationFromTemplate(NotificationTemplateProvider.APPLICATION_NOTIFICATION,
        //                                         NotificationType.Application, recipient, null, null, application, null, null,
        //                                         fromSchoolPerson, otherModel, baseUrl);
        //}

        public Notification BuildPrivateMessageNotification(PrivateMessageDetails privateMessage, Person fromPerson, Person toPerson)
        {
            var fromPersonRole = CoreRoles.GetById(fromPerson.RoleRef);
            var otherModel = new
                    {
                        NeedsUrlLink = fromPersonRole == CoreRoles.TEACHER_ROLE
                                        || fromPersonRole == CoreRoles.STUDENT_ROLE,
                        ShortedMessage = StringTools.BuildShortText(privateMessage.Body, 30),
                        MessageSubject = privateMessage.Subject,
                        MessageSender = privateMessage.Sender
                    };
            return BuildNotificationFromTemplate(NotificationTemplateProvider.PRIVATE_MESSAGE_NOTIFICATION,
                                                    NotificationType.Message, toPerson, null, null, null,
                                                    privateMessage, fromPerson, otherModel);
        }

        public Notification BuildEndMarkingPeriodNotification(MarkingPeriod markingPeriod, Person recipient, int endDays, 
            bool nextMarkingPeriodExist, bool nextMpNotAssignedToClass)
        {
            var otherModel = new
                    {
                        EndDays = endDays,
                        NextMarkingPeriodNotExist = nextMarkingPeriodExist,
                        NotAssignedToClass = nextMpNotAssignedToClass
                    };
            return BuildNotificationFromTemplate(NotificationTemplateProvider.END_MARKINGPERIOD_NOTIFICATION_TO_ADMIN,
                       NotificationType.MarkingPeriodEnding, recipient, null, null, markingPeriod, null, null, otherModel);
        }

        public Notification BuildAttendanceNotificationToAdmin(Person recipient, IList<Person> persons)
        {
            var otherModel = new { PersonsCount = persons.Count, Persons = persons };
            return BuildNotificationFromTemplate(NotificationTemplateProvider.ATTENDANCE_NOTIFICATION_TO_ADMIN,
                                                 NotificationType.Attendance, recipient, null, null, null, null, null, otherModel);
        }

        public Notification BuildAttendanceNotificationToStudent(Person recipient, ClassAttendanceDetails classAttendance)
        {
            var otherModel = new
                    {
                        AttendanceType = classAttendance.Level,
                        ClassName = classAttendance.Class.Name,
                        Date = classAttendance.Date.ToString(DATA_FORMAT),
                        Period = 0//TODO: no data in INOW ?
                    };
            return BuildNotificationFromTemplate(NotificationTemplateProvider.ATTENDANCE_NOTIFICATION_TO_STUDENT,
                                                 NotificationType.Attendance, recipient, null, null, null, null, null, otherModel);
        }
        
        public Notification BuildAppBudgetBalanceNotification(Person recipient, double budgetBalance)
        {
            var otherModel = new { BudgetBalance = budgetBalance };
            return BuildNotificationFromTemplate(NotificationTemplateProvider.APP_BUDGET_BALANCE_NOTIFICATION,
                                                 NotificationType.AppBudgetBallance, recipient, null, null, null, null, null, otherModel);
        }
    }
}
