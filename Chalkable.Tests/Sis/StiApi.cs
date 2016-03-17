﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Data.Common;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.SyncModel;
using NUnit.Framework;
using AttendanceMonth = Chalkable.StiConnector.SyncModel.AttendanceMonth;
using ContactRelationship = Chalkable.StiConnector.SyncModel.ContactRelationship;
using GradedItem = Chalkable.StiConnector.SyncModel.GradedItem;
using Standard = Chalkable.StiConnector.SyncModel.Standard;
using StudentContact = Chalkable.StiConnector.SyncModel.StudentContact;
using StudentSchool = Chalkable.StiConnector.SyncModel.StudentSchool;


namespace Chalkable.Tests.Sis
{
    public class StiApi : TestBase
    {
        [Test]
        public void SyncTest()
        {

            var cl = ConnectorLocator.Create("Chalkable", "Ti2Pb2Rz0", "http://208.83.95.80:8295/API/");
            var items = (cl.SyncConnector.GetDiff(typeof(Standard), 2321500) as SyncResult<Standard>);
            //Print(items.Inserted);
            //Print(items.Updated);
            Print(items.Deleted.OrderBy(x=>x.SYS_CHANGE_VERSION));
        }

        private void Print(IEnumerable<Standard> items)
        {
            foreach (var standard in items)
            {
                Debug.WriteLine("delete from standard where id = {0}", standard.StandardID);
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
