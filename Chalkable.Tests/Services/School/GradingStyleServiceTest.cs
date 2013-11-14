//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Chalkable.BusinessLogic.Mapping;
//using Chalkable.Data.School.Model;
//using Chalkable.Tests.Services.TestContext;
//using NUnit.Framework;

//namespace Chalkable.Tests.Services.School
//{
//    public class GradingStyleServiceTest : BaseSchoolServiceTest
//    {
//        [Test]
//        public void GetSetTest()
//        {
//            IDictionary<GradingStyleEnum, List<int>> mappers = new Dictionary<GradingStyleEnum, List<int>>();
//            var s1L1 = new List<int> { 100, 96, 92, 89, 86, 82, 79, 76, 72, 67, 59, 51, 50 };
//            mappers.Add(GradingStyleEnum.Abcf, s1L1);
//            var s1L2 = new List<int> { 100, 70, 30 };
//            mappers.Add(GradingStyleEnum.Check, s1L2);
//            var s1L3 = new List<int> { 100, 40 };
//            mappers.Add(GradingStyleEnum.Complete, s1L3);
//            var school1Mapper = GradingStyleMapper.Create(mappers);

//            AssertForDeny(sl => sl.GradingStyleService.SetMapper(school1Mapper), FirstSchoolContext
//                , SchoolContextRoles.AdminEditor | SchoolContextRoles.AdminViewer | SchoolContextRoles.FirstTeacher
//                 | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent | SchoolContextRoles.Checkin);

//            FirstSchoolContext.AdminGradeSl.GradingStyleService.SetMapper(school1Mapper);
            
//            var s1Mapper = FirstSchoolContext.AdminGradeSl.GradingStyleService.GetMapper();
//            AssertAreEqual(s1L1.OrderBy(x => x).ToList(), s1Mapper.GetValuesByStyle(GradingStyleEnum.Abcf));
//            AssertAreEqual(s1L2.OrderBy(x => x).ToList(), s1Mapper.GetValuesByStyle(GradingStyleEnum.Check));
//            AssertAreEqual(s1L3.OrderBy(x => x).ToList(), s1Mapper.GetValuesByStyle(GradingStyleEnum.Complete));

//        }
//    }
//}
