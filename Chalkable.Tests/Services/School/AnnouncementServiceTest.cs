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
    public class AnnouncementServiceTest : BaseSchoolServiceTest
    {

        //TODO: testing with applicatoins
        //TODO: gradingStyle testing

        [Test]
        public void CreateDeleteDraftAnnouncementTest()
        {
            var c = ClassServiceTest.CreateClass(SchoolTestContext, SchoolTestContext.FirstTeacher,
                                                 SchoolTestContext.FirstStudent, null);

            var annTypeHw = SchoolTestContext.AdminGradeSl.AnnouncementService.GetAnnouncementTypeBySystemType(
                    SystemAnnouncementType.HW);
            
            AssertForDeny(sl=>sl.AnnouncementService.CreateAnnouncement(annTypeHw.Id), 
                SchoolTestContext, SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent 
                | SchoolContextRoles.SecondStudent | SchoolContextRoles.Checkin); 

            var ann = SchoolTestContext.FirstTeacherSl.AnnouncementService.CreateAnnouncement(annTypeHw.Id);
            Assert.AreEqual(ann.Created.Date, SchoolTestContext.NowDate);
            Assert.IsTrue(ann.IsDraft);
            Assert.AreEqual(ann.AnnouncementTypeRef, annTypeHw.Id);
            Assert.AreEqual(ann.AnnouncementTypeName, annTypeHw.Name);
            Assert.IsTrue(ann.IsGradableType);
            Assert.AreEqual(ann.MarkingPeriodClassRef, null);
            Assert.AreEqual(ann.IsOwner, true);
            Assert.AreEqual(ann.Dropped, false);
            AssertAreEqual(ann, SchoolTestContext.FirstTeacherSl.AnnouncementService.GetAnnouncementDetails(ann.Id));
            Assert.AreEqual(SchoolTestContext.FirstTeacherSl.AnnouncementService.GetAnnouncements(false, 0 , int.MaxValue, null).Count, 0);

            AssertForDeny(sl => sl.AnnouncementService.DeleteAnnouncements(SchoolTestContext.FirstTeacher.Id), SchoolTestContext,
                SchoolContextRoles.AdminEditor | SchoolContextRoles.AdminGrade | SchoolContextRoles.AdminViewer
                | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent | SchoolContextRoles.SecondStudent
                | SchoolContextRoles.Checkin);

            SchoolTestContext.FirstTeacherSl.AnnouncementService.DeleteAnnouncements(SchoolTestContext.FirstTeacher.Id);
            Assert.IsNull(SchoolTestContext.FirstTeacherSl.AnnouncementService.GetAnnouncementDetails(ann.Id));
            
        }

        [Test]
        public void EditAnnouncementTest()
        {
            var c = ClassServiceTest.CreateClass(SchoolTestContext, SchoolTestContext.FirstTeacher, SchoolTestContext.FirstStudent, null);
            var annService = SchoolTestContext.FirstTeacherSl.AnnouncementService;
            var annTypeHW = annService.GetAnnouncementTypeBySystemType(SystemAnnouncementType.HW);
            var annDetails = annService.CreateAnnouncement(annTypeHW.Id);

            var oldCreatedDate = annDetails.Created;
            var oldGradingStyle = annDetails.GradingStyle;
            var expiresDate = SchoolTestContext.NowDate.AddDays(3);
            annDetails.Created = oldCreatedDate.AddDays(4);
            annDetails.GradingStyle = Enum.GetValues(typeof (GradingStyleEnum)).Cast<GradingStyleEnum>().First(x => x != oldGradingStyle);
            annDetails.Expires = expiresDate;
            annDetails.Subject = "subject";
            annDetails.Content = "content";
            
            var mp1Id = c.MarkingPeriodClass[0].MarkingPeriodRef;
            AssertForDeny(sl=>sl.AnnouncementService.EditAnnouncement(annDetails, mp1Id, c.Id), SchoolTestContext, 
                SchoolContextRoles.SecondTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent | SchoolContextRoles.Checkin);

            SchoolTestContext.FirstTeacherSl.AnnouncementReminderService.AddReminder(annDetails.Id, 1);
            var ann = annService.EditAnnouncement(annDetails, mp1Id, c.Id);
            
            Assert.AreEqual(ann.Created, oldCreatedDate);
            Assert.AreEqual(ann.GradingStyle, oldGradingStyle);
            Assert.AreEqual(ann.Expires, expiresDate);
            Assert.AreEqual(ann.MarkingPeriodClassRef, c.MarkingPeriodClass[0].Id);
            Assert.AreEqual(annDetails.Content, "content");
            Assert.AreEqual(annDetails.Subject, "subject");

            annDetails = annService.GetAnnouncementDetails(ann.Id);
            AssertAreEqual(ann, annDetails);
            var reminder = annDetails.AnnouncementReminders[0];
            Assert.AreEqual(reminder.RemindDate, annDetails.Expires.AddDays(-reminder.Before.Value));
            SchoolTestContext.FirstTeacherSl.AnnouncementReminderService.EditReminder(reminder.Id, null);
            ann = annService.EditAnnouncement(annDetails, mp1Id, c.Id);
            annDetails = annService.GetAnnouncementDetails(ann.Id);
            Assert.AreEqual(annDetails.AnnouncementReminders[0].RemindDate, SchoolTestContext.NowDate);
            ann.Expires = SchoolTestContext.NowDate.AddDays(-1);
            ann = annService.EditAnnouncement(ann, mp1Id, c.Id);
            Assert.AreEqual(annService.GetAnnouncementDetails(ann.Id).AnnouncementReminders.Count, 0);
            Assert.AreEqual(SchoolTestContext.FirstTeacherSl.AnnouncementReminderService.GetReminders(ann.Id).Count, 0);
            
        }

        [Test]
        public void SubmitAnnouncementTest()
        {
            var c = ClassServiceTest.CreateClass(SchoolTestContext, SchoolTestContext.FirstTeacher, SchoolTestContext.FirstStudent, null);
            var annService = SchoolTestContext.FirstTeacherSl.AnnouncementService;
            var annTypeHw = annService.GetAnnouncementTypeBySystemType(SystemAnnouncementType.HW);
            var annDetails = annService.CreateAnnouncement(annTypeHw.Id);
            annDetails.Expires = SchoolTestContext.NowDate.AddDays(7);
            annService.EditAnnouncement(annDetails);
            var mp1Id = c.MarkingPeriodClass[0].MarkingPeriodRef;
            AssertForDeny(sl => sl.AnnouncementService.SubmitAnnouncement(annDetails.Id, c.Id, mp1Id), SchoolTestContext,
                SchoolContextRoles.FirstParent | SchoolContextRoles.SecondTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.Checkin); 
            
            annService.SubmitAnnouncement(annDetails.Id, c.Id, c.MarkingPeriodClass[0].MarkingPeriodRef);
            annDetails = annService.GetAnnouncementDetails(annDetails.Id);
            Assert.IsFalse(annDetails.IsDraft);
            Assert.AreEqual(annDetails.State, AnnouncementState.Created);
            Assert.AreEqual(annDetails.IsOwner, true);
            Assert.AreEqual(annDetails.ClassId, c.Id);
            Assert.AreEqual(annDetails.ClassName, c.Name);
            Assert.AreEqual(annDetails.CourseId, c.CourseRef);

            AssertAreEqual(annService.GetAnnouncementById(annDetails.Id), annDetails);
            AssertException<Exception>(() => SchoolTestContext.SecondStudentSl.AnnouncementService.GetAnnouncementById(annDetails.Id));
            Assert.AreEqual(SchoolTestContext.FirstTeacherSl.AnnouncementService.GetAnnouncements(0, int.MaxValue).Count, 1);
            Assert.AreEqual(SchoolTestContext.FirstStudentSl.AnnouncementService.GetAnnouncements(0, int.MaxValue).Count, 1);
            Assert.AreEqual(SchoolTestContext.FirstStudentSl.AnnouncementService.GetAnnouncements(0, int.MaxValue, true).Count, 0);
            Assert.AreEqual(SchoolTestContext.FirstStudentSl.AnnouncementService.GetAnnouncements(false, 0, int.MaxValue, c.Id, mp1Id).Count, 1);
            Assert.AreEqual(SchoolTestContext.FirstStudentSl.AnnouncementService.GetAnnouncements(false, 0, int.MaxValue, c.Id, mp1Id, true).Count, 0);
            Assert.IsFalse(SchoolTestContext.FirstStudentSl.AnnouncementService.GetAnnouncementDetails(annDetails.Id).IsOwner);
        }

        [Test]
        public void SubmitForAdminTest()
        {
            var c = ClassServiceTest.CreateClass(SchoolTestContext, SchoolTestContext.FirstTeacher, SchoolTestContext.FirstStudent, null);
            var annService = SchoolTestContext.AdminGradeSl.AnnouncementService;
            var annTypeHw = annService.GetAnnouncementTypeBySystemType(SystemAnnouncementType.Admin);
            var annDetails = annService.CreateAnnouncement(annTypeHw.Id);
            annDetails.Expires = SchoolTestContext.NowDate.AddDays(7);

            var recipientInfo = new List<RecipientInfo>
                {
                    RecipientInfo.Create(false, null, null, SchoolTestContext.FirstStudent.Id),
                    RecipientInfo.Create(false, CoreRoles.TEACHER_ROLE.Id, null, null)
                };
            annService.EditAnnouncement(annDetails, null, null, recipientInfo);
            
            AssertForDeny(sl=>sl.AnnouncementService.SubmitForAdmin(annDetails.Id), SchoolTestContext
                , SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent
                | SchoolContextRoles.SecondTeacher | SchoolContextRoles.Checkin);

            annService.SubmitForAdmin(annDetails.Id);
            
            Assert.AreEqual(1, SchoolTestContext.AdminGradeSl.AnnouncementService.GetAnnouncements(false, 0, 10, null).Count);
            Assert.AreEqual(1, SchoolTestContext.FirstTeacherSl.AnnouncementService.GetAnnouncements(false, 0, 10, null).Count);
            Assert.AreEqual(1, SchoolTestContext.FirstStudentSl.AnnouncementService.GetAnnouncements(false, 0, 10, null).Count);
            Assert.AreEqual(1, SchoolTestContext.SecondTeacherSl.AnnouncementService.GetAnnouncements(false, 0, 10, null).Count);
            Assert.AreEqual(0, SchoolTestContext.SecondStudentSl.AnnouncementService.GetAnnouncements(false, 0, 10, null).Count);

            Assert.AreEqual(2, SchoolTestContext.AdminGradeSl.AnnouncementService.GetAnnouncementRecipients(annDetails.Id).Count);
            recipientInfo.Add(RecipientInfo.Create(false, CoreRoles.STUDENT_ROLE.Id, null, null));
            annService.EditAnnouncement(annDetails, null, null, recipientInfo);
            Assert.AreEqual(1, SchoolTestContext.SecondStudentSl.AnnouncementService.GetAnnouncements(false, 0, 10, null).Count);
            recipientInfo.Add(RecipientInfo.Create(true, null, null, null));
            annService.EditAnnouncement(annDetails, null, null, recipientInfo);
            Assert.AreEqual(1, SchoolTestContext.SecondStudentSl.AnnouncementService.GetAnnouncements(false, 0, 10, null).Count);           

        }

        [Test]
        public void DeleteAnnouncementTest()
        {
            
            var mathClass1 = ClassServiceTest.CreateClass(SchoolTestContext, SchoolTestContext.FirstTeacher, 
                SchoolTestContext.FirstStudent, SchoolTestContext.SecondStudent);
            var mathClass2 = ClassServiceTest.CreateClass(SchoolTestContext, SchoolTestContext.FirstTeacher,
                SchoolTestContext.FirstStudent, SchoolTestContext.SecondStudent, "math2");
            var teacherAnnService = SchoolTestContext.FirstTeacherSl.AnnouncementService;

            var type = teacherAnnService.GetAnnouncementTypeBySystemType(SystemAnnouncementType.HW);
            teacherAnnService.CreateAnnouncement(type.Id);
            AssertForDeny(sl => sl.AnnouncementService.DeleteAnnouncements(SchoolTestContext.FirstTeacher.Id), SchoolTestContext, 
                SchoolContextRoles.AdminGrade | SchoolContextRoles.FirstStudent | SchoolContextRoles.AdminEditor
                | SchoolContextRoles.AdminViewer | SchoolContextRoles.SecondTeacher);


            teacherAnnService.DeleteAnnouncements(SchoolTestContext.FirstTeacher.Id);
            Assert.IsNull(teacherAnnService.GetLastDraft());
            SchoolTestContext.FirstTeacherSl.AnnouncementService.CreateAnnouncement(type.Id, mathClass1.Id);
            SchoolTestContext.FirstTeacherSl.AnnouncementService.DeleteAnnouncements(mathClass1.Id, type.Id, AnnouncementState.Draft);
            Assert.IsNull(teacherAnnService.GetLastDraft());

            teacherAnnService.CreateAnnouncement(type.Id, mathClass1.Id);
            teacherAnnService.DeleteAnnouncements(mathClass2.Id, type.Id, AnnouncementState.Draft);
            Assert.IsNotNull(teacherAnnService.GetLastDraft());

            var announcement2 = teacherAnnService.CreateAnnouncement(type.Id);
            AssertForDeny(sl => sl.AnnouncementService.DeleteAnnouncement(announcement2.Id), SchoolTestContext,
                SchoolContextRoles.AdminGrade | SchoolContextRoles.FirstStudent | SchoolContextRoles.AdminEditor
                | SchoolContextRoles.AdminViewer | SchoolContextRoles.SecondTeacher);

            teacherAnnService.DeleteAnnouncement(announcement2.Id);
            Assert.IsNull(teacherAnnService.GetLastDraft());

            var announcement3 = teacherAnnService.CreateAnnouncement(type.Id);
            var mp = SchoolTestContext.FirstTeacherSl.MarkingPeriodService.GetLastMarkingPeriod(SchoolTestContext.NowDate);
            teacherAnnService.SubmitAnnouncement(announcement3.Id, mathClass1.Id, mp.Id);
            teacherAnnService.DeleteAnnouncements(SchoolTestContext.FirstTeacher.Id);
            Assert.IsNotNull(teacherAnnService.GetAnnouncementDetails(announcement3.Id));
            teacherAnnService.DeleteAnnouncements(mathClass1.Id, type.Id, AnnouncementState.Draft);
            Assert.IsNotNull(teacherAnnService.GetAnnouncementById(announcement3.Id));
            teacherAnnService.DeleteAnnouncements(SchoolTestContext.FirstTeacher.Id, AnnouncementState.Created);
            AssertException<Exception>(() => teacherAnnService.GetAnnouncementById(announcement3.Id));

            var announcement4 = teacherAnnService.CreateAnnouncement(type.Id);


            teacherAnnService.SubmitAnnouncement(announcement4.Id, mathClass1.Id, mp.Id);
            
            var items = SchoolTestContext.FirstTeacherSl.StudentAnnouncementService.GetStudentAnnouncements(announcement4.Id);
            SchoolTestContext.FirstTeacherSl.StudentAnnouncementService.SetGrade(items[0].Id, 70, "0", null, false);

            teacherAnnService.DeleteAnnouncements(mathClass1.Id, type.Id, AnnouncementState.Created);
            AssertException<Exception>(() => teacherAnnService.GetAnnouncementById(announcement4.Id));
        }

        [Test]
        public void StarringAnnouncementTest()
        {
            //TODO: implement
        }
    }
}
