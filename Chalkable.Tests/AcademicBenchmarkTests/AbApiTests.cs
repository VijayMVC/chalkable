using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services.Master;
using NUnit.Framework;

namespace Chalkable.Tests.AB_Api
{
    public class AbApiTests
    {

        [Test]
        public void TestSyncApi()
        {
            var abConnector = new AcademicBenchmarkConnector.Connectors.ConnectorLocator();

            var syncData = Task.Run(() => abConnector.SyncConnector.GetStandardsSyncData(new DateTime(2016, 01, 01), 0, 1000)).Result;

            foreach (var resorce in syncData)
            {
                Debug.WriteLine($"{resorce.StandardId} ---- {resorce.ChangeType}");
            }
        }

        [Test]
        public void TestStandardAllCount()
        {
            var abConnector = new AcademicBenchmarkConnector.Connectors.ConnectorLocator();

            var allCount = Task.Run(() => abConnector.StandardsConnector.GetAllStandardCount()).Result;
            
            Debug.WriteLine($"{allCount}");
        }

        [Test]
        public void TestCourses()
        {
            var abConnector = new AcademicBenchmarkConnector.Connectors.ConnectorLocator();
            var courses = Task.Run(() => abConnector.StandardsConnector.GetCourses(null, null, null, "5"));

            Debug.WriteLine(courses.Result.Count);
        }

        [Test]
        public void TestRelations()
        {
            var abConnector = new AcademicBenchmarkConnector.Connectors.ConnectorLocator();
            //var masterService = new ServiceLocatorMaster();

            var startTime = DateTime.Now;
            var relations = Task.Run(() => abConnector.StandardsConnector.GetStandardRelationsById(new Guid("6A59A66C-6EC0-11DF-AB2D-366B9DFF4B22"))).Result;
            var delta = DateTime.Now - startTime;
            //Debug.WriteLine($"{relations.Relations?.Origins?.Count} --- {relations.Relations?.Derivatives?.Count} --- {relations.Relations?.RelatedDerivatives?.Count}");
            Debug.WriteLine(delta);

        }
        
    }
}
