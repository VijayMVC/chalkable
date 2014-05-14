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
            Storage.GradingScaleStorage.Add(gradingScales);
        }

        public void EditGradingScales(IList<GradingScale> gradingScales)
        {
            Storage.GradingScaleStorage.Edit(gradingScales);
        }

        public void DeleteGradingScales(IList<int> gradingScalesIds)
        {
            Storage.GradingScaleStorage.Delete(gradingScalesIds);
        }

        public void AddGradingScaleRanges(IList<GradingScaleRange> gradingScaleRanges)
        {
            Storage.GradingScaleRangeStorage.Add(gradingScaleRanges);
        }

        public void EditGradingScaleRanges(IList<GradingScaleRange> gradingScaleRanges)
        {
            Storage.GradingScaleRangeStorage.Edit(gradingScaleRanges);
        }

        public void DeleteGradingScaleRanges(IList<GradingScaleRange> gradingScaleAlphaGradeIds)
        {
            throw new NotImplementedException();
        }
    }
}
