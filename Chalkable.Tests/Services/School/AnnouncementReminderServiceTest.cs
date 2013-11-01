//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Chalkable.BusinessLogic.Model;
//using Chalkable.Tests.Services.TestContext;
//using NUnit.Framework;

//namespace Chalkable.Tests.Services.School
//{
//    public class AnnouncementReminderServiceTest : BaseSchoolServiceTest
//    {
//        [Test]
//        public void AddReminderTest()
//        {
//            var c = ClassServiceTest.CreateClass(FirstSchoolContext, FirstSchoolContext.FirstTeacher, FirstSchoolContext.FirstStudent, null);
//            var mpId = c.MarkingPeriodClasses[0].MarkingPeriodRef;
//            var ann = AnnouncementServiceTest.CreateDefaulTeacherAnnouncement(FirstSchoolContext.FirstTeacherSl, c.Id, mpId);
//            var reminderService = FirstSchoolContext.FirstTeacherSl.AnnouncementReminderService;
//            AssertForDeny(sl=>sl.AnnouncementReminderService.AddReminder(ann.Id, 2), FirstSchoolContext
//                , SchoolContextRoles.FirstParent | SchoolContextRoles.SecondTeacher | SchoolContextRoles.SecondStudent
//                | SchoolContextRoles.Checkin);

//            var reminderBefor = 2;
//            var reminder = reminderService.AddReminder(ann.Id, reminderBefor);
//            Assert.AreEqual(reminder.Before, reminderBefor);
//            Assert.AreEqual(reminder.AnnouncementRef, ann.Id);
//            Assert.AreEqual(reminder.PersonRef, null);
//            Assert.AreEqual(reminder.RemindDate, ann.Expires.AddDays(-reminderBefor));
//            var reminders = reminderService.GetReminders(ann.Id);
//            ann = FirstSchoolContext.FirstTeacherSl.AnnouncementService.GetAnnouncementDetails(ann.Id);
//            Assert.AreEqual(ann.AnnouncementReminders.Count, 1);
//            AssertAreEqual(ann.AnnouncementReminders, reminders);
//            AssertAreEqual(reminders.First(), reminder);
//            Assert.AreEqual(reminders.Count, 1);
//            Assert.AreEqual(FirstSchoolContext.FirstStudentSl.AnnouncementReminderService.GetReminders(ann.Id).Count, 0);
//            Assert.AreEqual(FirstSchoolContext.SecondStudentSl.AnnouncementReminderService.GetReminders(ann.Id).Count, 0);
//            reminder = FirstSchoolContext.FirstStudentSl.AnnouncementReminderService.AddReminder(ann.Id, null);
//            Assert.IsNull(reminder.Before);
//            Assert.AreEqual(reminder.RemindDate, FirstSchoolContext.NowDate);
//            Assert.AreEqual(reminder.PersonRef, FirstSchoolContext.FirstStudent.Id);
//            Assert.AreEqual(FirstSchoolContext.FirstStudentSl.AnnouncementReminderService.GetReminders(ann.Id).Count, 1);
//            Assert.AreEqual(FirstSchoolContext.FirstStudentSl.AnnouncementService.GetAnnouncementDetails(ann.Id).AnnouncementReminders.Count, 1);

//            ann.Expires = FirstSchoolContext.NowDate.AddDays(-1);
//            FirstSchoolContext.FirstTeacherSl.AnnouncementService.EditAnnouncement(AnnouncementInfo.Create(ann));
//            reminder = reminderService.AddReminder(ann.Id, 2);
//            Assert.AreEqual(reminder.RemindDate, FirstSchoolContext.NowDate);
//        }

//        [Test]
//        public void EditReminderTest()
//        {
//            var c = ClassServiceTest.CreateClass(FirstSchoolContext, FirstSchoolContext.FirstTeacher, FirstSchoolContext.FirstStudent, null);
//            var mpId = c.MarkingPeriodClasses[0].MarkingPeriodRef;
//            var ann = AnnouncementServiceTest.CreateDefaulTeacherAnnouncement(FirstSchoolContext.FirstTeacherSl, c.Id, mpId);
//            var reminderService = FirstSchoolContext.FirstTeacherSl.AnnouncementReminderService;
//            var reminderBefor = 2;
//            var reminder = reminderService.AddReminder(ann.Id, reminderBefor);
//            var reminder2 = FirstSchoolContext.FirstStudentSl.AnnouncementReminderService.AddReminder(ann.Id, reminderBefor);
//            //Security
//            AssertForDeny(sl => sl.AnnouncementReminderService.EditReminder(reminder.Id, 2), FirstSchoolContext
//                , SchoolContextRoles.FirstParent | SchoolContextRoles.FirstStudent | SchoolContextRoles.SecondTeacher
//                | SchoolContextRoles.SecondStudent | SchoolContextRoles.Checkin);

//            AssertForDeny(sl => sl.AnnouncementReminderService.EditReminder(reminder2.Id, 2), FirstSchoolContext
//                , SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher | SchoolContextRoles.SecondTeacher
//                | SchoolContextRoles.SecondStudent | SchoolContextRoles.Checkin);

//            reminder = reminderService.EditReminder(reminder.Id, 1);
//            Assert.AreEqual(reminder.Before, 1);
//            Assert.AreEqual(reminder.RemindDate, ann.Expires.AddDays(-1));
//            reminder = reminderService.EditReminder(reminder.Id, null);
//            Assert.AreEqual(reminder.Before, null);
//            Assert.AreEqual(reminder.RemindDate, FirstSchoolContext.NowDate);
//            var reminders = reminderService.GetReminders(ann.Id);
//            AssertAreEqual(reminder, reminders.First());

//        }

//        [Test]
//        public void DeleteReminderTest()
//        {
//            var c = ClassServiceTest.CreateClass(FirstSchoolContext, FirstSchoolContext.FirstTeacher, FirstSchoolContext.FirstStudent, null);
//            var mpId = c.MarkingPeriodClasses[0].MarkingPeriodRef;
//            var ann = AnnouncementServiceTest.CreateDefaulTeacherAnnouncement(FirstSchoolContext.FirstTeacherSl, c.Id, mpId);
//            var reminderService = FirstSchoolContext.FirstTeacherSl.AnnouncementReminderService;
//            var reminderBefor = 2;
//            var reminder = reminderService.AddReminder(ann.Id, reminderBefor);
//            var reminder2 = FirstSchoolContext.FirstStudentSl.AnnouncementReminderService.AddReminder(ann.Id, reminderBefor);
//            //Secuirty
//            AssertForDeny(sl => sl.AnnouncementReminderService.DeleteReminder(reminder.Id), FirstSchoolContext
//                , SchoolContextRoles.FirstParent | SchoolContextRoles.FirstStudent | SchoolContextRoles.SecondTeacher
//                | SchoolContextRoles.SecondStudent | SchoolContextRoles.Checkin);

//            AssertForDeny(sl => sl.AnnouncementReminderService.DeleteReminder(reminder2.Id), FirstSchoolContext
//                , SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher | SchoolContextRoles.SecondTeacher
//                | SchoolContextRoles.SecondStudent | SchoolContextRoles.Checkin);

//            reminderService.DeleteReminder(reminder.Id);
//            Assert.AreEqual(reminderService.GetReminders(ann.Id).Count, 0);
//            ann = FirstSchoolContext.FirstTeacherSl.AnnouncementService.GetAnnouncementDetails(ann.Id);
//            Assert.AreEqual(ann.AnnouncementReminders.Count, 0);
//        }

//        [Test]
//        public void ProcessRemindersTest()
//        {
//            //TODO: implement
//        }
//    }
//}
