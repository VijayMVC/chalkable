//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Chalkable.Tests.Services.TestContext;
//using NUnit.Framework;

//namespace Chalkable.Tests.Services.School
//{
//    public class DisciplineTypeServiceTest : BaseSchoolServiceTest
//    {
//         [Test]
//        public void DisciplineTypeTest()
//        {
//            var dType1 = FirstSchoolContext.AdminGradeSl.DisciplineTypeService.Add("Fighting", 10);
//            var dType2 = FirstSchoolContext.AdminGradeSl.DisciplineTypeService.Add("Speaking", 3);
//            Assert.AreEqual(dType1.Name, "Fighting");
//            Assert.AreEqual(dType1.Score, 10);
//            AssertAreEqual(dType1, FirstSchoolContext.FirstTeacherSl.DisciplineTypeService.GetDisciplineTypeById(dType1.Id));

//            //Security check
//            AssertForDeny(sl => sl.DisciplineTypeService.Add("Playing", 1), FirstSchoolContext
//                , SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
//                | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);
//            AssertForDeny(sl => sl.DisciplineTypeService.Edit(dType2.Id, "Playing", 1), FirstSchoolContext
//                , SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
//                | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);
//            AssertForDeny(sl => sl.DisciplineTypeService.Delete(dType2.Id), FirstSchoolContext
//                , SchoolContextRoles.FirstParent | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
//                | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);

//            Assert.AreEqual(2, FirstSchoolContext.FirstTeacherSl.DisciplineTypeService.GetDisciplineTypes(0, 10).TotalCount);
//            FirstSchoolContext.AdminGradeSl.DisciplineTypeService.Delete(dType2.Id);
//            Assert.AreEqual(1, FirstSchoolContext.FirstTeacherSl.DisciplineTypeService.GetDisciplineTypes(0, 10).TotalCount);
//            Assert.AreEqual("Fighting", FirstSchoolContext.FirstTeacherSl.DisciplineTypeService.GetDisciplineTypes(0, 10).First().Name);
//            Assert.AreEqual(10, FirstSchoolContext.FirstTeacherSl.DisciplineTypeService.GetDisciplineTypes(0, 10).First().Score);
//            dType1 = FirstSchoolContext.AdminGradeSl.DisciplineTypeService.Edit(dType1.Id, "Playing", 5);
//            Assert.AreEqual(dType1.Name, "Playing");
//            Assert.AreEqual(dType1.Score, 5);
//            Assert.AreEqual("Playing", FirstSchoolContext.FirstTeacherSl.DisciplineTypeService.GetDisciplineTypes(0, 10).First().Name);
//            Assert.AreEqual(5, FirstSchoolContext.FirstTeacherSl.DisciplineTypeService.GetDisciplineTypes(0, 10).First().Score);
//        }
//    }
//}
