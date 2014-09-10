using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
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
            using (var fs = new FileStream("d:\\temp\\qa.log", FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    var sb = new StringBuilder(100000);
                    TableContinuationToken token = null;
                    IList<WADLogsTable> items = helper.GetNext(ref token);
                    Debug.WriteLine("Downloaded " + items.Count);
                    while (token != null)
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
                        writer.Flush();
                        items = helper.GetNext(ref token);
                        Debug.WriteLine("Downloaded " + items.Count);
                    }
                    
                }
            }
        }
    }
}
