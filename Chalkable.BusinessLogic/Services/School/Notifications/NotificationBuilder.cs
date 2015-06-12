﻿using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Common;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Model.Attendances;
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
        private IServiceLocatorSchool serviceLocator;

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

            var announcementAttachmentToPersonTemplate = NotificationTemplateProvider.GetTemplate(NotificationTemplateProvider.ANNOUNCEMENT_NOT_OWNER_ATTACHMENT_NOTIFICATION_TO_PERSON);
            RenderService.RegisterMainRenderer(NotificationTemplateProvider.ANNOUNCEMENT_NOT_OWNER_ATTACHMENT_NOTIFICATION_TO_PERSON, new TemplateRenderer(announcementAttachmentToPersonTemplate), false);

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

        public NotificationBuilder(IServiceLocatorSchool serviceLocator)
        {
            this.serviceLocator = serviceLocator;
        }

        private const string DATA_FORMAT = "MMM dd";
        private const string TEACHERS = "teachers";
        private const string STUDENTS = "students";


        private const string ROLE_SYSADMIN = "SysAdmin";
        private const string ROLE_TEACHER = "Teacher";
        private const string ROLE_STUDENT = "Student";
        private const string ROLE_PARENT = "Parent";
        private const string ROLE_CHECKIN = "Checkin";
        private const string ROLE_ADMIN = "Admin";

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
            if (person.RoleRef == CoreRoles.DISTRICT_ADMIN_ROLE.Id)
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

        public Notification BuildAnnouncementNewAttachmentNotification(DateTime created, AnnouncementComplex announcement, Person recipient)
        {
            return BuildNotificationFromTemplate(NotificationTemplateProvider.ANNOUNCEMENT_NEW_ATTACHMENT_NOTIFICATION,
                                                 NotificationType.Announcement, recipient, announcement, null, null,
                                                 null, null, new { AnnouncementTitle = announcement.Title});
        }
        public Notification BuildAnnouncementNewAttachmentNotificationToPerson(DateTime created, AnnouncementDetails announcement, Person toPerson, Person fromschoolPerson)
        {
            return BuildNotificationFromTemplate(NotificationTemplateProvider.ANNOUNCEMENT_NOT_OWNER_ATTACHMENT_NOTIFICATION_TO_PERSON,
                                                 NotificationType.Announcement, toPerson, announcement, null, null,  
                                                 null, fromschoolPerson, new
                                                     {
                                                         AnnouncementTitle = announcement.Title,
                                                         AttachmentOwnerFullName = fromschoolPerson.FullName()
                                                     });
        }

        public Notification BuildAnnouncementReminderNotification(DateTime created, AnnouncementComplex announcement, Person recipient)
        {
            return BuildNotificationFromTemplate(NotificationTemplateProvider.ANNOUNCEMENT_REMINDER_NOTIFICATION,
                                                 NotificationType.Announcement, recipient, announcement, null, null, 
                                                 null, null, new { AnnouncementTitle = announcement.Title });
        }

        public Notification BuildAnnouncementQnToAuthorNotifiaction(DateTime created, AnnouncementQnAComplex announcementQnA, AnnouncementComplex announcement, Person answerer)
        {
            return BuildNotificationFromTemplate(NotificationTemplateProvider.ANNOUNCEMENT_QUESTION_NOTIFICATION_TOAUTHOR,
                                                 NotificationType.Question, answerer, announcement, null, null, null, 
                                                 announcementQnA.Asker, new
                                                     {
                                                         announcement.AnnouncementTypeName,
                                                         PersonQuestion = StringTools.BuildShortText(announcementQnA.Question, 35),
                                                         AnnouncementTitle = announcement.Title,
                                                         AskerName = announcementQnA.Asker.DisplayName()
                                                     });
        }

        public Notification BuildAnnouncementAnswerToPersonNotifiaction(DateTime created, AnnouncementQnAComplex announcementQnA, AnnouncementComplex announcement)
        {

            return BuildNotificationFromTemplate(NotificationTemplateProvider.ANNOUNCEMENT_NOTIFICATION_ANSWER_TO_PERSON,
                                                 NotificationType.Announcement, announcementQnA.Asker, announcement, null, null, 
                                                 null, announcementQnA.Answerer, new
                                                     {
                                                         announcement.AnnouncementTypeName,
                                                         AnnouncementTitle = announcement.Title,
                                                         PersonQuestion = StringTools.BuildShortText(announcementQnA.Question, 40),
                                                         AnswererName = announcementQnA.Answerer.DisplayName()
                                                     });
        }

        public Notification BuildAnnouncementSetGradeToStudentNotification(DateTime created, AnnouncementDetails announcement, Person recipient)
        {
            return BuildNotificationFromTemplate(NotificationTemplateProvider.ANNOUNCEMENT_SET_GRADE_NOTIFICATION_TO_PERSON,
                                                 NotificationType.Announcement, recipient, announcement, null, null, null, null, 
                                                 new {
                                                         AnnouncementOwner = announcement.Owner,
                                                         announcement.AnnouncementTypeName,
                                                         AnnouncementTitle = announcement.Title,
                                                         AnnouncementOwnerName = announcement.Owner.DisplayName()
                                                     });
        }


        public Notification BuildPrivateMessageNotification(DateTime created, PrivateMessageDetails privateMessage, Person fromPerson, Person toPerson)
        {
            var fromPersonRole = CoreRoles.GetById(fromPerson.RoleRef);
            var otherModel = new
                    {
                        NeedsUrlLink = fromPersonRole == CoreRoles.TEACHER_ROLE
                                        || fromPersonRole == CoreRoles.STUDENT_ROLE,
                        ShortedMessage = StringTools.BuildShortText(privateMessage.Body, 30),
                        MessageSubject = privateMessage.Subject,
                        SenderId = privateMessage.Sender.Id,
                        SenderName = privateMessage.Sender.DisplayName()
                    };
            return BuildNotificationFromTemplate(NotificationTemplateProvider.PRIVATE_MESSAGE_NOTIFICATION,
                                                    NotificationType.Message, toPerson, null, null, null,
                                                    privateMessage, fromPerson, otherModel);
        }

        public Notification BuildEndMarkingPeriodNotification(DateTime created, MarkingPeriod markingPeriod, Person recipient, int endDays, 
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

        public Notification BuildAttendanceNotificationToAdmin(DateTime created, Person recipient, IList<Person> persons)
        {
            var otherModel = new { PersonsCount = persons.Count, Persons = persons };
            return BuildNotificationFromTemplate(NotificationTemplateProvider.ATTENDANCE_NOTIFICATION_TO_ADMIN,
                                                 NotificationType.Attendance, recipient, null, null, null, null, null, otherModel);
        }

        public Notification BuildAttendanceNotificationToStudent(Person recipient, StudentClassAttendance studentAttendance, Class cClass)
        {
            var otherModel = new
                    {
                        AttendanceType = studentAttendance.Level,
                        ClassName = cClass.Name,
                        Date = studentAttendance.Date.ToString(DATA_FORMAT),
                        Period = 0//TODO: no data in INOW ?
                    };
            return BuildNotificationFromTemplate(NotificationTemplateProvider.ATTENDANCE_NOTIFICATION_TO_STUDENT,
                                                 NotificationType.Attendance, recipient, null, null, null, null, null, otherModel);
        }

        public Notification BuildAppBudgetBalanceNotification(DateTime created, Person recipient, double budgetBalance)
        {
            var otherModel = new { BudgetBalance = budgetBalance };
            return BuildNotificationFromTemplate(NotificationTemplateProvider.APP_BUDGET_BALANCE_NOTIFICATION,
                                                 NotificationType.AppBudgetBallance, recipient, null, null, null, null, null, otherModel);
        }
    }
}
