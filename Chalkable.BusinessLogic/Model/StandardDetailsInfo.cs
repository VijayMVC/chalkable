using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Model
{
    public class StandardDetailsInfo
    {
        public Standard Standard { get; set; }
        public CommonCoreStandard CommonCoreStandard { get; set; }

        public static StandardDetailsInfo Create(Standard standard, CommonCoreStandard commonCoreStandard)
        {
            var res = new StandardDetailsInfo {Standard = standard};
            if (commonCoreStandard != null)
            {
                res.CommonCoreStandard = commonCoreStandard;
                res.Standard.CCStandardCode = commonCoreStandard.Code;
            }
            return res;
        }
    }
}
