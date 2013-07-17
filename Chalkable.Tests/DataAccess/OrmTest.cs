using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.DataAccess;
using NUnit.Framework;

namespace Chalkable.Tests.DataAccess
{
    public class OrmTest : TestBase
    {
        [Test]
        public void TestQueryBuilding()
        {
            long start = DateTime.Now.Ticks;
            var fields = Orm.Fields<User>();
            Debug.WriteLine(fields.JoinString(","));
            var u = new User();
            u.Id = Guid.NewGuid();
            u.Password = "some pwd";
            u.Login = "login";
            u.IsSysAdmin = true;
            u.IsDeveloper = false;
            var q = Orm.SimpleInsert(u);
            Debug.WriteLine(q.Sql);
            Debug.WriteLine(q.Parameters.Select(x=>x.Key + "=" + x.Value).JoinString("\n"));

            q = Orm.SimpleUpdate(u);
            Debug.WriteLine(q.Sql);
            Debug.WriteLine(q.Parameters.Select(x => x.Key + "=" + x.Value).JoinString("\n"));

            q = Orm.SimpleDelete(u);
            Debug.WriteLine(q.Sql);
            Debug.WriteLine(q.Parameters.Select(x => x.Key + "=" + x.Value).JoinString("\n"));

            Debug.WriteLine("Duration: " + (DateTime.Now.Ticks - start) / 10000);
        }
    }
}
