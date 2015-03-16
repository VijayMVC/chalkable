using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
using GradingComment = Chalkable.StiConnector.SyncModel.GradingComment;
using ScheduledTimeSlot = Chalkable.StiConnector.SyncModel.ScheduledTimeSlot;
using UserSchool = Chalkable.StiConnector.SyncModel.UserSchool;

namespace Chalkable.Tests.Sis
{
    public class StiApi : TestBase
    {
        [Test]
        public void SyncTest()
        {
            var did = Guid.Parse("840d8256-2e12-4edf-a5e4-f58ed7d71f63");

            
            District d;
            using (UnitOfWork u = new UnitOfWork("Data Source=yqdubo97gg.database.windows.net;Initial Catalog=ChalkableMaster;UID=chalkableadmin;Pwd=Hellowebapps1!", false))
            {
                d = (new DistrictDataAccess(u)).GetById(did);
            }

            var dConnStr = string.Format("Data Source=yqdubo97gg.database.windows.net;Initial Catalog={0};UID=chalkableadmin;Pwd=Hellowebapps1!",
                    did);
            using (UnitOfWork u = new UnitOfWork(dConnStr, false))
            {
                var version =
                    new SyncVersionDataAccess(u).GetAll().First(x => x.TableName.ToLower() == "course").Version;

                var cl = ConnectorLocator.Create(d.SisUserName, d.SisPassword, d.SisUrl);
                var items = (cl.SyncConnector.GetDiff(typeof(Course), version) as SyncResult<Course>);
                var toDelete = items.Deleted.Select(x => x.CourseID);

                var all = new HashSet<int>(new ClassDataAccess(u).GetAll().Select(x=>x.Id));

                IList<Class> classes = new List<Class>();
                foreach (var i in toDelete)
                {
                    if (!all.Contains(i))
                    {
                        Debug.WriteLine("({0}, 'fake{0}'),", i);
                        var c = new Class
                        {
                            Id = i,
                            Name = "fake" + i
                        };
                        classes.Add(c);
                    }
                        
                }
                
                new ClassDataAccess(u).Insert(classes);
            }
        }

        [Test]
        public void SyncTest2()
        {
            var cl = ConnectorLocator.Create("Chalkable", "1iA0wL7zJ", "https://api-houstonco.asc.edu/api/");
            var items = (cl.SyncConnector.GetDiff(typeof(UserSchool), 11699459) as SyncResult<UserSchool>);
            foreach (var gradingComment in items.All)
            {
                Debug.WriteLine("{0} {1} {2} {3}", gradingComment.SchoolID, gradingComment.UserID, gradingComment.DistrictGuid
                    , gradingComment.SYS_CHANGE_VERSION);
            }

            Debug.WriteLine("--------------------");
            foreach (var gradingComment in items.Updated)
            {
                Debug.WriteLine("{0} {1} {2} {3}", gradingComment.SchoolID, gradingComment.UserID, gradingComment.DistrictGuid
                    , gradingComment.SYS_CHANGE_VERSION);
            }

            Debug.WriteLine("--------------------");
            foreach (var gradingComment in items.Deleted)
            {
                Debug.WriteLine("{0} {1} {2} {3}", gradingComment.SchoolID, gradingComment.UserID, gradingComment.DistrictGuid
                    , gradingComment.SYS_CHANGE_VERSION);
            }
        }

        [Test]
        public void Test3()
        {
            var hash = UserService.PasswordMd5("tester");
            Debug.WriteLine(hash);
        }
    }
}
