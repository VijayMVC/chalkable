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
//    public class AttendanceReasonServiceTest : BaseSchoolServiceTest
//    {
//        [Test]
//        public void AttReasonTest()
//        {
//            AssertForDeny(sl => sl.AttendanceReasonService.Add(AttendanceTypeEnum.Excused, "code1", "reason 1"), FirstSchoolContext,
//                SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.AdminViewer | SchoolContextRoles.FirstParent);

//            var attReason = FirstSchoolContext.AdminGradeSl.AttendanceReasonService.Add(AttendanceTypeEnum.Excused, "code1", "reason 1");
//            Assert.AreEqual(attReason.Code, "code1");
//            Assert.AreEqual(attReason.AttendanceType, AttendanceTypeEnum.Excused);
//            Assert.AreEqual(attReason.Description, "reason 1");

//            Assert.AreEqual(FirstSchoolContext.AdminGradeSl.AttendanceReasonService.List().Count, 1);
//            Assert.AreEqual(FirstSchoolContext.FirstTeacherSl.AttendanceReasonService.List().Count, 1);

//            AssertForDeny(sl => sl.AttendanceReasonService.Edit(attReason.Id, AttendanceTypeEnum.Excused, "code1", "reason 1"), FirstSchoolContext,
//                SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent | SchoolContextRoles.AdminViewer);

//            attReason = FirstSchoolContext.AdminGradeSl.AttendanceReasonService.Edit(attReason.Id, AttendanceTypeEnum.Late, "code2", "reason 2");
//            Assert.AreEqual(attReason.Code, "code2");
//            Assert.AreEqual(attReason.AttendanceType, AttendanceTypeEnum.Late);
//            Assert.AreEqual(attReason.Description, "reason 2");

//            AssertForDeny(sl => sl.AttendanceReasonService.Delete(attReason.Id), FirstSchoolContext, SchoolContextRoles.FirstTeacher
//                | SchoolContextRoles.FirstStudent | SchoolContextRoles.AdminViewer | SchoolContextRoles.FirstParent);

//            FirstSchoolContext.AdminGradeSl.AttendanceReasonService.Delete(attReason.Id);
//        }
//    }
//}
