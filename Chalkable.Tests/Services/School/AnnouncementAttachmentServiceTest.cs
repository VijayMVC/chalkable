//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Chalkable.BusinessLogic.Model;
//using Chalkable.Common;
//using Chalkable.Data.School.Model;
//using Chalkable.Tests.Services.TestContext;
//using NUnit.Framework;

//namespace Chalkable.Tests.Services.School
//{
//    public class AnnouncementAttachmentServiceTest : BaseSchoolServiceTest
//    {
//        [Test]
//        public void AddDeleteAttachmentTest()
//        {
//            var c = ClassServiceTest.CreateClass(FirstSchoolContext, FirstSchoolContext.FirstTeacher, FirstSchoolContext.FirstStudent, null);
//            var annType = FirstSchoolContext.FirstTeacherSl.AnnouncementTypeService.GetAnnouncementTypeBySystemType(SystemAnnouncementType.HW);
//            var ann = FirstSchoolContext.FirstTeacherSl.AnnouncementService.CreateAnnouncement(annType.Id, c.Id);
//            var mp1Id = c.MarkingPeriodClasses[0].MarkingPeriodRef;
//            FirstSchoolContext.FirstTeacherSl.AnnouncementService.SubmitAnnouncement(ann.Id, c.Id, mp1Id);

//            var attContent = new byte[] {1, 3, 5};
//            var uuid1 = Guid.NewGuid().ToString();
//            AssertForDeny(sl=>sl.AnnouncementAttachmentService.AddAttachment(ann.Id, attContent, "test1", uuid1), FirstSchoolContext,
//                SchoolContextRoles.FirstParent | SchoolContextRoles.SecondStudent | SchoolContextRoles.SecondTeacher
//                | SchoolContextRoles.Checkin);

//            c = FirstSchoolContext.AdminGradeSl.ClassService.AddStudent(c.Id, FirstSchoolContext.SecondStudent.Id);

//            FirstSchoolContext.FirstTeacherSl.AnnouncementAttachmentService.AddAttachment(ann.Id, attContent, "test1", uuid1);
//            //check notification
//            Assert.AreEqual(1, FirstSchoolContext.FirstStudentSl.NotificationService.GetUnshownNotifications().Count);
//            Assert.AreEqual(ann.Id, FirstSchoolContext.FirstStudentSl.NotificationService.GetUnshownNotifications().First().AnnouncementRef);
//            Assert.AreEqual(1, FirstSchoolContext.SecondStudentSl.NotificationService.GetUnshownNotifications().Count);
//            Assert.AreEqual(ann.Id, FirstSchoolContext.SecondStudentSl.NotificationService.GetUnshownNotifications().First().AnnouncementRef);
            
            
//            var attachments =  FirstSchoolContext.FirstTeacherSl.AnnouncementAttachmentService.GetAttachments(ann.Id);
//            Assert.AreEqual(attachments.Count, 1);
//            Assert.AreEqual(attachments[0].AnnouncementRef, ann.Id);
//            Assert.AreEqual(attachments[0].Order, 0);
//            Assert.AreEqual(attachments[0].Name, "test1");
//            Assert.AreEqual(attachments[0].Uuid, uuid1);
//            Assert.AreEqual(attachments[0].AttachedDate.Date, FirstSchoolContext.NowDate);
//            Assert.AreEqual(attachments[0].PersonRef, FirstSchoolContext.FirstTeacher.Id);
//            var attachment1 = FirstSchoolContext.FirstTeacherSl.AnnouncementAttachmentService.GetAttachmentById(attachments[0].Id);
//            AssertAreEqual(attachment1, attachments[0]);
            
//            var uuid2 = Guid.NewGuid().ToString();
//            FirstSchoolContext.FirstStudentSl.AnnouncementAttachmentService.AddAttachment(ann.Id, attContent, "test2", uuid2);
//            //check notification
//            Assert.AreEqual(1, FirstSchoolContext.FirstTeacherSl.NotificationService.GetUnshownNotifications().Count);
//            Assert.AreEqual(ann.Id, FirstSchoolContext.FirstTeacherSl.NotificationService.GetUnshownNotifications().First().AnnouncementRef);
            

//            var uuid3 = Guid.NewGuid().ToString();
//            FirstSchoolContext.SecondStudentSl.AnnouncementAttachmentService.AddAttachment(ann.Id, attContent, "test3", uuid3);
//            Assert.AreEqual(FirstSchoolContext.FirstStudentSl.AnnouncementAttachmentService.GetAttachments(ann.Id).Count, 2);
//            Assert.AreEqual(FirstSchoolContext.FirstTeacherSl.AnnouncementAttachmentService.GetAttachments(ann.Id).Count, 3);
//            //check notification
//            Assert.AreEqual(2, FirstSchoolContext.FirstTeacherSl.NotificationService.GetUnshownNotifications().Count);
            
//            attachments = FirstSchoolContext.FirstStudentSl.AnnouncementAttachmentService.GetAttachments(ann.Id, 0, int.MaxValue, false);
//            Assert.AreEqual(attachments.Count, 1);
//            Assert.AreEqual(FirstSchoolContext.FirstTeacherSl.AnnouncementAttachmentService.GetAttachments(ann.Id, 0, int.MaxValue, false).Count, 1);
//            var attachment2 = attachments[0];

//            var adminAnnType = FirstSchoolContext.AdminGradeSl.AnnouncementTypeService.GetAnnouncementTypeBySystemType(SystemAnnouncementType.Admin);
//            var ann2 = FirstSchoolContext.AdminGradeSl.AnnouncementService.CreateAnnouncement(adminAnnType.Id);
//            ann2.Expires = FirstSchoolContext.NowDate.AddDays(3);

//            var adminAnnRecipients = new List<RecipientInfo>
//                {
//                    RecipientInfo.Create(false, CoreRoles.TEACHER_ROLE.Id, null, null),
//                    RecipientInfo.Create(false, CoreRoles.STUDENT_ROLE.Id, c.GradeLevelRef, null),
//                };
//            ann2.Subject = "test";
//            FirstSchoolContext.AdminGradeSl.AnnouncementService.EditAnnouncement(AnnouncementInfo.Create(ann2), null, null, adminAnnRecipients);
//            FirstSchoolContext.AdminGradeSl.AnnouncementService.SubmitForAdmin(ann2.Id);
//            var uuid4 = Guid.NewGuid().ToString();
//            AssertForDeny(sl => sl.AnnouncementAttachmentService.AddAttachment(ann2.Id, attContent, "adminAtt", uuid4), FirstSchoolContext,
//                SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstTeacher
//                | SchoolContextRoles.FirstParent | SchoolContextRoles.SecondStudent
//                | SchoolContextRoles.SecondTeacher | SchoolContextRoles.Checkin);
//            FirstSchoolContext.AdminGradeSl.AnnouncementAttachmentService.AddAttachment(ann2.Id, attContent, "adminAtt", uuid4);
//            //check notification
//            Assert.AreEqual(3, FirstSchoolContext.FirstTeacherSl.NotificationService.GetUnshownNotifications().Count);
//            Assert.AreEqual(2, FirstSchoolContext.FirstStudentSl.NotificationService.GetUnshownNotifications().Count);
//            Assert.AreEqual(2, FirstSchoolContext.SecondStudentSl.NotificationService.GetUnshownNotifications().Count);
            
            
//            Assert.AreEqual(FirstSchoolContext.AdminGradeSl.AnnouncementAttachmentService.GetAttachments(ann2.Id).Count, 1);
//            Assert.AreEqual(FirstSchoolContext.FirstTeacherSl.AnnouncementAttachmentService.GetAttachments(ann2.Id).Count, 1);
//            Assert.AreEqual(FirstSchoolContext.FirstTeacherSl.AnnouncementAttachmentService.GetAttachments(ann2.Id, 0, int.MaxValue, false).Count, 0);
//            Assert.AreEqual(FirstSchoolContext.FirstStudentSl.AnnouncementAttachmentService.GetAttachments(ann2.Id, 0, int.MaxValue, false).Count, 0);
            
//            AssertForDeny(sl => sl.AnnouncementAttachmentService.DeleteAttachment(attachment2.Id), FirstSchoolContext,
//                SchoolContextRoles.AdminGrade | SchoolContextRoles.AdminEditor | SchoolContextRoles.AdminViewer |
//                SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstParent | SchoolContextRoles.SecondStudent
//                | SchoolContextRoles.SecondTeacher | SchoolContextRoles.Checkin);
//            AssertForDeny(sl => sl.AnnouncementAttachmentService.DeleteAttachment(attachment1.Id), FirstSchoolContext,
//                SchoolContextRoles.AdminGrade | SchoolContextRoles.AdminEditor | SchoolContextRoles.AdminViewer |
//                SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent | SchoolContextRoles.SecondStudent
//                | SchoolContextRoles.SecondTeacher | SchoolContextRoles.Checkin);

//            FirstSchoolContext.FirstTeacherSl.AnnouncementAttachmentService.DeleteAttachment(attachment1.Id);
//            FirstSchoolContext.FirstStudentSl.AnnouncementAttachmentService.DeleteAttachment(attachment2.Id);
//            Assert.AreEqual(FirstSchoolContext.FirstTeacherSl.AnnouncementAttachmentService.GetAttachments(ann.Id).Count, 1);
//            Assert.AreEqual(FirstSchoolContext.FirstStudentSl.AnnouncementAttachmentService.GetAttachments(ann.Id).Count, 0);

//        }
//    }
//}
