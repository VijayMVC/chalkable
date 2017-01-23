using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IGradedItemService
    {
        void Add(IList<GradedItem> gradedItems);
        void Edit(IList<GradedItem> gradedItems);
        void Delete(IList<GradedItem> gradedItems);
        IList<GradedItem> GetGradedItems(int gradingPeriodId);
    }

    public class GradedItemService : SchoolServiceBase, IGradedItemService
    {
        public GradedItemService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void Add(IList<GradedItem> gradedItems)
        {
            DoUpdate(u => new DataAccessBase<GradedItem>(u).Insert(gradedItems));
        }

        public void Edit(IList<GradedItem> gradedItems)
        {
            DoUpdate(u => new DataAccessBase<GradedItem>(u).Update(gradedItems));
        }

        public void Delete(IList<GradedItem> gradedItems)
        {
            DoUpdate(u => new DataAccessBase<GradedItem>(u).Delete(gradedItems));
        }

        public IList<GradedItem> GetGradedItems(int gradingPeriodId)
        {
            var conds = new AndQueryCondition() {{GradedItem.GRADING_PERIOD_REF_FIELD, gradingPeriodId}};
            return DoRead(u => new DataAccessBase<GradedItem>(u).GetAll(conds));
        }
    }
}
