using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Tests.Services.School;
using Chalkable.Tests.Services.TestContext;
using NUnit.Framework;

namespace Chalkable.Tests.Services
{
    public class GradeLevelServiceTest : BaseSchoolServiceTest
    {
        [Test]
        public void AddTest()
        {
            var districtL = DistrictTestContext.DistrictLocatorFirstSchool;
            var glService = districtL.GradeLevelService;
            var glId = 1;
            AssertForDeny(sl=>sl.GradeLevelService.AddGradeLevel(glId, "1th", 1), FirstSchoolContext
                , SchoolContextRoles.AdminGrade | SchoolContextRoles.AdminEditor | SchoolContextRoles.AdminViewer
                | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent);

            glService.AddGradeLevel(glId, "1th", 1);
            var gls = FirstSchoolContext.AdminGradeSl.GradeLevelService.GetGradeLevels();
            Assert.AreEqual(gls.Count, 1);
            Assert.AreEqual(SecondSchoolContext.AdminGradeSl.GradeLevelService.GetGradeLevels().Count, 1);
            var gl = gls[0];
            Assert.AreEqual(gl.Id, glId);
            Assert.AreEqual(gl.Name, "1th");
            Assert.AreEqual(gl.Number, 1);
            var schoolId = FirstSchoolContext.School.LocalId;
            
            AssertForDeny(sl => sl.GradeLevelService.AddSchoolGradeLevel(glId, schoolId), FirstSchoolContext
                , SchoolContextRoles.AdminGrade | SchoolContextRoles.AdminEditor | SchoolContextRoles.AdminViewer
                | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent);

            Assert.AreEqual(glService.GetGradeLevels(schoolId).Count, 0);
            glService.AddSchoolGradeLevel(glId, schoolId);
            Assert.AreEqual(glService.GetGradeLevels(schoolId).Count, 1);

            AssertForDeny(sl => sl.GradeLevelService.DeleteSchoolGradeLevel(glId, schoolId), FirstSchoolContext
                , SchoolContextRoles.AdminGrade | SchoolContextRoles.AdminEditor | SchoolContextRoles.AdminViewer
                | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent);

            glService.DeleteSchoolGradeLevel(glId, schoolId);
            Assert.AreEqual(glService.GetGradeLevels(schoolId).Count, 0);
            
        }
    }
}
