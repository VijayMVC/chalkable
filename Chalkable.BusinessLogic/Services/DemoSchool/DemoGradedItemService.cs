using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoGradedItemService : DemoSchoolService, IGradedItemService
    {
        public DemoGradedItemService(IServiceLocatorSchool serviceLocator, DemoStorage demoStorage) : base(serviceLocator, demoStorage)
        {
        }

        public void Add(IList<GradedItem> gradedItems)
        {
            throw new NotImplementedException();
        }

        public void Edit(IList<GradedItem> gradedItems)
        {
            throw new NotImplementedException();
        }

        public void Delete(IList<GradedItem> gradedItems)
        {
            throw new NotImplementedException();
        }

        public IList<GradedItem> GetGradedItems(int gradingPeriodId)
        {
            return Storage.GradedItemStorage.GetAll().Where(x => x.GradingPeriodRef == gradingPeriodId).ToList();
        }
    }
}
