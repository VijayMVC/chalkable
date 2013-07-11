using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Data.School.Model;
using NUnit.Framework;

namespace Chalkable.Tests.Services.School
{
    public class AnnouncementAttachmentServiceTest : BaseSchoolServiceTest
    {
        [Test]
        public void AddDeleteAttachmentTest()
        {
            var c = ClassServiceTest.CreateClass(SchoolTestContext, SchoolTestContext.FirstTeacher, SchoolTestContext.FirstStudent, null);
            var annType = SchoolTestContext.FirstTeacherSl.AnnouncementTypeService.GetAnnouncementTypeBySystemType(SystemAnnouncementType.HW);
            var ann = SchoolTestContext.FirstTeacherSl.AnnouncementService.CreateAnnouncement(annType.Id, c.Id);
            var mp1Id = c.MarkingPeriodClass[0].MarkingPeriodRef;
            SchoolTestContext.FirstTeacherSl.AnnouncementService.SubmitAnnouncement(ann.Id, c.Id, mp1Id);

            var attContent = new byte[] {1, 3, 5};
            var uuid1 = Guid.NewGuid().ToString();
            AssertForDeny(sl=>sl.AnnouncementAttachmentService.AddAttachment(ann.Id, attContent, "test1", uuid1), SchoolTestContext,
                SchoolContextRoles.FirstParent | SchoolContextRoles.SecondStudent | SchoolContextRoles.SecondTeacher
                | SchoolContextRoles.Checkin);

            c = SchoolTestContext.AdminGradeSl.ClassService.AddStudent(c.Id, SchoolTestContext.SecondStudent.Id);

            SchoolTestContext.FirstTeacherSl.AnnouncementAttachmentService.AddAttachment(ann.Id, attContent, "test1", uuid1);
            var attachments =  SchoolTestContext.FirstTeacherSl.AnnouncementAttachmentService.GetAttachments(ann.Id);
            Assert.AreEqual(attachments.Count, 1);
            Assert.AreEqual(attachments[0].AnnouncementRef, ann.Id);
            Assert.AreEqual(attachments[0].Order, 0);
            Assert.AreEqual(attachments[0].Name, "test1");
            Assert.AreEqual(attachments[0].Uuid, uuid1);
            Assert.AreEqual(attachments[0].AttachedDate.Date, SchoolTestContext.NowDate);
            Assert.AreEqual(attachments[0].PersonRef, SchoolTestContext.FirstTeacher.Id);
            var attachment1 = SchoolTestContext.FirstTeacherSl.AnnouncementAttachmentService.GetAttachmentById(attachments[0].Id);
            AssertAreEqual(attachment1, attachments[0]);
            
            var uuid2 = Guid.NewGuid().ToString();
            SchoolTestContext.FirstStudentSl.AnnouncementAttachmentService.AddAttachment(ann.Id, attContent, "test2", uuid2);
            var uuid3 = Guid.NewGuid().ToString();
            SchoolTestContext.SecondStudentSl.AnnouncementAttachmentService.AddAttachment(ann.Id, attContent, "test3", uuid3);
            Assert.AreEqual(SchoolTestContext.FirstStudentSl.AnnouncementAttachmentService.GetAttachments(ann.Id).Count, 2);
            Assert.AreEqual(SchoolTestContext.FirstTeacherSl.AnnouncementAttachmentService.GetAttachments(ann.Id).Count, 3);
           
 
            attachments = SchoolTestContext.FirstStudentSl.AnnouncementAttachmentService.GetAttachments(ann.Id, 0, int.MaxValue, false);
            Assert.AreEqual(attachments.Count, 1);
            Assert.AreEqual(SchoolTestContext.FirstTeacherSl.AnnouncementAttachmentService.GetAttachments(ann.Id, 0, int.MaxValue, false).Count, 1);
            var attachment2 = attachments[0];

            var adminAnnType = SchoolTestContext.AdminGradeSl.AnnouncementTypeService.GetAnnouncementTypeBySystemType(SystemAnnouncementType.Admin);
            var ann2 = SchoolTestContext.AdminGradeSl.AnnouncementService.CreateAnnouncement(adminAnnType.Id);
            ann2.Expires = SchoolTestContext.NowDate.AddDays(3);

            var adminAnnRecipients = new List<RecipientInfo>
                {
                    RecipientInfo.Create(false, CoreRoles.TEACHER_ROLE.Id, null, null),
                    RecipientInfo.Create(false, CoreRoles.STUDENT_ROLE.Id, c.GradeLevelRef, null),
                };
            SchoolTestContext.AdminGradeSl.AnnouncementService.EditAnnouncement(ann2, null, null, adminAnnRecipients);
            SchoolTestContext.AdminGradeSl.AnnouncementService.SubmitForAdmin(ann2.Id);
            var uuid4 = Guid.NewGuid().ToString();
            AssertForDeny(sl => sl.AnnouncementAttachmentService.AddAttachment(ann2.Id, attContent, "adminAtt", uuid4), SchoolTestContext,
                SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstTeacher
                | SchoolContextRoles.FirstParent | SchoolContextRoles.SecondStudent
                | SchoolContextRoles.SecondTeacher | SchoolContextRoles.Checkin);
            SchoolTestContext.AdminGradeSl.AnnouncementAttachmentService.AddAttachment(ann2.Id, attContent, "adminAtt", uuid4);



            Assert.AreEqual(SchoolTestContext.AdminGradeSl.AnnouncementAttachmentService.GetAttachments(ann2.Id).Count, 1);
            Assert.AreEqual(SchoolTestContext.FirstTeacherSl.AnnouncementAttachmentService.GetAttachments(ann2.Id).Count, 1);
            Assert.AreEqual(SchoolTestContext.FirstTeacherSl.AnnouncementAttachmentService.GetAttachments(ann2.Id, 0, int.MaxValue, false).Count, 0);
            Assert.AreEqual(SchoolTestContext.FirstStudentSl.AnnouncementAttachmentService.GetAttachments(ann2.Id, 0, int.MaxValue, false).Count, 0);
            
            AssertForDeny(sl => sl.AnnouncementAttachmentService.DeleteAttachment(attachment2.Id), SchoolTestContext,
                SchoolContextRoles.AdminGrade | SchoolContextRoles.AdminEditor | SchoolContextRoles.AdminViewer |
                SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstParent | SchoolContextRoles.SecondStudent
                | SchoolContextRoles.SecondTeacher | SchoolContextRoles.Checkin);
            AssertForDeny(sl => sl.AnnouncementAttachmentService.DeleteAttachment(attachment1.Id), SchoolTestContext,
                SchoolContextRoles.AdminGrade | SchoolContextRoles.AdminEditor | SchoolContextRoles.AdminViewer |
                SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent | SchoolContextRoles.SecondStudent
                | SchoolContextRoles.SecondTeacher | SchoolContextRoles.Checkin);

            SchoolTestContext.FirstTeacherSl.AnnouncementAttachmentService.DeleteAttachment(attachment1.Id);
            SchoolTestContext.FirstStudentSl.AnnouncementAttachmentService.DeleteAttachment(attachment2.Id);
            Assert.AreEqual(SchoolTestContext.FirstTeacherSl.AnnouncementAttachmentService.GetAttachments(ann.Id).Count, 1);
            Assert.AreEqual(SchoolTestContext.FirstStudentSl.AnnouncementAttachmentService.GetAttachments(ann.Id).Count, 0);

        }
    }
}
