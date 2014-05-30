using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoApplicationRatingStorage:BaseDemoGuidStorage<ApplicationRating>
    {
        public DemoApplicationRatingStorage(DemoStorage storage) : base(storage, x => x.Id)
        {
        }

        public bool Exists(Guid applicationId, Guid userId)
        {
            return data.Count(x => x.Value.ApplicationRef == applicationId && x.Value.UserRef == userId) == 1;
        }

        public IList<ApplicationRating> GetAll(Guid applicationId)
        {
            return data.Where(x => x.Value.ApplicationRef == applicationId).Select(x => x.Value).ToList();
        }
    }
}
