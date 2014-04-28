using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    //TODO: implementation
    public class DemoGradingScaleService : DemoSchoolServiceBase, IGradingScaleService
    {
        public DemoGradingScaleService(IServiceLocatorSchool serviceLocator, DemoStorage demoStorage) : base(serviceLocator, demoStorage)
        {
        }

        public void AddGradingScales(IList<GradingScale> gradingScales)
        {
            throw new NotImplementedException();
        }

        public void EditGradingScales(IList<GradingScale> gradingScales)
        {
            throw new NotImplementedException();
        }

        public void DeleteGradingScales(IList<int> gradingScalesIds)
        {
            throw new NotImplementedException();
        }

        public void AddGradingScaleRanges(IList<GradingScaleRange> gradingScaleRanges)
        {
            throw new NotImplementedException();
        }

        public void EditGradingScaleRanges(IList<GradingScaleRange> gradingScaleRanges)
        {
            throw new NotImplementedException();
        }

        public void DeleteGradingScaleRanges(IDictionary<int, int> gradingScaleAlphaGradeIds)
        {
            throw new NotImplementedException();
        }
    }
}
