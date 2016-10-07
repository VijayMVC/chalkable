using System;
using Chalkable.Data.Common;

namespace Chalkable.Data.AcademicBenchmark.Model
{
    public class SyncLastDate
    {
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public DateTime Date { get; set; }

        public static SyncLastDate Create(int id, DateTime date)
        {
            return new SyncLastDate
            {
                Id = id,
                Date = date
            };
        }
    }
}
