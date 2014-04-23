﻿using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Web.Hosting;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.ApplicationInstall;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoApplicationInstallActionStorage:BaseDemoStorage<int, ApplicationInstallAction>
    {
        public DemoApplicationInstallActionStorage(DemoStorage storage)
            : base(storage)
        {
        }

        public void Add(List<ApplicationInstallAction> appInstall)
        {
            foreach (var applicationInstall in appInstall)
            {
                Add(applicationInstall);
            }
        }

        public void Add(ApplicationInstallAction appInstall)
        {
            var id = GetNextFreeId();
            appInstall.Id = id;
            data[appInstall.Id] = appInstall;
        }

        public ApplicationInstallAction GetLastAppInstallAction(Guid id, int userId)
        {
            return data.OrderByDescending(x => x.Value.Id).First(x => x.Value.ApplicationRef == id && x.Value.OwnerRef == userId).Value;
        }

        public void Update(ApplicationInstallAction res)
        {
            if (data.ContainsKey(res.Id))
                data[res.Id] = res;
        }

        public override void Setup()
        {
            throw new NotImplementedException();
        }
    }
}
