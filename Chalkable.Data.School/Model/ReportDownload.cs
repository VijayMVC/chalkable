using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.School.Model
{
    public class ReportDownload
    {
        public Guid Id { get; set; }
        public int Format { get; set; }
        public int ImportSystemType { get; set; }
        public Guid PersonRef { get; set; }
        public ReportType ReportType { get; set; }
        public DateTime DownloadDate { get; set; }
        public string FriendlyName { get; set; }
    }

    public enum ReportType
    {
        People = 1,
        Attendance = 2,
        Discipline = 3
    }
}
