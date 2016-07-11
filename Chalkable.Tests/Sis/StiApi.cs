﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.SyncModel;
using NUnit.Framework;
using Address = Chalkable.StiConnector.SyncModel.Address;
using AttendanceMonth = Chalkable.StiConnector.SyncModel.AttendanceMonth;
using ContactRelationship = Chalkable.StiConnector.SyncModel.ContactRelationship;
using CourseType = Chalkable.StiConnector.SyncModel.CourseType;
using District = Chalkable.Data.Master.Model.District;
using GradedItem = Chalkable.StiConnector.SyncModel.GradedItem;
using GradeLevel = Chalkable.StiConnector.SyncModel.GradeLevel;
using GradingScale = Chalkable.StiConnector.SyncModel.GradingScale;
using Person = Chalkable.StiConnector.SyncModel.Person;
using Room = Chalkable.StiConnector.SyncModel.Room;
using Standard = Chalkable.StiConnector.SyncModel.Standard;
using Student = Chalkable.StiConnector.SyncModel.Student;
using StudentContact = Chalkable.StiConnector.SyncModel.StudentContact;
using StudentSchool = Chalkable.StiConnector.SyncModel.StudentSchool;
using User = Chalkable.StiConnector.SyncModel.User;
using UserSchool = Chalkable.StiConnector.SyncModel.UserSchool;
using BellSchedule = Chalkable.StiConnector.SyncModel.BellSchedule;
using Infraction = Chalkable.StiConnector.SyncModel.Infraction;


namespace Chalkable.Tests.Sis
{
    public partial class StiApi : TestBase
    {
        [Test]
        public void SyncTest()
        {
            var cl = ConnectorLocator.Create("Chalkable", "g5Hk4By4V", "https://inowhome.dcs.edu/Api/");
            var items = (cl.SyncConnector.GetDiff(typeof(Infraction), null) as SyncResult<Infraction>);
            Print(items.All.Where(x=>x.VisibleInClassroom == true));
            //Print(items.Updated);
            //Print(items.Deleted);
        }
        
        [Test]
        public void FixBellScheduleDelete()
        {
            var ids = new List<Guid>
            {
                Guid.Parse("628F4CA2-7232-418F-A42E-F859286E912A"),
                Guid.Parse("9209de50-0bbe-40f4-8e50-cf52ce39cb2b"),
                
            };
            foreach (var guid in ids)
            {
                FixBellScheduleDelete(guid);
            }
        }

        [Test]
        public void FixRoomDelete()
        {
            var ids = new List<Guid>
            {
                Guid.Parse("ccf4b45d-3357-43ae-9ee8-66949e64279e"),

            };
            foreach (var guid in ids)
            {
                FixRoomDelete(guid);
            }
        }



        [Test]
        public void FixUserSchoolSync()
        {
            var ids = new List<Guid>
            {
                Guid.Parse("F14ABF1C-102B-4B4E-ACC3-1367F6F31069"),

                //Guid.Parse("428CA1B1-BC79-4096-981A-955EF5B2A74B"),

                //Guid.Parse("77A5F7C0-A97E-448F-B619-72799719DC78"), //!!!!!!!!!!!! constantly repeats

                //Guid.Parse("E02C0198-B69B-47F6-871E-C4DE3ECBBE1E")
            };
            foreach (var guid in ids)
            {
                FixUserSchoolSync(guid);
            }
        }

        

        [Test]
        public void FixUserSyncDistricts()
        {
            FixUserSyncAllDistricts();
        }

        public void FixUserSyncAllDistricts()
        {
            var mcs = "Data Source=yqdubo97gg.database.windows.net;Initial Catalog=ChalkableMaster;UID=chalkableadmin;Pwd=Hellowebapps1!";

            IList<District> districts;
            using (var uow = new UnitOfWork(mcs, true))
            {
                var da = new DistrictDataAccess(uow);
                districts = da.GetAll();
            }
            int cnt = 30;
            List<District>[] lists = new List<District>[cnt];
            for (int i = 0; i < cnt; i++)
                lists[i] = new List<District>();
            for (int i = 0; i < districts.Count; i++)
            {
                lists[i%30].Add(districts[i]);
            }
            Thread[] threads = new Thread[cnt];
            for (int i = 0; i < cnt; i++)
            {
                int ii = i;
                var t = new Thread(() =>
                {
                    int k = ii;
                    for (int j = 0; j < lists[k].Count; j++)
                    {
                        FixUserSync(lists[k][j].Id);
                        Debug.WriteLine($"{k} {j} completed");
                    }
                });
                threads[i] = t;
                t.Start();
            }
            for (int i = 0; i < cnt; i++)
                threads[i].Join();
        }

        public void FixUserSync(Guid districtid)
        {
            StringBuilder log = new StringBuilder();
            try
            {
                var mcs = "Data Source=yqdubo97gg.database.windows.net;Initial Catalog=ChalkableMaster;UID=chalkableadmin;Pwd=Hellowebapps1!";

                District d;
                IList<Data.Master.Model.User> chalkableUsers;
                using (var uow = new UnitOfWork(mcs, true))
                {
                    var da = new DistrictDataAccess(uow);
                    d = da.GetById(districtid);
                    var conds = new SimpleQueryCondition("DistrictRef", districtid, ConditionRelation.Equal);
                    chalkableUsers = (new UserDataAccess(uow)).GetAll(conds);
                }
                //var cs = String.Format("Data Source={0};Initial Catalog={1};UID=chalkableadmin;Pwd=Hellowebapps1!", d.ServerUrl, d.Id);

                var cl = ConnectorLocator.Create("Chalkable", d.SisPassword, d.SisUrl);
                var inowUsers = (cl.SyncConnector.GetDiff(typeof(User), null) as SyncResult<User>).All;
                var st = new HashSet<int>(chalkableUsers.Select(x => x.SisUserId.Value).ToList());

                IList<Data.Master.Model.User> users = new List<Data.Master.Model.User>();
                foreach (var sisu in inowUsers)
                    if (!st.Contains(sisu.UserID))
                    {
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
                        log.AppendLine(sisu.UserID.ToString());
                    }

                using (var uow = new UnitOfWork(mcs, true))
                {

                    (new UserDataAccess(uow)).Insert(users);
                    uow.Commit();
                }
                log.AppendLine($"{users.Count} users were added");
            }
            catch (Exception ex)
            {
                log.AppendLine(ex.Message);
                log.AppendLine(ex.StackTrace);
            }
            
            File.WriteAllText($"c:\\tmp\\logs\\{districtid}.txt", log.ToString());
        }

        

        [Test]
        public void Test4()
        {
            var cl = ConnectorLocator.Create("Chalkable", "8nA4qU4yG", "http://sandbox.sti-k12.com/chalkable/api/");
            var items = (cl.SyncConnector.GetDiff(typeof(AcadSession), null) as SyncResult<AcadSession>).All.ToList();
            items = items.ToList();

            var sql = new StringBuilder();
            sql.Append(@"declare @sy table (id int, ArchiveDate datetime2 null) 
                         insert into @sy
                         values");

            foreach ( var item in items)
            {
                var s = string.Format("({0},{1}),", item.AcadSessionID, item.ArchiveDate.HasValue ? "cast('" + item.ArchiveDate.Value + "' as datetime2)" : "null");
                sql.Append(s);
            }
            sql.Append(" ").Append(@"update SchoolYear
                                    set ArchiveDate = sy.ArchiveDate
                                    from SchoolYear 
                                    join @sy sy on SchoolYear.Id = sy.Id");

        
            Debug.WriteLine(sql.ToString());
        }

        [Test]
        public void Test3()
        {
            Debug.WriteLine(DateTime.Now.Month);
        }

    }
}
