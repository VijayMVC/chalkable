using System.Collections.Generic;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;
using System.Linq;

namespace Chalkable.BusinessLogic.Mapping
{
    public interface IGradingStyleMapper
    {
        int? Map(GradingStyleEnum styleEnum, int? value);
        int? MapBack(GradingStyleEnum styleEnum, int? styledValue);
        IList<int> GetValuesByStyle(GradingStyleEnum styleEnum);
    }

    public class GradingStyleMapper : IGradingStyleMapper
    {
        private GradingStyleMapper(){}
        
        private IDictionary<GradingStyleEnum, List<int>> mappers;
        
        public static GradingStyleMapper Create(IList<GradingStyle> gradingStyles)
        {
            var res = new GradingStyleMapper {mappers = new Dictionary<GradingStyleEnum, List<int>>()};
            res.mappers[GradingStyleEnum.Abcf] = new List<int>();
            res.mappers[GradingStyleEnum.Complete] = new List<int>();
            res.mappers[GradingStyleEnum.Check] = new List<int>();
            gradingStyles = gradingStyles.OrderByDescending(x => x.StyledValue).ToList();
            foreach (var gradingStyle in gradingStyles)
            {
                res.mappers[gradingStyle.GradingStyleValue].Add(gradingStyle.MaxValue);
            }
            return res;
        }

        public static GradingStyleMapper Create(IDictionary<GradingStyleEnum, List<int>> mappers)
        {
            return new GradingStyleMapper {mappers = mappers};
        }

        public int? Map(GradingStyleEnum styleEnum, int? value) 
        {
            if (!value.HasValue)
                return null;
            if (value < 0)
                throw new ChalkableException(string.Format(ChlkResources.ERR_GRADING_STYLE_INVALID_GRADE_VALUE, value));
            if (styleEnum == GradingStyleEnum.Numeric100)
            {
                if (value.Value > 100)
                    throw new ChalkableException(string.Format(ChlkResources.ERR_GRADING_STYLE_INVALID_GRADE_VALUE, value));
                return value;
            }
                
            IList<int> mapping = mappers[styleEnum];
            
            int i;
            for (i = 0; i < mapping.Count; i++)
                if (value <= mapping[i])
                    break;
            if (i == mapping.Count)
                if (value < 0)
                    throw new ChalkableException(string.Format(ChlkResources.ERR_GRADING_STYLE_INVALID_GRADE_VALUE, value));
            return i;
        }

        public int? MapBack(GradingStyleEnum styleEnum, int? styledValue)
        {
            if (!styledValue.HasValue)
                return null;
            if (styledValue < 0)
                throw new ChalkableException(string.Format(ChlkResources.ERR_GRADING_STYLE_INVALID_GRADE_VALUE, styledValue));
            if (styleEnum == GradingStyleEnum.Numeric100)
            {
                return styledValue;
            }
            IList<int> mapping = mappers[styleEnum];
            if (styledValue.Value >= mapping.Count)
                throw new ChalkableException(string.Format(ChlkResources.ERR_GRADING_STYLE_INVALID_GRADE_VALUE, styledValue));
            return mapping[styledValue.Value];
        }

        public IList<int> GetValuesByStyle(GradingStyleEnum styleEnum)
        {
            return mappers[styleEnum];
        }

        public static IGradingStyleMapper CreateDefault()
        {
            IDictionary<GradingStyleEnum, List<int>> mappers = new Dictionary<GradingStyleEnum, List<int>>();
            mappers.Add(GradingStyleEnum.Abcf, new List<int> { 50, 51, 59, 67, 72, 76, 79, 82, 86, 89, 92, 96, 100 });
            mappers.Add(GradingStyleEnum.Check, new List<int> { 30, 70, 100 });
            mappers.Add(GradingStyleEnum.Complete, new List<int> { 40, 100 });
            return Create(mappers);
        }
    }
}
