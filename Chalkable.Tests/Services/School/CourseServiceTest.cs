using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            
        }
    }
}
