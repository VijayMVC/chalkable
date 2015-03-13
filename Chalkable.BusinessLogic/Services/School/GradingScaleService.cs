using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IGradingScaleService
    {
        void AddGradingScales(IList<GradingScale> gradingScales);
        void EditGradingScales(IList<GradingScale> gradingScales);
        void DeleteGradingScales(IList<GradingScale> gradingScales);

        void AddGradingScaleRanges(IList<GradingScaleRange> gradingScaleRanges);
        void EditGradingScaleRanges(IList<GradingScaleRange> gradingScaleRanges);
        void DeleteGradingScaleRanges(IList<GradingScaleRange> gradingScaleRanges);

    }

    public class GradingScaleService : SchoolServiceBase, IGradingScaleService
    {
        public GradingScaleService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void AddGradingScales(IList<GradingScale> gradingScales)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<GradingScale>(u).Insert(gradingScales));
        }

        public void EditGradingScales(IList<GradingScale> gradingScales)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<GradingScale>(u).Update(gradingScales));
        }

        public void DeleteGradingScales(IList<GradingScale> gradingScales)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<GradingScale>(u).Delete(gradingScales));
        }

        public void AddGradingScaleRanges(IList<GradingScaleRange> gradingScaleRanges)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<GradingScaleRange>(u).Insert(gradingScaleRanges));
        }

        public void EditGradingScaleRanges(IList<GradingScaleRange> gradingScaleRanges)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<GradingScaleRange>(u).Update(gradingScaleRanges));
        }

        public void DeleteGradingScaleRanges(IList<GradingScaleRange> gradingScaleRanges)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<GradingScaleRange>(u).Delete(gradingScaleRanges));
        }
        
    }
}
