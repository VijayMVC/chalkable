using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
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

        IList<GradingScale> GetGradingScales(bool onlyAppliedToAlphaGrades = true);
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

        public IList<GradingScale> GetGradingScales(bool onlyAppliedToAlphaGrades = true)
        {
            using (var u = Read())
            {
                var gradingScales = new DataAccessBase<GradingScale>(u).GetAll(new AndQueryCondition
                    {
                        {GradingScale.SCHOOL_ID_FIELD, Context.SchoolLocalId}
                    });
                var gradingRangeScales = new DataAccessBase<GradingScaleRange>(u).GetAll();
                return gradingScales.Where(x=>gradingRangeScales.Any(y=> y.GradingScaleRef == x.Id)).ToList();
            }
        }
    }
}
