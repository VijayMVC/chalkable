using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Mapping;
using Chalkable.Data.School.Model;
using NUnit.Framework;

namespace Chalkable.Tests.Services.School
{
    public class GradingStyleMappingTest : BaseSchoolServiceTest
    {
        [Test]
        public void MapTest()
        {
            var mapper = MapperDefaulInit();

            Assert.IsNull(mapper.Map(GradingStyleEnum.Abcf, null));

            Assert.AreEqual((AbcfGradingStyle)mapper.Map(GradingStyleEnum.Abcf, 40), AbcfGradingStyle.F);
            Assert.AreEqual((AbcfGradingStyle)mapper.Map(GradingStyleEnum.Abcf, 50), AbcfGradingStyle.F);
            Assert.AreEqual((AbcfGradingStyle)mapper.Map(GradingStyleEnum.Abcf, 51), AbcfGradingStyle.DM);
            Assert.AreEqual((AbcfGradingStyle)mapper.Map(GradingStyleEnum.Abcf, 55), AbcfGradingStyle.D);
            Assert.AreEqual((AbcfGradingStyle)mapper.Map(GradingStyleEnum.Abcf, 59), AbcfGradingStyle.D);
            Assert.AreEqual((AbcfGradingStyle)mapper.Map(GradingStyleEnum.Abcf, 62), AbcfGradingStyle.DP);
            Assert.AreEqual((AbcfGradingStyle)mapper.Map(GradingStyleEnum.Abcf, 67), AbcfGradingStyle.DP);
            Assert.AreEqual((AbcfGradingStyle)mapper.Map(GradingStyleEnum.Abcf, 68), AbcfGradingStyle.CM);
            Assert.AreEqual((AbcfGradingStyle)mapper.Map(GradingStyleEnum.Abcf, 72), AbcfGradingStyle.CM);
            Assert.AreEqual((AbcfGradingStyle)mapper.Map(GradingStyleEnum.Abcf, 73), AbcfGradingStyle.C);
            Assert.AreEqual((AbcfGradingStyle)mapper.Map(GradingStyleEnum.Abcf, 76), AbcfGradingStyle.C);
            Assert.AreEqual((AbcfGradingStyle)mapper.Map(GradingStyleEnum.Abcf, 77), AbcfGradingStyle.CP);
            Assert.AreEqual((AbcfGradingStyle)mapper.Map(GradingStyleEnum.Abcf, 79), AbcfGradingStyle.CP);
            Assert.AreEqual((AbcfGradingStyle)mapper.Map(GradingStyleEnum.Abcf, 80), AbcfGradingStyle.BM);
            Assert.AreEqual((AbcfGradingStyle)mapper.Map(GradingStyleEnum.Abcf, 82), AbcfGradingStyle.BM);
            Assert.AreEqual((AbcfGradingStyle)mapper.Map(GradingStyleEnum.Abcf, 83), AbcfGradingStyle.B);
            Assert.AreEqual((AbcfGradingStyle)mapper.Map(GradingStyleEnum.Abcf, 86), AbcfGradingStyle.B);
            Assert.AreEqual((AbcfGradingStyle)mapper.Map(GradingStyleEnum.Abcf, 87), AbcfGradingStyle.BP);
            Assert.AreEqual((AbcfGradingStyle)mapper.Map(GradingStyleEnum.Abcf, 89), AbcfGradingStyle.BP);
            Assert.AreEqual((AbcfGradingStyle)mapper.Map(GradingStyleEnum.Abcf, 90), AbcfGradingStyle.AM);
            Assert.AreEqual((AbcfGradingStyle)mapper.Map(GradingStyleEnum.Abcf, 92), AbcfGradingStyle.AM);
            Assert.AreEqual((AbcfGradingStyle)mapper.Map(GradingStyleEnum.Abcf, 93), AbcfGradingStyle.A);
            Assert.AreEqual((AbcfGradingStyle)mapper.Map(GradingStyleEnum.Abcf, 96), AbcfGradingStyle.A);
            Assert.AreEqual((AbcfGradingStyle)mapper.Map(GradingStyleEnum.Abcf, 97), AbcfGradingStyle.AP);
            Assert.AreEqual((AbcfGradingStyle)mapper.Map(GradingStyleEnum.Abcf, 100), AbcfGradingStyle.AP);

            Assert.AreEqual((CompleteIncompleteGradingStyle)mapper.Map(GradingStyleEnum.Complete, 0), CompleteIncompleteGradingStyle.Incomplete);
            Assert.AreEqual((CompleteIncompleteGradingStyle)mapper.Map(GradingStyleEnum.Complete, 30), CompleteIncompleteGradingStyle.Incomplete);
            Assert.AreEqual((CompleteIncompleteGradingStyle)mapper.Map(GradingStyleEnum.Complete, 40), CompleteIncompleteGradingStyle.Incomplete);
            Assert.AreEqual((CompleteIncompleteGradingStyle)mapper.Map(GradingStyleEnum.Complete, 50), CompleteIncompleteGradingStyle.Complete);
            Assert.AreEqual((CompleteIncompleteGradingStyle)mapper.Map(GradingStyleEnum.Complete, 100), CompleteIncompleteGradingStyle.Complete);

            Assert.AreEqual((CheckGradingStyle)mapper.Map(GradingStyleEnum.Check, 0), CheckGradingStyle.CheckMinus);
            Assert.AreEqual((CheckGradingStyle)mapper.Map(GradingStyleEnum.Check, 20), CheckGradingStyle.CheckMinus);
            Assert.AreEqual((CheckGradingStyle)mapper.Map(GradingStyleEnum.Check, 30), CheckGradingStyle.CheckMinus);
            Assert.AreEqual((CheckGradingStyle)mapper.Map(GradingStyleEnum.Check, 40), CheckGradingStyle.Check);
            Assert.AreEqual((CheckGradingStyle)mapper.Map(GradingStyleEnum.Check, 70), CheckGradingStyle.Check);
            Assert.AreEqual((CheckGradingStyle)mapper.Map(GradingStyleEnum.Check, 80), CheckGradingStyle.CheckPlus);
            Assert.AreEqual((CheckGradingStyle)mapper.Map(GradingStyleEnum.Check, 100), CheckGradingStyle.CheckPlus);

            for (int i = 0; i <= 100; i++)
                Assert.AreEqual(mapper.Map(GradingStyleEnum.Numeric100, i), i);
        }

        [Test]
        public void MapBackTest()
        {

            var mapper = MapperDefaulInit();
            Assert.IsNull(mapper.MapBack(GradingStyleEnum.Abcf, null));
            Assert.IsNull(mapper.MapBack(GradingStyleEnum.Check, null));
            Assert.IsNull(mapper.MapBack(GradingStyleEnum.Complete, null));
            Assert.IsNull(mapper.MapBack(GradingStyleEnum.Numeric100, null));

            Assert.AreEqual(mapper.MapBack(GradingStyleEnum.Abcf, (int)AbcfGradingStyle.F), 50);
            Assert.AreEqual(mapper.MapBack(GradingStyleEnum.Abcf, (int)AbcfGradingStyle.DM), 51);
            Assert.AreEqual(mapper.MapBack(GradingStyleEnum.Abcf, (int)AbcfGradingStyle.D), 59);
            Assert.AreEqual(mapper.MapBack(GradingStyleEnum.Abcf, (int)AbcfGradingStyle.DP), 67);
            Assert.AreEqual(mapper.MapBack(GradingStyleEnum.Abcf, (int)AbcfGradingStyle.CM), 72);
            Assert.AreEqual(mapper.MapBack(GradingStyleEnum.Abcf, (int)AbcfGradingStyle.C), 76);
            Assert.AreEqual(mapper.MapBack(GradingStyleEnum.Abcf, (int)AbcfGradingStyle.CP), 79);
            Assert.AreEqual(mapper.MapBack(GradingStyleEnum.Abcf, (int)AbcfGradingStyle.BM), 82);
            Assert.AreEqual(mapper.MapBack(GradingStyleEnum.Abcf, (int)AbcfGradingStyle.B), 86);
            Assert.AreEqual(mapper.MapBack(GradingStyleEnum.Abcf, (int)AbcfGradingStyle.BP), 89);
            Assert.AreEqual(mapper.MapBack(GradingStyleEnum.Abcf, (int)AbcfGradingStyle.AM), 92);
            Assert.AreEqual(mapper.MapBack(GradingStyleEnum.Abcf, (int)AbcfGradingStyle.A), 96);
            Assert.AreEqual(mapper.MapBack(GradingStyleEnum.Abcf, (int)AbcfGradingStyle.AP), 100);

            Assert.AreEqual(mapper.MapBack(GradingStyleEnum.Complete, (int)CompleteIncompleteGradingStyle.Incomplete), 40);
            Assert.AreEqual(mapper.MapBack(GradingStyleEnum.Complete, (int)CompleteIncompleteGradingStyle.Complete), 100);

            Assert.AreEqual(mapper.MapBack(GradingStyleEnum.Check, (int)CheckGradingStyle.CheckMinus), 30);
            Assert.AreEqual(mapper.MapBack(GradingStyleEnum.Check, (int)CheckGradingStyle.Check), 70);
            Assert.AreEqual(mapper.MapBack(GradingStyleEnum.Check, (int)CheckGradingStyle.CheckPlus), 100);

            for (int i = 0; i <= 100; i++)
                Assert.AreEqual(mapper.MapBack(GradingStyleEnum.Numeric100, i), i);
        }

        private GradingStyleMapper MapperDefaulInit()
        {
            IDictionary<GradingStyleEnum, List<int>> mappers = new Dictionary<GradingStyleEnum, List<int>>();

            mappers.Add(GradingStyleEnum.Abcf, new List<int> { 50, 51, 59, 67, 72, 76, 79, 82, 86, 89, 92, 96, 100 });
            mappers.Add(GradingStyleEnum.Check, new List<int> { 30, 70, 100 });
            mappers.Add(GradingStyleEnum.Complete, new List<int> { 40, 100 });

            return GradingStyleMapper.Create(mappers);
        }
    }
}
