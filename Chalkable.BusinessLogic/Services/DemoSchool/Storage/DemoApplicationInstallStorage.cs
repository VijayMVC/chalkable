using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Web.Hosting;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.ApplicationInstall;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoApplicationInstallStorage:BaseDemoStorage<int, ApplicationInstall>
    {
        public DemoApplicationInstallStorage(DemoStorage storage)
            : base(storage)
        {
        }

        public void Add(ApplicationInstall appInstall)
        {
            data[appInstall.Id] = appInstall;
        }

        public IList<ApplicationInstall> GetAll(int personId)
        {
            return data.Where(x => x.Value.PersonRef== personId).Select(x => x.Value).ToList();
        }

        public IList<ApplicationInstall> GetInstalledForClass(ClassDetails clazz)
        {
            throw new NotImplementedException();
        }

        public IList<ApplicationInstall> GetInstalledByAppId(Guid applicationId, int id)
        {
            throw new NotImplementedException();
        }
    }
}
