using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoApplicationRatingStorage:BaseDemoStorage
    {
        private Dictionary<Guid, ApplicationRating> appRatingData = new Dictionary<Guid, ApplicationRating>();

        public DemoApplicationRatingStorage(DemoStorage storage) : base(storage)
        {
        }

        public bool Exists(Guid applicationId, Guid userId)
        {
            var res = appRatingData.FirstOrDefault(x => x.Value.ApplicationRef == applicationId && x.Value.UserRef == userId);
            return res.Value != null;
        }

        public void Add(ApplicationRating appRating)
        {
            appRatingData[appRating.Id] = appRating;
        }

        public IList<ApplicationRating> GetAll(Guid applicationId)
        {
            return appRatingData.Where(x => x.Value.ApplicationRef == applicationId).Select(x => x.Value).ToList();
        }
    }
}
