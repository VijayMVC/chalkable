using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoGradedItemStorage : BaseDemoIntStorage<GradedItem>
    {
        public DemoGradedItemStorage()
            : base(x => x.Id)
        {
        }
    }

    public class DemoGradedItemService : DemoSchoolService, IGradedItemService
    {
        private DemoGradedItemStorage GradedItemStorage { get; set; }
        public DemoGradedItemService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            GradedItemStorage = new DemoGradedItemStorage();
        }

        public void Add(IList<GradedItem> gradedItems)
        {
            GradedItemStorage.Add(gradedItems);
        }

        public void Edit(IList<GradedItem> gradedItems)
        {
            GradedItemStorage.Update(gradedItems);
        }

        public void Delete(IList<GradedItem> gradedItems)
        {
            GradedItemStorage.Delete(gradedItems);
        }

        public IList<GradedItem> GetGradedItems(int gradingPeriodId)
        {
            return GradedItemStorage.GetAll().Where(x => x.GradingPeriodRef == gradingPeriodId).ToList();
        }
    }
}
