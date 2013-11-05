using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Tests.Services.TestContext;
using NUnit.Framework;

namespace Chalkable.Tests.Services.School
{
    public class PrivateMessageServiceTest : BaseSchoolServiceTest
    {
        [Test]
        public void SendRecieveMessageTest()
        {
            AssertForDeny(sl=>sl.PrivateMessageService.SendMessage(FirstSchoolContext.FirstTeacher.Id, "Hello", "Hello Mr. Teacher")
                ,SecondSchoolContext, SchoolContextRoles.AdminGrade | SchoolContextRoles.AdminEditor | SchoolContextRoles.AdminViewer
                | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent);

            var adminMsg = FirstSchoolContext.AdminGradeSl.PrivateMessageService.SendMessage(FirstSchoolContext.FirstTeacher.Id, "Hello", "Hello Mr. Teacher");

            Assert.AreEqual(0, FirstSchoolContext.AdminGradeSl.PrivateMessageService.GetMessages(0, 10, null, PrivateMessageType.Income, "", "").TotalCount);
            Assert.AreEqual(0, FirstSchoolContext.AdminEditSl.PrivateMessageService.GetMessages(0, 10, null, PrivateMessageType.Income, "", "").TotalCount);
            Assert.AreEqual(0, FirstSchoolContext.AdminEditSl.PrivateMessageService.GetMessages(0, 10, null, PrivateMessageType.Outcome, "", "").TotalCount);
            var adminMsgs = FirstSchoolContext.AdminGradeSl.PrivateMessageService.GetMessages(0, 10, null, PrivateMessageType.Outcome, "", "");
            Assert.AreEqual(1, adminMsgs.TotalCount);
            Assert.AreEqual("Hello", adminMsgs.First().Subject);
            Assert.AreEqual("Hello Mr. Teacher", adminMsgs.First().Body);
            //Only recipients can mark message as read
            var msgIds = new List<int> { adminMsg.Id };

            AssertForDeny(sl => sl.PrivateMessageService.MarkAsRead(msgIds, true), FirstSchoolContext
                , SchoolContextRoles.FirstStudent | SchoolContextRoles.AdminGrade | SchoolContextRoles.SecondTeacher);
            AssertForDeny(sl => sl.PrivateMessageService.Delete(msgIds), FirstSchoolContext
                , SchoolContextRoles.FirstStudent | SchoolContextRoles.SecondTeacher);

            FirstSchoolContext.AdminGradeSl.PrivateMessageService.Delete(msgIds);
            adminMsgs = FirstSchoolContext.AdminGradeSl.PrivateMessageService.GetMessages(0, 10, null, PrivateMessageType.Outcome, "", "");
            Assert.AreEqual(0, adminMsgs.TotalCount);

            //todo :check notification
            Assert.AreEqual(1, FirstSchoolContext.FirstTeacherSl.NotificationService.GetUnshownNotifications().Count);
            Assert.AreEqual(adminMsg.Id, FirstSchoolContext.FirstTeacherSl.NotificationService.GetUnshownNotifications().First().PrivateMessageRef);
            
            var teacherMsgs = FirstSchoolContext.FirstTeacherSl.PrivateMessageService.GetMessages(0, 10, null, PrivateMessageType.Income, "", "");
            msgIds = new List<int> { teacherMsgs.First().Id };
            Assert.AreEqual(1, teacherMsgs.TotalCount);
            Assert.AreEqual("Hello", teacherMsgs.First().Subject);
            Assert.AreEqual("Hello Mr. Teacher", teacherMsgs.First().Body);
            Assert.IsFalse(teacherMsgs.First().Read);
            FirstSchoolContext.FirstTeacherSl.PrivateMessageService.MarkAsRead(msgIds, true);
            teacherMsgs = FirstSchoolContext.FirstTeacherSl.PrivateMessageService.GetMessages(0, 10, null, PrivateMessageType.Income, "", "");
            Assert.IsTrue(teacherMsgs.First().Read);
            Assert.AreEqual(1, FirstSchoolContext.FirstTeacherSl.PrivateMessageService.GetMessages(0, 10, true, PrivateMessageType.Income, "", "").TotalCount);
            Assert.AreEqual(0, FirstSchoolContext.FirstTeacherSl.PrivateMessageService.GetMessages(0, 10, false, PrivateMessageType.Income, "", "").TotalCount);
            FirstSchoolContext.FirstTeacherSl.PrivateMessageService.Delete(msgIds);
            Assert.AreEqual(0, FirstSchoolContext.FirstTeacherSl.PrivateMessageService.GetMessages(0, 10, null, PrivateMessageType.Income, "", "").TotalCount);

            FirstSchoolContext.FirstStudentSl.PrivateMessageService.SendMessage(FirstSchoolContext.SecondStudent.Id, "Test", "How are you?");
            FirstSchoolContext.FirstStudentSl.PrivateMessageService.SendMessage(FirstSchoolContext.FirstParent.Id, "Test", "How are you?");
            Assert.AreEqual(2, FirstSchoolContext.FirstStudentSl.PrivateMessageService.GetMessages(0, 10, null, PrivateMessageType.Outcome, "", "").TotalCount);
            Assert.AreEqual(1, FirstSchoolContext.SecondStudentSl.PrivateMessageService.GetMessages(0, 10, null, PrivateMessageType.Income, "", "").TotalCount);
            Assert.AreEqual(1, FirstSchoolContext.FirstParentSl.PrivateMessageService.GetMessages(0, 10, null, PrivateMessageType.Income, "", "").TotalCount);
        }
    }
}
