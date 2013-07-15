using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.School.Model;
using NUnit.Framework;

namespace Chalkable.Tests.Services.School
{
    //TODO: notification sending test 

    public class AnnouncementQnServiceTest : BaseSchoolServiceTest
    {
        [Test]
        public void QuestionAnswerTest()
        {
            var c = ClassServiceTest.CreateClass(SchoolTestContext, SchoolTestContext.FirstTeacher,
                                                 SchoolTestContext.FirstStudent, null);

            var annType = SchoolTestContext.FirstStudentSl.AnnouncementTypeService.GetAnnouncementTypeBySystemType(SystemAnnouncementType.HW);
            var announcement = SchoolTestContext.FirstTeacherSl.AnnouncementService.CreateAnnouncement(annType.Id, c.Id);
            var currentMpId = c.MarkingPeriodClasses[0].MarkingPeriodRef;  
            SchoolTestContext.FirstTeacherSl.AnnouncementService.SubmitAnnouncement(announcement.Id, c.Id, currentMpId);
            announcement = SchoolTestContext.FirstTeacherSl.AnnouncementService.GetAnnouncementDetails(announcement.Id);

            AssertForDeny(sl => sl.AnnouncementQnAService.AskQuestion(announcement.Id, "question"), SchoolTestContext
                          , SchoolContextRoles.FirstParent | SchoolContextRoles.SecondStudent 
                          | SchoolContextRoles.SecondTeacher | SchoolContextRoles.Checkin);

            var annQuestion = SchoolTestContext.FirstStudentSl.AnnouncementQnAService.AskQuestion(announcement.Id, "question");
            Assert.AreEqual(annQuestion.PersonRef, SchoolTestContext.FirstStudent.Id);
            Assert.AreEqual(annQuestion.Question, "question");
            Assert.AreEqual(annQuestion.State, AnnouncementQnAState.Asked);
            Assert.AreEqual(annQuestion.Answer, null);
            AssertAreEqual(annQuestion, SchoolTestContext.FirstStudentSl.AnnouncementQnAService.GetAnnouncmentQnA(annQuestion.Id));
            Assert.AreEqual(SchoolTestContext.FirstStudentSl.AnnouncementQnAService.GetAnnouncementQnAs(announcement.Id).Count, 1);

            AssertForDeny(sl => sl.AnnouncementQnAService.Answer(annQuestion.Id, "question", "answer"), SchoolTestContext
                          , SchoolContextRoles.FirstParent | SchoolContextRoles.SecondStudent | SchoolContextRoles.FirstStudent
                          | SchoolContextRoles.SecondTeacher | SchoolContextRoles.Checkin);

            annQuestion = SchoolTestContext.FirstTeacherSl.AnnouncementQnAService.Answer(annQuestion.Id, "question", "answer");
            Assert.AreEqual(annQuestion.Answer, "answer");
            Assert.AreEqual(annQuestion.State, AnnouncementQnAState.Answered);
            Assert.AreEqual(annQuestion.AnsweredTime.Date, SchoolTestContext.NowDate);

            AssertAreEqual(annQuestion, SchoolTestContext.FirstStudentSl.AnnouncementQnAService.GetAnnouncmentQnA(annQuestion.Id));
            Assert.AreEqual(SchoolTestContext.FirstStudentSl.AnnouncementQnAService.GetAnnouncementQnAs(announcement.Id).Count, 1);

            //security check
            AssertForDeny(sl => sl.AnnouncementQnAService.EditAnswer(annQuestion.Id, "answer2"), SchoolTestContext
              , SchoolContextRoles.FirstParent | SchoolContextRoles.SecondStudent | SchoolContextRoles.FirstStudent
              | SchoolContextRoles.SecondTeacher | SchoolContextRoles.Checkin);
            AssertForDeny(sl => sl.AnnouncementQnAService.EditQuestion(annQuestion.Id, "question2"), SchoolTestContext
                          , SchoolContextRoles.FirstParent | SchoolContextRoles.SecondStudent | SchoolContextRoles.FirstStudent
                          | SchoolContextRoles.SecondTeacher | SchoolContextRoles.Checkin);
            AssertForDeny(sl => sl.AnnouncementQnAService.MarkUnanswered(annQuestion.Id), SchoolTestContext
                          , SchoolContextRoles.FirstParent | SchoolContextRoles.SecondStudent | SchoolContextRoles.FirstStudent
                          | SchoolContextRoles.SecondTeacher | SchoolContextRoles.Checkin);
            AssertForDeny(sl => sl.AnnouncementQnAService.Delete(annQuestion.Id), SchoolTestContext
                          , SchoolContextRoles.FirstParent | SchoolContextRoles.SecondStudent | SchoolContextRoles.FirstStudent
                          | SchoolContextRoles.SecondTeacher | SchoolContextRoles.Checkin);
            
            annQuestion = SchoolTestContext.FirstTeacherSl.AnnouncementQnAService.EditAnswer(annQuestion.Id, "answer2");
            Assert.AreEqual(annQuestion.Answer, "answer2");
            annQuestion = SchoolTestContext.FirstTeacherSl.AnnouncementQnAService.EditQuestion(annQuestion.Id, "question2");
            Assert.AreEqual(annQuestion.Question, "question2");
            annQuestion = SchoolTestContext.FirstTeacherSl.AnnouncementQnAService.MarkUnanswered(annQuestion.Id);
            Assert.AreEqual(annQuestion.State, AnnouncementQnAState.Unanswered);
            Assert.AreEqual(SchoolTestContext.SecondStudentSl.AnnouncementQnAService.GetAnnouncementQnAs(announcement.Id).Count, 0);
            c = SchoolTestContext.AdminGradeSl.ClassService.AddStudent(c.Id, SchoolTestContext.SecondStudent.Id);
            Assert.AreEqual(SchoolTestContext.SecondStudentSl.AnnouncementQnAService.GetAnnouncementQnAs(announcement.Id).Count, 1);
            SchoolTestContext.FirstTeacherSl.AnnouncementQnAService.Delete(annQuestion.Id);
            Assert.AreEqual(SchoolTestContext.FirstTeacherSl.AnnouncementQnAService.GetAnnouncementQnAs(announcement.Id).Count, 0);  

        }
    }
}
