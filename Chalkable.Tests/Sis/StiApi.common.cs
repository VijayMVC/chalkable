using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.SyncModel;
using BellSchedule = Chalkable.StiConnector.SyncModel.BellSchedule;
using District = Chalkable.Data.Master.Model.District;
using UserSchool = Chalkable.StiConnector.SyncModel.UserSchool;
using Room = Chalkable.StiConnector.SyncModel.Room;

namespace Chalkable.Tests.Sis
{

    public partial class StiApi : TestBase
    {
        void ForEachDistrict(IEnumerable<Guid> districtIds, Action<District, ConnectorLocator, UnitOfWork> action)
        {
            foreach (var districtId in districtIds)
            {
                var mcs = "Data Source=yqdubo97gg.database.windows.net;Initial Catalog=ChalkableMaster;UID=chalkableadmin;Pwd=Hellowebapps1!";
                District d;
                using (var uow = new UnitOfWork(mcs, false))
                {
                    var da = new DistrictDataAccess(uow);
                    d = da.GetById(districtId);
                }
                var cl = ConnectorLocator.Create(d.SisUserName, d.SisPassword, d.SisUrl);
                var cs = $"Data Source={d.ServerUrl};Initial Catalog={d.Id};UID=chalkableadmin;Pwd=Hellowebapps1!";
                using (var uow = new UnitOfWork(cs, true))
                {
                    action(d, cl, uow);
                    uow.Commit();
                }
                Debug.WriteLine($"district {d.Id} processing is done");
            }
        }

        void ForEachDistrict(IEnumerable<Guid> districtIds, Action<District, ConnectorLocator, UnitOfWork, UnitOfWork> action)
        {
            foreach (var districtId in districtIds)
            {
                var mcs = "Data Source=yqdubo97gg.database.windows.net;Initial Catalog=ChalkableMaster;UID=chalkableadmin;Pwd=Hellowebapps1!";
                District d;
                using (var muow = new UnitOfWork(mcs, true))
                {
                    var da = new DistrictDataAccess(muow);
                    d = da.GetById(districtId);

                    var cl = ConnectorLocator.Create(d.SisUserName, d.SisPassword, d.SisUrl);
                    var cs = $"Data Source={d.ServerUrl};Initial Catalog={d.Id};UID=chalkableadmin;Pwd=Hellowebapps1!";
                    using (var uow = new UnitOfWork(cs, true))
                    {
                        action(d, cl, muow, uow);
                        uow.Commit();
                    }
                    muow.Commit();
                }
                Debug.WriteLine($"district {d.Id} processing is done");
            }
        }

        void ForEachDistrict(IEnumerable<Guid> districtIds, Action<District, UnitOfWork> action)
        {
            foreach (var districtId in districtIds)
            {
                var mcs = "Data Source=yqdubo97gg.database.windows.net;Initial Catalog=ChalkableMaster;UID=chalkableadmin;Pwd=Hellowebapps1!";
                District d;
                using (var uow = new UnitOfWork(mcs, false))
                {
                    var da = new DistrictDataAccess(uow);
                    d = da.GetById(districtId);
                }
                var cs = $"Data Source={d.ServerUrl};Initial Catalog={d.Id};UID=chalkableadmin;Pwd=Hellowebapps1!";
                using (var uow = new UnitOfWork(cs, true))
                {
                    action(d, uow);
                    uow.Commit();
                }
                Debug.WriteLine($"district {d.Id} processing is done");
            }
        }

        void ForEachDistrict(IEnumerable<Guid> districtIds, Action<District, ConnectorLocator> action)
        {
            foreach (var districtId in districtIds)
            {
                var mcs = "Data Source=yqdubo97gg.database.windows.net;Initial Catalog=ChalkableMaster;UID=chalkableadmin;Pwd=Hellowebapps1!";
                District d;
                using (var uow = new UnitOfWork(mcs, false))
                {
                    var da = new DistrictDataAccess(uow);
                    d = da.GetById(districtId);
                }
                var cl = ConnectorLocator.Create(d.SisUserName, d.SisPassword, d.SisUrl);
                action(d, cl);
                Debug.WriteLine($"district {d.Id} processing is done");
            }
        }

    }
}
