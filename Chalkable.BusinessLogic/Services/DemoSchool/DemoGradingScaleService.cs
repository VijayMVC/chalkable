using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoGradingScaleStorage : BaseDemoIntStorage<GradingScale>
    {
        public DemoGradingScaleStorage()
            : base(x => x.Id)
        {
        }
    }

    public class DemoGradingScaleRangeStorage : BaseDemoIntStorage<GradingScaleRange>
    {
        public DemoGradingScaleRangeStorage()
            : base(null, true)
        {
        }
    }

    public class DemoGradingScaleService : DemoSchoolServiceBase, IGradingScaleService
    {
        private DemoGradingScaleStorage GradingScaleStorage { get; set; }
        private DemoGradingScaleRangeStorage GradingScaleRangeStorage { get; set; }
        public DemoGradingScaleService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            GradingScaleStorage = new DemoGradingScaleStorage();
            GradingScaleRangeStorage = new DemoGradingScaleRangeStorage();
        }

        public void AddGradingScales(IList<GradingScale> gradingScales)
        {
            GradingScaleStorage.Add(gradingScales);
        }

        public void EditGradingScales(IList<GradingScale> gradingScales)
        {
            GradingScaleStorage.Update(gradingScales);
        }

        public void DeleteGradingScales(IList<GradingScale> gradingScales)
        {
            GradingScaleStorage.Delete(gradingScales);
        }

        public void AddGradingScaleRanges(IList<GradingScaleRange> gradingScaleRanges)
        {
            GradingScaleRangeStorage.Add(gradingScaleRanges);
        }

        public void EditGradingScaleRanges(IList<GradingScaleRange> gradingScaleRanges)
        {
            GradingScaleRangeStorage.Update(gradingScaleRanges);
        }

        public void DeleteGradingScaleRanges(IList<GradingScaleRange> gradingScaleRanges)
        {
            GradingScaleRangeStorage.Delete(gradingScaleRanges);
        }

        public IList<GradingScale> GetGradingScales(bool onlyAppliedToAlphaGrades = true)
        {
            var res = GradingScaleStorage.GetAll();
            if (!onlyAppliedToAlphaGrades) return res;
            var gradingScalesRange = GradingScaleRangeStorage.GetAll();
            res = res.Where(x => gradingScalesRange.Any(y => y.GradingScaleRef == x.Id)).ToList();
            return res;
        }

        public IList<GradingScaleRange> GetClassGradingScaleRanges(int classId)
        {
            throw new NotImplementedException();
        }

        public GradingScaleRange GetByAlphaGradeId(int alphaGradeId)
        {
             return GradingScaleRangeStorage.GetAll().FirstOrDefault(x => x.AlphaGradeRef == alphaGradeId);
        }
    }
}
