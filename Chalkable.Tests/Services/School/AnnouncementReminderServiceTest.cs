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
//            var c = ClassServiceTest.CreateClass(SchoolTestContext, SchoolTestContext.FirstTeacher, SchoolTestContext.FirstStudent, null);
//            var mpId = c.MarkingPeriodClasses[0].MarkingPeriodRef;
//            var ann = AnnouncementServiceTest.CreateDefaulTeacherAnnouncement(SchoolTestContext.FirstTeacherSl, c.Id, mpId);
//            var reminderService = SchoolTestContext.FirstTeacherSl.AnnouncementReminderService;
//            AssertForDeny(sl=>sl.AnnouncementReminderService.AddReminder(ann.Id, 2), SchoolTestContext
//                , SchoolContextRoles.FirstParent | SchoolContextRoles.SecondTeacher | SchoolContextRoles.SecondStudent
//                | SchoolContextRoles.Checkin);

//            var reminderBefor = 2;
//            var reminder = reminderService.AddReminder(ann.Id, reminderBefor);
//            Assert.AreEqual(reminder.Before, reminderBefor);
//            Assert.AreEqual(reminder.AnnouncementRef, ann.Id);
//            Assert.AreEqual(reminder.PersonRef, null);
//            Assert.AreEqual(reminder.RemindDate, ann.Expires.AddDays(-reminderBefor));
//            var reminders = reminderService.GetReminders(ann.Id);
//            ann = SchoolTestContext.FirstTeacherSl.AnnouncementService.GetAnnouncementDetails(ann.Id);
//            Assert.AreEqual(ann.AnnouncementReminders.Count, 1);
//            AssertAreEqual(ann.AnnouncementReminders, reminders);
//            AssertAreEqual(reminders.First(), reminder);
//            Assert.AreEqual(reminders.Count, 1);
//            Assert.AreEqual(SchoolTestContext.FirstStudentSl.AnnouncementReminderService.GetReminders(ann.Id).Count, 0);
//            Assert.AreEqual(SchoolTestContext.SecondStudentSl.AnnouncementReminderService.GetReminders(ann.Id).Count, 0);
//            reminder = SchoolTestContext.FirstStudentSl.AnnouncementReminderService.AddReminder(ann.Id, null);
//            Assert.IsNull(reminder.Before);
//            Assert.AreEqual(reminder.RemindDate, SchoolTestContext.NowDate);
//            Assert.AreEqual(reminder.PersonRef, SchoolTestContext.FirstStudent.Id);
//            Assert.AreEqual(SchoolTestContext.FirstStudentSl.AnnouncementReminderService.GetReminders(ann.Id).Count, 1);
//            Assert.AreEqual(SchoolTestContext.FirstStudentSl.AnnouncementService.GetAnnouncementDetails(ann.Id).AnnouncementReminders.Count, 1);

//            ann.Expires = SchoolTestContext.NowDate.AddDays(-1);
//            SchoolTestContext.FirstTeacherSl.AnnouncementService.EditAnnouncement(AnnouncementInfo.Create(ann));
//            reminder = reminderService.AddReminder(ann.Id, 2);
//            Assert.AreEqual(reminder.RemindDate, SchoolTestContext.NowDate);
//        }

//        [Test]
//        public void EditReminderTest()
//        {
//            var c = ClassServiceTest.CreateClass(SchoolTestContext, SchoolTestContext.FirstTeacher, SchoolTestContext.FirstStudent, null);
//            var mpId = c.MarkingPeriodClasses[0].MarkingPeriodRef;
//            var ann = AnnouncementServiceTest.CreateDefaulTeacherAnnouncement(SchoolTestContext.FirstTeacherSl, c.Id, mpId);
//            var reminderService = SchoolTestContext.FirstTeacherSl.AnnouncementReminderService;
//            var reminderBefor = 2;
//            var reminder = reminderService.AddReminder(ann.Id, reminderBefor);
//            var reminder2 = SchoolTestContext.FirstStudentSl.AnnouncementReminderService.AddReminder(ann.Id, reminderBefor);
//            //Security
//            AssertForDeny(sl => sl.AnnouncementReminderService.EditReminder(reminder.Id, 2), SchoolTestContext
//                , SchoolContextRoles.FirstParent | SchoolContextRoles.FirstStudent | SchoolContextRoles.SecondTeacher
//                | SchoolContextRoles.SecondStudent | SchoolContextRoles.Checkin);

//            AssertForDeny(sl => sl.AnnouncementReminderService.EditReminder(reminder2.Id, 2), SchoolTestContext
//                , SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher | SchoolContextRoles.SecondTeacher
//                | SchoolContextRoles.SecondStudent | SchoolContextRoles.Checkin);

//            reminder = reminderService.EditReminder(reminder.Id, 1);
//            Assert.AreEqual(reminder.Before, 1);
//            Assert.AreEqual(reminder.RemindDate, ann.Expires.AddDays(-1));
//            reminder = reminderService.EditReminder(reminder.Id, null);
//            Assert.AreEqual(reminder.Before, null);
//            Assert.AreEqual(reminder.RemindDate, SchoolTestContext.NowDate);
//            var reminders = reminderService.GetReminders(ann.Id);
//            AssertAreEqual(reminder, reminders.First());

//        }

//        [Test]
//        public void DeleteReminderTest()
//        {
//            var c = ClassServiceTest.CreateClass(SchoolTestContext, SchoolTestContext.FirstTeacher, SchoolTestContext.FirstStudent, null);
//            var mpId = c.MarkingPeriodClasses[0].MarkingPeriodRef;
//            var ann = AnnouncementServiceTest.CreateDefaulTeacherAnnouncement(SchoolTestContext.FirstTeacherSl, c.Id, mpId);
//            var reminderService = SchoolTestContext.FirstTeacherSl.AnnouncementReminderService;
//            var reminderBefor = 2;
//            var reminder = reminderService.AddReminder(ann.Id, reminderBefor);
//            var reminder2 = SchoolTestContext.FirstStudentSl.AnnouncementReminderService.AddReminder(ann.Id, reminderBefor);
//            //Secuirty
//            AssertForDeny(sl => sl.AnnouncementReminderService.DeleteReminder(reminder.Id), SchoolTestContext
//                , SchoolContextRoles.FirstParent | SchoolContextRoles.FirstStudent | SchoolContextRoles.SecondTeacher
//                | SchoolContextRoles.SecondStudent | SchoolContextRoles.Checkin);

//            AssertForDeny(sl => sl.AnnouncementReminderService.DeleteReminder(reminder2.Id), SchoolTestContext
//                , SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher | SchoolContextRoles.SecondTeacher
//                | SchoolContextRoles.SecondStudent | SchoolContextRoles.Checkin);

//            reminderService.DeleteReminder(reminder.Id);
//            Assert.AreEqual(reminderService.GetReminders(ann.Id).Count, 0);
//            ann = SchoolTestContext.FirstTeacherSl.AnnouncementService.GetAnnouncementDetails(ann.Id);
//            Assert.AreEqual(ann.AnnouncementReminders.Count, 0);
//        }

//        [Test]
//        public void ProcessRemindersTest()
//        {
//            //TODO: implement
//        }
//    }
//}
