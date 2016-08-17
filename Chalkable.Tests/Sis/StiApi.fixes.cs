using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.SyncModel;
using BellSchedule = Chalkable.StiConnector.SyncModel.BellSchedule;
using District = Chalkable.Data.Master.Model.District;
using UserSchool = Chalkable.StiConnector.SyncModel.UserSchool;
using Room = Chalkable.StiConnector.SyncModel.Room;
using User = Chalkable.StiConnector.SyncModel.User;

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
            var deletedRooms = (cl.SyncConnector.GetDiff(typeof(Room), versions.First(x => x.TableName == "Room").Version) as SyncResult<Room>).Deleted;

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

        public void FixMissingUsersSync(Guid districtid)
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

            var cl = ConnectorLocator.Create(d.SisUserName, d.SisPassword, d.SisUrl);
            var addedUsers = (cl.SyncConnector.GetDiff(typeof(User), versions.First(x => x.TableName == "User").Version) as SyncResult<User>).Inserted;
            var allInowUsers = (cl.SyncConnector.GetDiff(typeof(User), null) as SyncResult<User>).All;
            var allUsers = (cl.SyncConnector.GetDiff(typeof(User), null) as SyncResult<User>).All;
            //var addedUserSchools = (cl.SyncConnector.GetDiff(typeof(UserSchool), versions.First(x => x.TableName == "UserSchool").Version) as SyncResult<UserSchool>).Inserted;

            IList<Data.Master.Model.User> users = new List<Data.Master.Model.User>();
            var ids = allInowUsers.Select(x => x.UserID).Distinct();
            var existingUserSet = new HashSet<int>(existingUsers.Select(x => x.SisUserId.Value).ToList());
            foreach (var addedUserSchool in ids)
            {
                if (!existingUserSet.Contains(addedUserSchool))
                {
                    if (addedUsers.All(x => x.UserID != addedUserSchool))
                    {
                        var sisu = allUsers.First(x => x.UserID == addedUserSchool);
                        Data.Master.Model.User u = new Data.Master.Model.User
                        {
                            Id = Guid.NewGuid(),
                            DistrictRef = districtid,
                            FullName = sisu.FullName,
                            Login = $"user{sisu.UserID}_{districtid}@chalkable.com",
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
        
        private void FixMissingSchoolUsersSync(IList<Guid> districtIds)
        {
            ForEachDistrict(districtIds, delegate (District d, ConnectorLocator cl, UnitOfWork mu, UnitOfWork du)
            {
                var inowSchoolUsers = (cl.SyncConnector.GetDiff(typeof(UserSchool), null) as SyncResult<UserSchool>).All;
                var conds = new SimpleQueryCondition("DistrictRef", d.Id, ConditionRelation.Equal);
                var chalkableSchoolUsers = (new DataAccessBase<SchoolUser>(mu)).GetAll(conds);
                var toAdd = new List<SchoolUser>();
                foreach (var inowSchoolUser in inowSchoolUsers)
                {
                    if (
                        !chalkableSchoolUsers.Any(
                            x => x.SchoolRef == inowSchoolUser.SchoolID && x.UserRef == inowSchoolUser.UserID))
                    {
                        toAdd.Add(new SchoolUser()
                        {
                            SchoolRef = inowSchoolUser.SchoolID,
                            UserRef = inowSchoolUser.UserID,
                            DistrictRef = d.Id
                        });
                        Debug.WriteLine($"adding {inowSchoolUser.UserID} {inowSchoolUser.SchoolID}");
                    }
                        
                }
                (new DataAccessBase<SchoolUser>(mu)).Insert(toAdd);
            });
        }
    }
}