using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IGradingScaleService
    {
        void AddGradingScales(IList<GradingScale> gradingScales);
        void EditGradingScales(IList<GradingScale> gradingScales);
        void DeleteGradingScales(IList<int> gradingScalesIds);

        void AddGradingScaleRanges(IList<GradingScaleRange> gradingScaleRanges);
        void EditGradingScaleRanges(IList<GradingScaleRange> gradingScaleRanges);
        void DeleteGradingScaleRanges(IList<GradingScaleRange> gradingScaleAlphaGradeIds);

    }

    public class GradingScaleService : SchoolServiceBase, IGradingScaleService
    {
        public GradingScaleService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void AddGradingScales(IList<GradingScale> gradingScales)
        {
            ModifyGradingScale(da => da.Insert(gradingScales));
        }

        public void EditGradingScales(IList<GradingScale> gradingScales)
        {
            ModifyGradingScale(da => da.Update(gradingScales));
        }

        public void DeleteGradingScales(IList<int> gradingScalesIds)
        {
            ModifyGradingScale(da => da.Delete(gradingScalesIds));
        }

        public void AddGradingScaleRanges(IList<GradingScaleRange> gradingScaleRanges)
        {
            ModifyGradingScaleRange(da => da.Insert(gradingScaleRanges));
        }

        public void EditGradingScaleRanges(IList<GradingScaleRange> gradingScaleRanges)
        {
            ModifyGradingScaleRange(da => da.Update(gradingScaleRanges));
        }

        public void DeleteGradingScaleRanges(IList<GradingScaleRange> gradingScaleAlphaGradeIds)
        {
            ModifyGradingScaleRange(da => da.Delete(gradingScaleAlphaGradeIds));
        }

        private void ModifyGradingScale(Action<GradingScaleDataAccess> modifyMethod)
        {
            Modify(uow => modifyMethod(new GradingScaleDataAccess(uow, Context.SchoolLocalId)));
        }
        private void ModifyGradingScaleRange(Action<GradingScaleRangeDataAccess> modifyMethod)
        {
            Modify(uow => modifyMethod(new GradingScaleRangeDataAccess(uow)));
        }
        private void Modify(Action<UnitOfWork> modifyMethod)
        {
            if(!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                modifyMethod(uow);
                uow.Commit();
            }
        } 
        

    }
}
