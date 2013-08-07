using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;
using NUnit.Framework;

namespace Chalkable.Tests.Services
{
    public class ServiceTestBase : OnDataBaseTest
    {
        
        protected static void AssertException<TExc>(Action action) where TExc : Exception
        {
            bool wasException = false;
            try
            {
                action();
            }
            catch (Exception ex)
            {
                if (ex is TExc)
                    wasException = true;
            }
            Assert.IsTrue(wasException);
        }

        protected static void AssertAreEqual<T>(T expected, T actual) where T : new()
        {
            var type = typeof(T);
            var fields = type.GetProperties().Where(x => !x.PropertyType.IsClass && !x.PropertyType.IsInterface).ToList();
            foreach (var propertyInfo in fields)
            {
                Assert.AreEqual(propertyInfo.GetValue(expected), propertyInfo.GetValue(actual));
            }
        }

        protected static void AssertAreEqual<T>(IList<T> expected, IList<T> actual) where T : new()
        {
            if (expected == null && actual == null)
                return;
            if (expected == null)
                throw new Exception("Expected list is null but actual isn't");
            if (actual == null)
                throw new Exception("Actual list is null but expected isn't");
            if (expected.Count != actual.Count)
                throw new Exception(string.Format("Different size of lists. expected {0} actual {1}", expected.Count, actual.Count));
            
            var type = typeof (T);
            var isObject = type.IsClass || type.IsInterface;
            for (int i = 0; i < expected.Count; i++)
            {
                if (isObject)
                {
                    AssertAreEqual(expected[i], actual[i]);
                }
                else
                {
                    Assert.AreEqual(expected[i], actual[i]); 
                }
            }
        }



        protected Data.Master.Model.School CreateTestSchool()
        {
            return CreateSimpleSchool("SchoolForTest");
        }
        protected Data.Master.Model.School CreateTestDemoSchool()
        {
            return CreateSimpleSchool("DemoSchoolForTest", true);
        }

        protected Data.Master.Model.School CreateSimpleSchool(string schoolName, bool isDemo = false)
        {
            var chalkableMasterConnection = Settings.MasterConnectionString;
            var masterConnection = chalkableMasterConnection.Replace(MASTER_DB_NAME, "Master");

            var server = Settings.Servers[0];
            var school = new Data.Master.Model.School
            {
                Id = Guid.NewGuid(),
                Name = schoolName,
                IsEmpty = false,
                ServerUrl = server,
                Status = SchoolStatus.PayingCustomer,
                TimeZone = "UTC"
            };
            if (isDemo)
                school.DemoPrefix = Guid.NewGuid().ToString().Replace("-", "");
            using (var uow = new UnitOfWork(chalkableMasterConnection, true))
            {
                var da = new SchoolDataAccess(uow);
                da.Create(school);
                uow.Commit();
                ExecuteQuery(masterConnection, "create database [" + school.Id.ToString() + "]");
                var schoolDbConnectionString = string.Format(Settings.SchoolConnectionStringTemplate, server, school.Id.ToString());
                RunCreateSchoolScripts(schoolDbConnectionString);
            }
            return school;
        }

        protected override void BeforCreateDb(string chalkableConnection, string masterConnection)
        {
            if (ExistsDb(masterConnection, MASTER_DB_NAME))
            {
                using (var uow = new UnitOfWork(chalkableConnection, true))
                {
                    var schools = new SchoolDataAccess(uow).GetSchools();
                    uow.Commit();
                    foreach (var school in schools)
                    {
                        try
                        {
                            DropDbIfExists(masterConnection, school.Id.ToString());
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                    }
                }
            }

            base.BeforCreateDb(chalkableConnection, masterConnection);
        }
    }
}
