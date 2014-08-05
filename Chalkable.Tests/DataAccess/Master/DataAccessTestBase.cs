using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Common;
using Chalkable.Data.Common;
using NUnit.Framework;

namespace Chalkable.Tests.DataAccess.Master
{
    public class DataAccessTestBase : OnDataBaseTest
    {
        
        protected UnitOfWork UnitOfWork { get; private set; }
        [SetUp]
        public void SetUp()
        {
            CreateMasterDb();

            var connectionString = Settings.MasterConnectionString;
            UnitOfWork = new UnitOfWork(connectionString, true);
            
        }

        [TearDown]
        public void TearDown()
        {
            UnitOfWork.Commit();
            UnitOfWork.Dispose();
        }
    }
}
