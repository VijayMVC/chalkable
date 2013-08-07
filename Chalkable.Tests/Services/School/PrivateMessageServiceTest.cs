using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services.School;
using NUnit.Framework;

namespace Chalkable.Tests.Services.School
{
    public class PrivateMessageServiceTest : BaseSchoolServiceTest
    {
        [Test]
        public void SendRecieveMessageTest()
        {
            var adminMsg = SchoolTestContext.AdminGradeSl.PrivateMessageService.SendMessage(SchoolTestContext.FirstTeacher.Id, "Hello", "Hello Mr. Teacher");

            Assert.AreEqual(0, SchoolTestContext.AdminGradeSl.PrivateMessageService.GetMessages(0, 10, null, PrivateMessageType.Income, "", "").TotalCount);
            var adminMsgs = SchoolTestContext.AdminGradeSl.PrivateMessageService.GetMessages(0, 10, null, PrivateMessageType.Outcome, "", "");
            Assert.AreEqual(1, adminMsgs.TotalCount);
            Assert.AreEqual("Hello", adminMsgs.First().Subject);
            Assert.AreEqual("Hello Mr. Teacher", adminMsgs.First().Body);
            //Only recipients can mark message as read
            var msgIds = new List<Guid> {adminMsg.Id};
            
            AssertForDeny(sl=>sl.PrivateMessageService.MarkAsRead(msgIds, true), SchoolTestContext
                , SchoolContextRoles.FirstStudent | SchoolContextRoles.AdminGrade | SchoolContextRoles.SecondTeacher);
            AssertForDeny(sl => sl.PrivateMessageService.Delete(msgIds), SchoolTestContext
                , SchoolContextRoles.FirstStudent | SchoolContextRoles.SecondTeacher);

            SchoolTestContext.AdminGradeSl.PrivateMessageService.Delete(msgIds);
            adminMsgs = SchoolTestContext.AdminGradeSl.PrivateMessageService.GetMessages(0, 10, null, PrivateMessageType.Outcome, "", "");
            Assert.AreEqual(0, adminMsgs.TotalCount);

            //check notification
            Assert.AreEqual(1, SchoolTestContext.FirstTeacherSl.NotificationService.GetUnshownNotifications().Count);
            Assert.AreEqual(adminMsg.Id, SchoolTestContext.FirstTeacherSl.NotificationService.GetUnshownNotifications().First().PrivateMessageRef);
            //
            var teacherMsgs = SchoolTestContext.FirstTeacherSl.PrivateMessageService.GetMessages(0, 10, null, PrivateMessageType.Income, "", "");
            msgIds = new List<Guid> { teacherMsgs.First().Id };
            Assert.AreEqual(1, teacherMsgs.TotalCount);
            Assert.AreEqual("Hello", teacherMsgs.First().Subject);
            Assert.AreEqual("Hello Mr. Teacher", teacherMsgs.First().Body);
            Assert.IsFalse(teacherMsgs.First().Read);
            SchoolTestContext.FirstTeacherSl.PrivateMessageService.MarkAsRead(msgIds, true);
            teacherMsgs = SchoolTestContext.FirstTeacherSl.PrivateMessageService.GetMessages(0, 10, null, PrivateMessageType.Income, "", "");
            Assert.IsTrue(teacherMsgs.First().Read);
            Assert.AreEqual(1, SchoolTestContext.FirstTeacherSl.PrivateMessageService.GetMessages(0, 10, true, PrivateMessageType.Income, "", "").TotalCount);
            Assert.AreEqual(0, SchoolTestContext.FirstTeacherSl.PrivateMessageService.GetMessages(0, 10, false, PrivateMessageType.Income, "", "").TotalCount);
            SchoolTestContext.FirstTeacherSl.PrivateMessageService.Delete(msgIds);
            Assert.AreEqual(0, SchoolTestContext.FirstTeacherSl.PrivateMessageService.GetMessages(0, 10, null, PrivateMessageType.Income, "", "").TotalCount);

            SchoolTestContext.FirstStudentSl.PrivateMessageService.SendMessage(SchoolTestContext.SecondStudent.Id, "Test", "How are you?");
            SchoolTestContext.FirstStudentSl.PrivateMessageService.SendMessage(SchoolTestContext.FirstParent.Id, "Test", "How are you?");
            Assert.AreEqual(2, SchoolTestContext.FirstStudentSl.PrivateMessageService.GetMessages(0, 10, null, PrivateMessageType.Outcome, "", "").TotalCount);
            Assert.AreEqual(1, SchoolTestContext.SecondStudentSl.PrivateMessageService.GetMessages(0, 10, null, PrivateMessageType.Income, "", "").TotalCount);
            Assert.AreEqual(1, SchoolTestContext.FirstParentSl.PrivateMessageService.GetMessages(0, 10, null, PrivateMessageType.Income, "", "").TotalCount);
        }
    }
}
