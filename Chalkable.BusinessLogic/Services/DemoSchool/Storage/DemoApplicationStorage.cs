using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoApplicationStorage:BaseDemoStorage
    {
        private readonly Dictionary<Guid, Application> applicationData = new Dictionary<Guid, Application>();


        public DemoApplicationStorage(DemoStorage storage) : base(storage)
        {
        }

        public Application GetApplicationById(Guid applicationId)
        {
            if (applicationData.ContainsKey(applicationId))
            {
                return applicationData[applicationId];
            }
            return null;
        }

        public Application GetApplicationByUrl(string applicationUrl)
        {
            return applicationData.Where(x => x.Value.Url == applicationUrl).Select(x => x.Value).FirstOrDefault();
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
