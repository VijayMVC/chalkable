using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Tests.Services.Master;
using NUnit.Framework;

namespace Chalkable.Tests.Services.School
{
    public class CourseServiceTest : BaseSchoolServiceTest
    {
        [Test]
        public void AddDeleteCourseTest()
        {
            AssertForDeny(sl=>sl.CourseService.Add("testCourse", "testCourse", null), SchoolTestContext, SchoolContextRoles.FirstParent
                | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstTeacher | SchoolContextRoles.AdminViewer
                | SchoolContextRoles.Checkin);

            AssertException<Exception>(()=> SchoolTestContext.AdminGradeSl.CourseService.Add("testCourse", "TestCourse", null, Guid.NewGuid()));

            Image icon;
            byte[] iconContent;
            PictureServiceTest.LoadImage(DefaulImage1Path, out icon, out iconContent);
            var department = SysAdminMasterLocator.ChalkableDepartmentService.Add("department1", "department1", iconContent);
            var course1 = SchoolTestContext.AdminGradeSl.CourseService.Add("001", "first course", null, department.Id);
            var adminMasterSl = SchoolTestContext.AdminGradeSl.ServiceLocatorMaster;
            Assert.AreEqual(course1.Code, "001");
            Assert.AreEqual(course1.Title, "first course");
            Assert.AreEqual(course1.ChalkableDepartmentRef, department.Id);
            Assert.AreEqual(adminMasterSl.CourseIconService.GetPicture(course1.Id, null, null), iconContent);
            AssertAreEqual(course1, SchoolTestContext.AdminGradeSl.CourseService.GetCourseById(course1.Id));
            Assert.AreEqual(SchoolTestContext.AdminGradeSl.CourseService.GetCourses().Count, 1);
            byte[] iconContent2;
            Image icon2;
            PictureServiceTest.LoadImage(DefaulImage2Path, out icon2, out iconContent2);
            var course2 = SchoolTestContext.AdminGradeSl.CourseService.Add("002", "second course", iconContent2, department.Id);
            Assert.AreEqual(adminMasterSl.CourseIconService.GetPicture(course2.Id, null, null), iconContent2);
            Assert.AreEqual(SchoolTestContext.AdminGradeSl.CourseService.GetCourses().Count, 2);
            

            AssertForDeny(sl => sl.CourseService.Edit(course1.Id, "testCourse", "testCourse", null), SchoolTestContext, SchoolContextRoles.FirstParent
                | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstTeacher | SchoolContextRoles.AdminViewer
                | SchoolContextRoles.Checkin);
            AssertException<Exception>(() => SchoolTestContext.AdminGradeSl.CourseService.Edit(course1.Id,"003", "third course", null, Guid.NewGuid()));

            course1 = SchoolTestContext.AdminGradeSl.CourseService.Edit(course1.Id, "003", "third course", null);
            Assert.AreEqual(course1.Code, "003");
            Assert.AreEqual(course1.Title, "third course");
            Assert.AreEqual(course1.ChalkableDepartmentRef, null);
            Assert.AreEqual(adminMasterSl.CourseIconService.GetPicture(course1.Id, null, null), iconContent);
            AssertAreEqual(course1, SchoolTestContext.AdminGradeSl.CourseService.GetCourseById(course1.Id));
            course1 = SchoolTestContext.AdminGradeSl.CourseService.Edit(course1.Id, "003", "third course", iconContent2, department.Id);
            Assert.AreEqual(adminMasterSl.CourseIconService.GetPicture(course1.Id, null, null), iconContent2);
            Assert.AreEqual(course1.ChalkableDepartmentRef, department.Id);

            AssertForDeny(sl => sl.CourseService.Delete(course1.Id), SchoolTestContext, SchoolContextRoles.FirstParent
                | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstTeacher | SchoolContextRoles.AdminViewer
                | SchoolContextRoles.Checkin);

            SchoolTestContext.AdminGradeSl.CourseService.Delete(course1.Id);
            Assert.IsNull(adminMasterSl.CourseIconService.GetPicture(course1.Id, null, null));  
            Assert.AreEqual(SchoolTestContext.AdminGradeSl.CourseService.GetCourses().Count, 1);
            SchoolTestContext.AdminGradeSl.CourseService.Delete(course2.Id);
            Assert.AreEqual(SchoolTestContext.AdminGradeSl.CourseService.GetCourses().Count, 0);
        }
    }
}
