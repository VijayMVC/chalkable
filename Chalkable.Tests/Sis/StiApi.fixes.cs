using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        public void FixBellScheduleDelete(Guid districtid)
        {
            District d;
            var mcs = "Data Source=yqdubo97gg.database.windows.net;Initial Catalog=ChalkableMaster;UID=chalkableadmin;Pwd=Hellowebapps1!";

            using (var uow = new UnitOfWork(mcs, false))
            {
                var da = new DistrictDataAccess(uow);
                d = da.GetById(districtid);
            }

            var cs = String.Format("Data Source={0};Initial Catalog={1};UID=chalkableadmin;Pwd=Hellowebapps1!", d.ServerUrl, d.Id);
            IList<SyncVersion> versions;
            using (var uow = new UnitOfWork(cs, true))
            {
                versions = (new SyncVersionDataAccess(uow)).GetAll();
                uow.Commit();
            }

            var cl = ConnectorLocator.Create("Chalkable", d.SisPassword, d.SisUrl);
            var deletedBellSchedules = (cl.SyncConnector.GetDiff(typeof(BellSchedule), versions.First(x => x.TableName == "BellSchedule").Version) as SyncResult<BellSchedule>).Deleted;

            using (var uow = new UnitOfWork(cs, true))
            {
                var da = new DateDataAccess(uow);
                var all = da.GetAll();
                var dates = all.Where(x => deletedBellSchedules.Any(y => x.BellScheduleRef == y.BellScheduleID)).ToList();
                foreach (var date in dates)
                {
                    date.BellScheduleRef = null;

                }
                da.Update(dates.ToList());
                uow.Commit();
            }
        }

        public void FixRoomDelete(Guid districtid)
        {
            District d;
            var mcs = "Data Source=yqdubo97gg.database.windows.net;Initial Catalog=ChalkableMaster;UID=chalkableadmin;Pwd=Hellowebapps1!";

            using (var uow = new UnitOfWork(mcs, false))
            {
                var da = new DistrictDataAccess(uow);
                d = da.GetById(districtid);
            }

            var cs = String.Format("Data Source={0};Initial Catalog={1};UID=chalkableadmin;Pwd=Hellowebapps1!", d.ServerUrl, d.Id);
            IList<SyncVersion> versions;
            using (var uow = new UnitOfWork(cs, true))
            {
                versions = (new SyncVersionDataAccess(uow)).GetAll();
                uow.Commit();
            }

            var cl = ConnectorLocator.Create("Chalkable", d.SisPassword, d.SisUrl);
            var deletedRooms = (cl.SyncConnector.GetDiff(typeof(Room), versions.First(x => x.TableName == "BellSchedule").Version) as SyncResult<Room>).Deleted;

            using (var uow = new UnitOfWork(cs, true))
            {
                var da = new ClassDataAccess(uow);
                var all = da.GetAll();
                var classes = all.Where(x => deletedRooms.Any(y => x.RoomRef == y.RoomID)).ToList();
                foreach (var @class in classes)
                {
                    @class.RoomRef = null;

                }
                da.Update(classes.ToList());
                uow.Commit();
            }
        }

        public void FixUserSchoolSync(Guid districtid)
        {
            var mcs = "Data Source=yqdubo97gg.database.windows.net;Initial Catalog=ChalkableMaster;UID=chalkableadmin;Pwd=Hellowebapps1!";

            District d;
            IList<Data.Master.Model.User> existingUsers;
            using (var uow = new UnitOfWork(mcs, false))
            {
                var da = new DistrictDataAccess(uow);
                d = da.GetById(districtid);
                var conds = new SimpleQueryCondition("DistrictRef", districtid, ConditionRelation.Equal);
                existingUsers = (new UserDataAccess(uow)).GetAll(conds);
            }
            var cs = String.Format("Data Source={0};Initial Catalog={1};UID=chalkableadmin;Pwd=Hellowebapps1!", d.ServerUrl, d.Id);
            IList<SyncVersion> versions;
            using (var uow = new UnitOfWork(cs, true))
            {
                versions = (new SyncVersionDataAccess(uow)).GetAll();
                uow.Commit();
            }

            var cl = ConnectorLocator.Create("Chalkable", d.SisPassword, d.SisUrl);
            var addedUsers = (cl.SyncConnector.GetDiff(typeof(User), versions.First(x => x.TableName == "User").Version) as SyncResult<User>).Inserted;
            var AllUsers = (cl.SyncConnector.GetDiff(typeof(User), null) as SyncResult<User>).All;
            var addedUserSchools = (cl.SyncConnector.GetDiff(typeof(UserSchool), versions.First(x => x.TableName == "UserSchool").Version) as SyncResult<UserSchool>).Inserted;

            IList<Data.Master.Model.User> users = new List<Data.Master.Model.User>();
            var ids = addedUserSchools.Select(x => x.UserID).Distinct();
            foreach (var addedUserSchool in ids)
            {
                if (existingUsers.All(x => x.SisUserId != addedUserSchool))
                {
                    if (addedUsers.All(x => x.UserID != addedUserSchool))
                    {
                        var sisu = AllUsers.First(x => x.UserID == addedUserSchool);
                        Data.Master.Model.User u = new Data.Master.Model.User
                        {
                            Id = Guid.NewGuid(),
                            DistrictRef = districtid,
                            FullName = sisu.FullName,
                            Login = String.Format("user{0}_{1}@chalkable.com", sisu.UserID, districtid),
                            Password = "1Ztq1N1GZ95sasjFa54ikw==",
                            SisUserName = sisu.UserName,
                            SisUserId = sisu.UserID
                        };
                        users.Add(u);
                    }
                }
            }


            using (var uow = new UnitOfWork(mcs, true))
            {

                (new UserDataAccess(uow)).Insert(users);
                uow.Commit();
            }
        }
    }
}