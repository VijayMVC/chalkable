using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoApplicationStorage:BaseDemoStorage<Guid, Application>
    {
        public DemoApplicationStorage(DemoStorage storage) : base(storage)
        {
        }

        public Application GetApplicationByUrl(string applicationUrl)
        {
            return data.Where(x => x.Value.Url == applicationUrl).Select(x => x.Value).FirstOrDefault();
        }

        public PaginatedList<Application> GetPaginatedApplications(ApplicationQuery query)
        {
            //return general apps for other roles return apps for dev
            throw new NotImplementedException();
        }

        public Application GetApplication(ApplicationQuery applicationQuery)
        {
            throw new NotImplementedException();
        }
    }
}
