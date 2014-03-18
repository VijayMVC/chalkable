using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Mapping;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{

    public class DemoGradingStyleService : DemoSchoolServiceBase, IGradingStyleService
    {
        private List<GradingStyle> gradingStyles = new List<GradingStyle>();
 
        public DemoGradingStyleService(IServiceLocatorSchool serviceLocator, DemoStorage demoStorage)
            : base(serviceLocator ,demoStorage)
        {
        }

        public IGradingStyleMapper GetMapper()
        {
            var list = GetGradingStyles();
            if (list.Count == 0)
                return GradingStyleMapper.CreateDefault();
            return GradingStyleMapper.Create(list);
        }

        public void SetMapper(IGradingStyleMapper mapper)
        {

            if (!BaseSecurity.IsAdminGrader(Context))
                throw new ChalkableSecurityException();

            gradingStyles.Clear();
            gradingStyles.AddRange(AddGradingStyleItems(GradingStyleEnum.Abcf, mapper.GetValuesByStyle(GradingStyleEnum.Abcf)));
            gradingStyles.AddRange(AddGradingStyleItems(GradingStyleEnum.Complete, mapper.GetValuesByStyle(GradingStyleEnum.Complete)));
            gradingStyles.AddRange(AddGradingStyleItems(GradingStyleEnum.Check, mapper.GetValuesByStyle(GradingStyleEnum.Check)));

        }

        public IList<GradingStyle> GetGradingStyles()
        {
            return gradingStyles;
        }

        private IEnumerable<GradingStyle> AddGradingStyleItems(GradingStyleEnum style, IList<int> values)
        {
            var res = new List<GradingStyle>();
            for (int i = 0; i < values.Count; i++)
                res.Add(new GradingStyle()
                {
                    Id = Guid.NewGuid(),
                    GradingStyleValue = style,
                    MaxValue = values[i],
                    StyledValue = i
                });
            return res;
        }



    }
}
