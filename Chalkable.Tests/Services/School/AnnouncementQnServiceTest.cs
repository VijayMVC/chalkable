//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Chalkable.Data.School.Model;
//using Chalkable.Tests.Services.TestContext;
//using NUnit.Framework;

//namespace Chalkable.Tests.Services.School
//{
//    //TODO: notification sending test 

//    public class AnnouncementQnServiceTest : BaseSchoolServiceTest
//    {
//        [Test]
//        public void QuestionAnswerTest()
//        {
//            var c = ClassServiceTest.CreateClass(FirstSchoolContext, FirstSchoolContext.FirstTeacher,
//                                                 FirstSchoolContext.FirstStudent, null);

//            var annType = FirstSchoolContext.FirstStudentSl.AnnouncementTypeService.GetAnnouncementTypeBySystemType(SystemAnnouncementType.HW);
//            var announcement = FirstSchoolContext.FirstTeacherSl.AnnouncementService.CreateAnnouncement(annType.Id, c.Id);
//            var currentMpId = c.MarkingPeriodClasses[0].MarkingPeriodRef;  
//            FirstSchoolContext.FirstTeacherSl.AnnouncementService.SubmitAnnouncement(announcement.Id, c.Id, currentMpId);
//            announcement = FirstSchoolContext.FirstTeacherSl.AnnouncementService.GetAnnouncementDetails(announcement.Id);

//            AssertForDeny(sl => sl.AnnouncementQnAService.AskQuestion(announcement.Id, "question"), FirstSchoolContext
//                          , SchoolContextRoles.FirstParent | SchoolContextRoles.SecondStudent 
//                          | SchoolContextRoles.SecondTeacher | SchoolContextRoles.Checkin);

//            var annQuestion = FirstSchoolContext.FirstStudentSl.AnnouncementQnAService.AskQuestion(announcement.Id, "question");
//            //check notification
//            Assert.AreEqual(1, FirstSchoolContext.FirstTeacherSl.NotificationService.GetUnshownNotifications().Count);
//            Assert.AreEqual(announcement.Id, FirstSchoolContext.FirstTeacherSl.NotificationService.GetUnshownNotifications().First().AnnouncementRef);
            

//            Assert.AreEqual(annQuestion.PersonRef, FirstSchoolContext.FirstStudent.Id);
//            Assert.AreEqual(annQuestion.Question, "question");
//            Assert.AreEqual(annQuestion.State, AnnouncementQnAState.Asked);
//            Assert.AreEqual(annQuestion.Answer, null);
//            AssertAreEqual(annQuestion, FirstSchoolContext.FirstStudentSl.AnnouncementQnAService.GetAnnouncementQnA(annQuestion.Id));
//            Assert.AreEqual(FirstSchoolContext.FirstStudentSl.AnnouncementQnAService.GetAnnouncementQnAs(announcement.Id).Count, 1);

//            AssertForDeny(sl => sl.AnnouncementQnAService.Answer(annQuestion.Id, "question", "answer"), FirstSchoolContext
//                          , SchoolContextRoles.FirstParent | SchoolContextRoles.SecondStudent | SchoolContextRoles.FirstStudent
//                          | SchoolContextRoles.SecondTeacher | SchoolContextRoles.Checkin);

//            annQuestion = FirstSchoolContext.FirstTeacherSl.AnnouncementQnAService.Answer(annQuestion.Id, "question", "answer");
//            //check notification
//            Assert.AreEqual(1, FirstSchoolContext.FirstStudentSl.NotificationService.GetUnshownNotifications().Count);
//            Assert.AreEqual(announcement.Id, FirstSchoolContext.FirstStudentSl.NotificationService.GetUnshownNotifications().First().AnnouncementRef);
            
//            Assert.AreEqual(annQuestion.Answer, "answer");
//            Assert.AreEqual(annQuestion.State, AnnouncementQnAState.Answered);
//            Assert.AreEqual(annQuestion.AnsweredTime.Value.Date, FirstSchoolContext.NowDate);

//            AssertAreEqual(annQuestion, FirstSchoolContext.FirstStudentSl.AnnouncementQnAService.GetAnnouncementQnA(annQuestion.Id));
//            Assert.AreEqual(FirstSchoolContext.FirstStudentSl.AnnouncementQnAService.GetAnnouncementQnAs(announcement.Id).Count, 1);

//            //security check
//            AssertForDeny(sl => sl.AnnouncementQnAService.EditAnswer(annQuestion.Id, "answer2"), FirstSchoolContext
//              , SchoolContextRoles.FirstParent | SchoolContextRoles.SecondStudent | SchoolContextRoles.FirstStudent
//              | SchoolContextRoles.SecondTeacher | SchoolContextRoles.Checkin);
//            AssertForDeny(sl => sl.AnnouncementQnAService.EditQuestion(annQuestion.Id, "question2"), FirstSchoolContext
//                          , SchoolContextRoles.FirstParent | SchoolContextRoles.SecondStudent | SchoolContextRoles.FirstStudent
//                          | SchoolContextRoles.SecondTeacher | SchoolContextRoles.Checkin);
//            AssertForDeny(sl => sl.AnnouncementQnAService.MarkUnanswered(annQuestion.Id), FirstSchoolContext
//                          , SchoolContextRoles.FirstParent | SchoolContextRoles.SecondStudent | SchoolContextRoles.FirstStudent
//                          | SchoolContextRoles.SecondTeacher | SchoolContextRoles.Checkin);
//            AssertForDeny(sl => sl.AnnouncementQnAService.Delete(annQuestion.Id), FirstSchoolContext
//                          , SchoolContextRoles.FirstParent | SchoolContextRoles.SecondStudent | SchoolContextRoles.FirstStudent
//                          | SchoolContextRoles.SecondTeacher | SchoolContextRoles.Checkin);
            
//            annQuestion = FirstSchoolContext.FirstTeacherSl.AnnouncementQnAService.EditAnswer(annQuestion.Id, "answer2");
//            Assert.AreEqual(annQuestion.Answer, "answer2");
//            annQuestion = FirstSchoolContext.FirstTeacherSl.AnnouncementQnAService.EditQuestion(annQuestion.Id, "question2");
//            Assert.AreEqual(annQuestion.Question, "question2");
//            annQuestion = FirstSchoolContext.FirstTeacherSl.AnnouncementQnAService.MarkUnanswered(annQuestion.Id);
//            Assert.AreEqual(annQuestion.State, AnnouncementQnAState.Unanswered);
//            Assert.AreEqual(FirstSchoolContext.SecondStudentSl.AnnouncementQnAService.GetAnnouncementQnAs(announcement.Id).Count, 0);
//            c = FirstSchoolContext.AdminGradeSl.ClassService.AddStudent(c.Id, FirstSchoolContext.SecondStudent.Id);
//            Assert.AreEqual(FirstSchoolContext.SecondStudentSl.AnnouncementQnAService.GetAnnouncementQnAs(announcement.Id).Count, 1);
//            FirstSchoolContext.FirstTeacherSl.AnnouncementQnAService.Delete(annQuestion.Id);
//            Assert.AreEqual(FirstSchoolContext.FirstTeacherSl.AnnouncementQnAService.GetAnnouncementQnAs(announcement.Id).Count, 0);  

//        }
//    }
//}
