﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    }
}
