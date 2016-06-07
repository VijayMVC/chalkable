using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.School.Model.Announcements
{
    internal class SisActivityCopyResult
    {
        public int FromActivityId { get; set; }
        public int ToActivityId { get; set; }

        public static IList<SisActivityCopyResult> Create(IDictionary<int, int> sisFromToActivityIds)
        {
            return sisFromToActivityIds.Select(x => new SisActivityCopyResult
            {
                FromActivityId = x.Key,
                ToActivityId = x.Value
            }).ToList();
        }
    }
}
