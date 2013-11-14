//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Chalkable.BusinessLogic.Model;
//using Chalkable.BusinessLogic.Services;
//using Chalkable.BusinessLogic.Services.School;
//using Chalkable.Common;
//using Chalkable.Data.School.Model;
//using Chalkable.Tests.Services.TestContext;
//using NUnit.Framework;

//namespace Chalkable.Tests.Services.School
//{
//    public class AnnouncementServiceTest : BaseSchoolServiceTest
//    {

//        //TODO: testing with applicatoins
//        //TODO: gradingStyle testing

//        [Test]
//        public void CreateDeleteDraftAnnouncementTest()
//        {
//            var c = ClassServiceTest.CreateClass(FirstSchoolContext, FirstSchoolContext.FirstTeacher,
//                                                 FirstSchoolContext.FirstStudent, null);

//            var annTypeHw = FirstSchoolContext.AdminGradeSl.AnnouncementTypeService.GetAnnouncementTypeBySystemType(
//                    SystemAnnouncementType.HW);
            
//            AssertForDeny(sl=>sl.AnnouncementService.CreateAnnouncement(annTypeHw.Id), 
//                FirstSchoolContext, SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent 
//                | SchoolContextRoles.SecondStudent | SchoolContextRoles.Checkin); 

//            var ann = FirstSchoolContext.FirstTeacherSl.AnnouncementService.CreateAnnouncement(annTypeHw.Id);
//            Assert.AreEqual(ann.Created.Date, FirstSchoolContext.NowDate);
//            Assert.IsTrue(ann.IsDraft);
//            Assert.AreEqual(ann.AnnouncementTypeRef, annTypeHw.Id);
//            Assert.AreEqual(ann.AnnouncementTypeName, annTypeHw.Name);
//            Assert.IsTrue(ann.GradableType);
//            Assert.AreEqual(ann.MarkingPeriodClassRef, null);
//            Assert.AreEqual(ann.IsOwner, true);
//            Assert.AreEqual(ann.Dropped, false);
//            AssertAreEqual(ann, FirstSchoolContext.FirstTeacherSl.AnnouncementService.GetAnnouncementDetails(ann.Id));
//            Assert.AreEqual(FirstSchoolContext.FirstTeacherSl.AnnouncementService.GetAnnouncements(false, 0 , int.MaxValue, null).Count, 0);

//            AssertForDeny(sl => sl.AnnouncementService.DeleteAnnouncements(FirstSchoolContext.FirstTeacher.Id), FirstSchoolContext,
//                SchoolContextRoles.AdminEditor | SchoolContextRoles.AdminGrade | SchoolContextRoles.AdminViewer
//                | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent | SchoolContextRoles.SecondStudent
//                | SchoolContextRoles.Checkin);

//            FirstSchoolContext.FirstTeacherSl.AnnouncementService.DeleteAnnouncements(FirstSchoolContext.FirstTeacher.Id);
//            Assert.IsNull(FirstSchoolContext.FirstTeacherSl.AnnouncementService.GetAnnouncementDetails(ann.Id));
            
//        }

//        [Test]
//        public void EditAnnouncementTest()
//        {
//            var c = ClassServiceTest.CreateClass(FirstSchoolContext, FirstSchoolContext.FirstTeacher, FirstSchoolContext.FirstStudent, null);
//            var annService = FirstSchoolContext.FirstTeacherSl.AnnouncementService;
//            var annTypeHW =  FirstSchoolContext.FirstTeacherSl.AnnouncementTypeService.GetAnnouncementTypeBySystemType(SystemAnnouncementType.HW);
//            var annDetails = annService.CreateAnnouncement(annTypeHW.Id);

//            var oldCreatedDate = annDetails.Created;
//            var oldGradingStyle = annDetails.GradingStyle;
//            var expiresDate = FirstSchoolContext.NowDate.AddDays(3);
//            annDetails.Created = oldCreatedDate.AddDays(4);
//            annDetails.GradingStyle = Enum.GetValues(typeof (GradingStyleEnum)).Cast<GradingStyleEnum>().First(x => x != oldGradingStyle);
//            annDetails.Expires = expiresDate;
//            annDetails.Subject = "subject";
//            annDetails.Content = "content";
            
//            var mp1Id = c.MarkingPeriodClasses[0].MarkingPeriodRef;
//            AssertForDeny(sl=>sl.AnnouncementService.EditAnnouncement(AnnouncementInfo.Create(annDetails), mp1Id, c.Id), FirstSchoolContext, 
//                SchoolContextRoles.SecondTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent | SchoolContextRoles.Checkin);

//            FirstSchoolContext.FirstTeacherSl.AnnouncementReminderService.AddReminder(annDetails.Id, 1);
//            var ann = annService.EditAnnouncement(AnnouncementInfo.Create(annDetails), mp1Id, c.Id);
            
//            Assert.AreEqual(ann.Created, oldCreatedDate);
//            Assert.AreEqual(ann.GradingStyle, oldGradingStyle);
//            Assert.AreEqual(ann.Expires, expiresDate);
//            Assert.AreEqual(ann.MarkingPeriodClassRef, c.MarkingPeriodClasses[0].Id);
//            Assert.AreEqual(annDetails.Content, "content");
//            Assert.AreEqual(annDetails.Subject, "subject");

//            annDetails = annService.GetAnnouncementDetails(ann.Id);
//            AssertAreEqual(ann, annDetails);
//            var reminder = annDetails.AnnouncementReminders[0];
//            Assert.AreEqual(reminder.RemindDate, annDetails.Expires.AddDays(-reminder.Before.Value));
//            FirstSchoolContext.FirstTeacherSl.AnnouncementReminderService.EditReminder(reminder.Id, null);
//            ann = annService.EditAnnouncement(AnnouncementInfo.Create(annDetails), mp1Id, c.Id);
//            annDetails = annService.GetAnnouncementDetails(ann.Id);
//            Assert.AreEqual(annDetails.AnnouncementReminders[0].RemindDate, FirstSchoolContext.NowDate);
//            ann.Expires = FirstSchoolContext.NowDate.AddDays(-1);
//            ann = annService.EditAnnouncement(AnnouncementInfo.Create(ann), mp1Id, c.Id);
//            Assert.AreEqual(annService.GetAnnouncementDetails(ann.Id).AnnouncementReminders.Count, 0);
//            Assert.AreEqual(FirstSchoolContext.FirstTeacherSl.AnnouncementReminderService.GetReminders(ann.Id).Count, 0);
            
//        }

//        [Test]
//        public void SubmitAnnouncementTest()
//        {
//            var c = ClassServiceTest.CreateClass(FirstSchoolContext, FirstSchoolContext.FirstTeacher, FirstSchoolContext.FirstStudent, null);
//            var annService = FirstSchoolContext.FirstTeacherSl.AnnouncementService;
//            var annTypeHw = FirstSchoolContext.FirstTeacherSl.AnnouncementTypeService.GetAnnouncementTypeBySystemType(SystemAnnouncementType.HW);
//            var annDetails = annService.CreateAnnouncement(annTypeHw.Id);
//            annDetails.Expires = FirstSchoolContext.NowDate.AddDays(7);
//            annService.EditAnnouncement(AnnouncementInfo.Create(annDetails));
//            var mp1Id = c.MarkingPeriodClasses[0].MarkingPeriodRef;
//            AssertForDeny(sl => sl.AnnouncementService.SubmitAnnouncement(annDetails.Id, c.Id, mp1Id), FirstSchoolContext,
//                SchoolContextRoles.FirstParent | SchoolContextRoles.SecondTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.Checkin); 
            
//            annService.SubmitAnnouncement(annDetails.Id, c.Id, c.MarkingPeriodClasses[0].MarkingPeriodRef);
//            annDetails = annService.GetAnnouncementDetails(annDetails.Id);
//            Assert.IsFalse(annDetails.IsDraft);
//            Assert.AreEqual(annDetails.State, AnnouncementState.Created);
//            Assert.AreEqual(annDetails.IsOwner, true);
//            Assert.AreEqual(annDetails.ClassId, c.Id);
//            Assert.AreEqual(annDetails.ClassName, c.Name);
//            Assert.AreEqual(annDetails.CourseId, c.CourseRef);

//            AssertAreEqual(annService.GetAnnouncementById(annDetails.Id), annDetails);
//            AssertException<Exception>(() => FirstSchoolContext.SecondStudentSl.AnnouncementService.GetAnnouncementById(annDetails.Id));
//            Assert.AreEqual(FirstSchoolContext.FirstTeacherSl.AnnouncementService.GetAnnouncements(0, int.MaxValue).Count, 1);
//            Assert.AreEqual(FirstSchoolContext.FirstStudentSl.AnnouncementService.GetAnnouncements(0, int.MaxValue).Count, 1);
//            Assert.AreEqual(FirstSchoolContext.FirstStudentSl.AnnouncementService.GetAnnouncements(0, int.MaxValue, true).Count, 0);
//            Assert.AreEqual(FirstSchoolContext.FirstStudentSl.AnnouncementService.GetAnnouncements(false, 0, int.MaxValue, c.Id, mp1Id).Count, 1);
//            Assert.AreEqual(FirstSchoolContext.FirstStudentSl.AnnouncementService.GetAnnouncements(false, 0, int.MaxValue, c.Id, mp1Id, true).Count, 0);
//            Assert.IsFalse(FirstSchoolContext.FirstStudentSl.AnnouncementService.GetAnnouncementDetails(annDetails.Id).IsOwner);
//        }

//        [Test]
//        public void SubmitForAdminTest()
//        {
//            var c = ClassServiceTest.CreateClass(FirstSchoolContext, FirstSchoolContext.FirstTeacher, FirstSchoolContext.FirstStudent, null);
//            var annService = FirstSchoolContext.AdminGradeSl.AnnouncementService;
//            var annTypeHw = FirstSchoolContext.FirstTeacherSl.AnnouncementTypeService.GetAnnouncementTypeBySystemType(SystemAnnouncementType.Admin);
//            var annDetails = annService.CreateAnnouncement(annTypeHw.Id);
//            annDetails.Expires = FirstSchoolContext.NowDate.AddDays(7);

//            var recipientInfo = new List<RecipientInfo>
//                {
//                    RecipientInfo.Create(false, null, null, FirstSchoolContext.FirstStudent.Id),
//                    RecipientInfo.Create(false, CoreRoles.TEACHER_ROLE.Id, null, null)
//                };
//            annService.EditAnnouncement(AnnouncementInfo.Create(annDetails), null, null, recipientInfo);
            
//            AssertForDeny(sl=>sl.AnnouncementService.SubmitForAdmin(annDetails.Id), FirstSchoolContext
//                , SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent
//                | SchoolContextRoles.SecondTeacher | SchoolContextRoles.Checkin);

//            annService.SubmitForAdmin(annDetails.Id);
            
//            Assert.AreEqual(1, FirstSchoolContext.AdminGradeSl.AnnouncementService.GetAnnouncements(false, 0, 10, null).Count);
//            Assert.AreEqual(1, FirstSchoolContext.FirstTeacherSl.AnnouncementService.GetAnnouncements(false, 0, 10, null).Count);
//            Assert.AreEqual(1, FirstSchoolContext.FirstStudentSl.AnnouncementService.GetAnnouncements(false, 0, 10, null).Count);
//            Assert.AreEqual(1, FirstSchoolContext.SecondTeacherSl.AnnouncementService.GetAnnouncements(false, 0, 10, null).Count);
//            Assert.AreEqual(0, FirstSchoolContext.SecondStudentSl.AnnouncementService.GetAnnouncements(false, 0, 10, null).Count);

//            Assert.AreEqual(2, FirstSchoolContext.AdminGradeSl.AnnouncementService.GetAnnouncementRecipients(annDetails.Id).Count);
//            recipientInfo.Add(RecipientInfo.Create(false, CoreRoles.STUDENT_ROLE.Id, null, null));
//            annService.EditAnnouncement(AnnouncementInfo.Create(annDetails), null, null, recipientInfo);
//            Assert.AreEqual(1, FirstSchoolContext.SecondStudentSl.AnnouncementService.GetAnnouncements(false, 0, 10, null).Count);
//            recipientInfo.Add(RecipientInfo.Create(true, null, null, null));
//            annService.EditAnnouncement(AnnouncementInfo.Create(annDetails), null, null, recipientInfo);
//            Assert.AreEqual(1, FirstSchoolContext.SecondStudentSl.AnnouncementService.GetAnnouncements(false, 0, 10, null).Count);           

//        }

//        [Test]
//        public void DeleteAnnouncementTest()
//        {
            
//            var mathClass1 = ClassServiceTest.CreateClass(FirstSchoolContext, FirstSchoolContext.FirstTeacher, 
//                FirstSchoolContext.FirstStudent, FirstSchoolContext.SecondStudent);
//            var mathClass2 = ClassServiceTest.CreateClass(FirstSchoolContext, FirstSchoolContext.FirstTeacher,
//                FirstSchoolContext.FirstStudent, FirstSchoolContext.SecondStudent, "math2");
//            var teacherAnnService = FirstSchoolContext.FirstTeacherSl.AnnouncementService;

//            var type = FirstSchoolContext.FirstTeacherSl.AnnouncementTypeService.GetAnnouncementTypeBySystemType(SystemAnnouncementType.HW);
//            teacherAnnService.CreateAnnouncement(type.Id);
//            AssertForDeny(sl => sl.AnnouncementService.DeleteAnnouncements(FirstSchoolContext.FirstTeacher.Id), FirstSchoolContext, 
//                SchoolContextRoles.AdminGrade | SchoolContextRoles.FirstStudent | SchoolContextRoles.AdminEditor
//                | SchoolContextRoles.AdminViewer | SchoolContextRoles.SecondTeacher);


//            teacherAnnService.DeleteAnnouncements(FirstSchoolContext.FirstTeacher.Id);
//            Assert.IsNull(teacherAnnService.GetLastDraft());
//            FirstSchoolContext.FirstTeacherSl.AnnouncementService.CreateAnnouncement(type.Id, mathClass1.Id);
//            FirstSchoolContext.FirstTeacherSl.AnnouncementService.DeleteAnnouncements(mathClass1.Id, type.Id, AnnouncementState.Draft);
//            Assert.IsNull(teacherAnnService.GetLastDraft());

//            teacherAnnService.CreateAnnouncement(type.Id, mathClass1.Id);
//            teacherAnnService.DeleteAnnouncements(mathClass2.Id, type.Id, AnnouncementState.Draft);
//            Assert.IsNotNull(teacherAnnService.GetLastDraft());

//            var announcement2 = teacherAnnService.CreateAnnouncement(type.Id);
//            AssertForDeny(sl => sl.AnnouncementService.DeleteAnnouncement(announcement2.Id), FirstSchoolContext,
//                SchoolContextRoles.AdminGrade | SchoolContextRoles.FirstStudent | SchoolContextRoles.AdminEditor
//                | SchoolContextRoles.AdminViewer | SchoolContextRoles.SecondTeacher);

//            teacherAnnService.DeleteAnnouncement(announcement2.Id);
//            Assert.IsNull(teacherAnnService.GetLastDraft());

//            var announcement3 = teacherAnnService.CreateAnnouncement(type.Id);
//            var mp = FirstSchoolContext.FirstTeacherSl.MarkingPeriodService.GetLastMarkingPeriod(FirstSchoolContext.NowDate);
//            teacherAnnService.SubmitAnnouncement(announcement3.Id, mathClass1.Id, mp.Id);
//            teacherAnnService.DeleteAnnouncements(FirstSchoolContext.FirstTeacher.Id);
//            Assert.IsNotNull(teacherAnnService.GetAnnouncementDetails(announcement3.Id));
//            teacherAnnService.DeleteAnnouncements(mathClass1.Id, type.Id, AnnouncementState.Draft);
//            Assert.IsNotNull(teacherAnnService.GetAnnouncementById(announcement3.Id));
//            teacherAnnService.DeleteAnnouncements(FirstSchoolContext.FirstTeacher.Id, AnnouncementState.Created);
//            AssertException<Exception>(() => teacherAnnService.GetAnnouncementById(announcement3.Id));

//            var announcement4 = teacherAnnService.CreateAnnouncement(type.Id);


//            teacherAnnService.SubmitAnnouncement(announcement4.Id, mathClass1.Id, mp.Id);
            
//            var items = FirstSchoolContext.FirstTeacherSl.StudentAnnouncementService.GetStudentAnnouncements(announcement4.Id);
//            FirstSchoolContext.FirstTeacherSl.StudentAnnouncementService.SetGrade(items[0].Id, 70, "0", null, false);

//            teacherAnnService.DeleteAnnouncements(mathClass1.Id, type.Id, AnnouncementState.Created);
//            AssertException<Exception>(() => teacherAnnService.GetAnnouncementById(announcement4.Id));
//        }

//        [Test]
//        public void StarringAnnouncementTest()
//        {
//            //TODO: implement
//        }

//        public static AnnouncementDetails CreateDefaulTeacherAnnouncement(IServiceLocatorSchool locator, Guid classId
//            , Guid mpId, DateTime? expiredDate = null, SystemAnnouncementType systemType = SystemAnnouncementType.Standard)
//        {
//            var annType = locator.AnnouncementTypeService.GetAnnouncementTypeBySystemType(systemType);
//            var res = locator.AnnouncementService.CreateAnnouncement(annType.Id, classId);
//            res.Expires = expiredDate ?? locator.Context.NowSchoolTime.Date.AddDays(3);
//            res.Subject = "subject1";
//            res.Content = "content1";
//            locator.AnnouncementService.EditAnnouncement(AnnouncementInfo.Create(res));
//            locator.AnnouncementService.SubmitAnnouncement(res.Id, classId, mpId);
//            return locator.AnnouncementService.GetAnnouncementDetails(res.Id);
//        }
//    }
//}
