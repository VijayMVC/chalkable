using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.SyncModel;
using NUnit.Framework;
using AttendanceMonth = Chalkable.StiConnector.SyncModel.AttendanceMonth;
using ContactRelationship = Chalkable.StiConnector.SyncModel.ContactRelationship;
using District = Chalkable.Data.Master.Model.District;
using GradedItem = Chalkable.StiConnector.SyncModel.GradedItem;
using Standard = Chalkable.StiConnector.SyncModel.Standard;
using StudentContact = Chalkable.StiConnector.SyncModel.StudentContact;
using StudentSchool = Chalkable.StiConnector.SyncModel.StudentSchool;
using User = Chalkable.StiConnector.SyncModel.User;
using UserSchool = Chalkable.StiConnector.SyncModel.UserSchool;


namespace Chalkable.Tests.Sis
{
    public class StiApi : TestBase
    {
        [Test]
        public void SyncTest()
        {

            var cl = ConnectorLocator.Create("Chalkable", "1aC6wI3qQ", "https://inow.tcss.net/API/");
            var items = (cl.SyncConnector.GetDiff(typeof(User), 58050246) as SyncResult<User>);
            Print(items.Inserted.OrderBy(x=>x.UserID));
            Print(items.Updated);
            Print(items.Deleted);
        }

        [Test]
        public void FixUserSchoolSync()
        {
            var ids = new List<Guid>
            {
                Guid.Parse("F14ABF1C-102B-4B4E-ACC3-1367F6F31069"),
                Guid.Parse("E02C0198-B69B-47F6-871E-C4DE3ECBBE1E")

            };
            foreach (var guid in ids)
            {
                FixUserSchoolSync(guid);
            }
        }

        public void FixUserSchoolSync(Guid districtid)
        {
            var mcs = "Data Source=yqdubo97gg.database.windows.net;Initial Catalog=ChalkableMaster;UID=chalkableadmin;Pwd=Hellowebapps1!";
            
            District d;
            IList<Data.Master.Model.User> existingUsers;
            using (var uow = new UnitOfWork(mcs, true))
            {
                var da = new DistrictDataAccess(uow);
                d = da.GetById(districtid);
                var conds = new SimpleQueryCondition("DistrictRef", districtid, ConditionRelation.Equal);
                existingUsers = (new UserDataAccess(uow)).GetAll(conds);

                uow.Commit();
            }
            var cs = String.Format("Data Source={0};Initial Catalog={1};UID=chalkableadmin;Pwd=Hellowebapps1!", d.ServerUrl, d.Id);
            IList<SyncVersion> versions;
            using (var uow = new UnitOfWork(cs, true))
            {
                versions = (new SyncVersionDataAccess(uow)).GetAll();
                uow.Commit();
            }

            var cl = ConnectorLocator.Create("Chalkable", d.SisPassword, d.SisUrl);
            var addedUsers = (cl.SyncConnector.GetDiff(typeof(User), versions.First(x=>x.TableName=="User").Version) as SyncResult<User>).Inserted;
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

        private void Print(IEnumerable<StudentScheduleTerm> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.StudentID} {item.SectionID} {item.TermID} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
        }

        private void Print(IEnumerable<PersonTelephone> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.PersonID} {item.TelephoneNumber} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
        }
        
        private void Print(IEnumerable<Course> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.CourseID} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
        }

        private void Print(IEnumerable<User> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.UserID} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
        }
        
        private void Print(IEnumerable<UserSchool> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.UserID} {item.SchoolID} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
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
