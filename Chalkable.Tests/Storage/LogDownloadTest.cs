using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using NUnit.Framework;

namespace Chalkable.Tests.Storage
{
    [TestFixture]
    public class LogDownloadTest
    {
        public class WADLogsTable : TableEntity
        {
            public DateTime TimeStamp { get; set; }
            public long EventTickCount { get; set; }
            public string Role { get; set; }
            public string RoleInstance { get; set; }
            public int Level { get; set; }
            public string Message { get; set; }
        }

        [Test]
        public void DownloadLogs()
        {
            var helper = new TableHelper<WADLogsTable>("https", "chalkableqa", "dqWoVG68dHoteECPY1qmCAPloeWdvfMNt2Kjb8SCzT6Qqy3MELQrFfrJ7ZeJXrXxW8RhEn4XkwAvAYtMafqQKw==");
            int count = 10;
            using (var fs = new FileStream("d:\\temp\\qa.log", FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    var sb = new StringBuilder(10000);
                    Debug.WriteLine("Downloading first " + count);
                    var items = helper.GetNext(null, count);
                    Debug.WriteLine("Downloaded " + items.Count);
                    while (items.Count > 0)
                    {
                        foreach (var item in items)
                        {
                            sb.Append(item.RowKey).Append(",")
                              .Append(item.PartitionKey).Append(",")
                              .Append(item.TimeStamp).Append(",")
                              .Append(item.EventTickCount).Append(",")
                              .Append(item.Role).Append(",")
                              .Append(item.RoleInstance).Append(",")
                              .Append(item.Level).Append(",")
                              .Append(item.Message).Append(",");
                            writer.WriteLine(sb.ToString());
                            sb.Clear();
                        }
                        Debug.WriteLine("Downloading next " + count);
                        items = helper.GetNext(items[items.Count-1].RowKey, count);
                        Debug.WriteLine("Downloaded " + items.Count);
                    }
                    
                }
            }
        }
    }
}
